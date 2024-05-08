using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Windows.Ink;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System.Collections.Immutable;
using System.Windows.Input;
using WPFLabaSCOIApp.Models;
using System.Collections.Specialized;
using System.Windows;
using System.CodeDom;
using System.Reflection;
namespace WPFLabaSCOIApp.ViewModels
{
    internal class GradationTransformWindowVM : INotifyPropertyChanged
    {
        public GradationTransformWindowVM(BitmapSource image)
        {
            AddPointVM(new PointVM(0, 0));
            AddPointVM(new PointVM(255, 255));
            Bitmap = image;
            GraphModel = UpdatePlotModel();
            Points.CollectionChanged += Points_CollectionChanged;
            _newPoint = new System.Drawing.Point(1,1);
        }
        private ICommand deleteCommand;
        private ICommand addCommand;
        private BitmapSource _bitmap;
        private PlotModel _graphModel;
        private new System.Drawing.Point _newPoint;
        public PlotModel GraphModel
        {
            get { return _graphModel; }
            set
            {
                _graphModel = value;
                OnPropertyChanged("GraphModel");
            }
        }
        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set {
                _bitmap = value;
                OnPropertyChanged("Bitmap");
            }
        }
        public int NewPointX
        {
            get { return _newPoint.X; }
            set
            {
                _newPoint.X = value;
                OnPropertyChanged("NewPointX");
            }
        }
        public int NewPointY
        {
            get { return _newPoint.Y; }
            set
            {
                _newPoint.Y = value;
                OnPropertyChanged("NewPointY");
            }
        }
        public ObservableCollection<PointVM> Points { get; set; } = new ObservableCollection<PointVM>();

        void Points_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is PointVM newPoint)
                    {
                        GraphModel = UpdatePlotModel();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    GraphModel = UpdatePlotModel();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                default:
                    GraphModel = UpdatePlotModel();
                    break;
            }
        }

        public void AddPointVM(PointVM pointVM)
        {
            Points.Add(pointVM);
            pointVM.PropertyChanged += (s, prop_name) =>
            {
                if (prop_name.PropertyName == nameof(PointVM.X)
                    || prop_name.PropertyName == nameof(PointVM.Y))
                {
                    GraphModel = UpdatePlotModel();
                }
            };
        }
        public void InsertPointVM(int index, PointVM pointVM)
        {
            Points.Insert(index, pointVM);
            pointVM.PropertyChanged += (s, prop_name) =>
            {
                if (prop_name.PropertyName == nameof(PointVM.X)
                    || prop_name.PropertyName == nameof(PointVM.Y))
                {
                    GraphModel = UpdatePlotModel();
                }
            };
        }
        public double Interpolation(double xValue) // Линейная
        {
            PointVM point1 = new PointVM();
            PointVM point2 = new PointVM();
            for(int i = 1;  i < Points.Count; i++) 
            {
                if ((Points[i-1].X <= xValue) && (Points[i].X>xValue))
                {
                    point1 = Points[i - 1];
                    point2 = Points[i];
                    break;
                }
            }
            double result = point1.Y + (xValue- point1.X)*(point2.Y-point1.Y)/(point2.X-point1.X);
            return result;

        }

        private PlotModel UpdatePlotModel()
        {
            var plotModel = new PlotModel();
            plotModel.Series.Add(new FunctionSeries(x => Interpolation(x), 0, 255, 0.1));
            return plotModel;
        }

        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ??= new RelayCommand(t => true, (obj) => 
                {
                    var point = obj as PointVM;
                    if ((point.X == 0) || (point.X == 255))
                        MessageBox.Show("!!!Нельзя удалять краевые точки!!!");
                    else
                        Points.Remove(point);
                });
            }
        }
        public ICommand AddCommand
        {
            get
            {
                return addCommand ??= new RelayCommand(t => true, (obj) =>
                {
                    var pointVM = new PointVM(_newPoint.X, _newPoint.Y);

                    int index = 0;
                    for (int i = 1; i < Points.Count; i++)
                    {
                        if ((Points[i - 1].X < pointVM.X) && (Points[i].X > pointVM.X))
                        {
                            index = i; break;
                        }
                    }
                    if(index == 0)
                    {
                        MessageBox.Show
                        ("Были введены значения выходдящие за диапазон от 0 до 255\n" +
                            "или была введена точка с уже занятой координатой X");
                    }
                    else
                        InsertPointVM(index, pointVM);
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
