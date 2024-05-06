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
namespace WPFLabaSCOIApp.ViewModels
{
    internal class GradationTransformWindowVM : INotifyPropertyChanged
    {
        public GradationTransformWindowVM(BitmapSource image)
        {
            Points.Add(new Point(-1, -1));
            Points.Add(new Point(0, 0));
            Points.Add(new Point(30, 60));
            Points.Add(new Point(255, 255));
            Points.Add(new Point(256, 256));
            Points.OrderBy(x => x.X);
            Bitmap = image;
            GraphModel = CreatePlotModel();
        }
        private BitmapSource _bitmap;
        public PlotModel GraphModel { get; set; }
        public BitmapSource Bitmap 
        {
            get { return _bitmap; }
            set { 
                _bitmap = value;
                OnPropertyChanged("Bitmap");
            }
        }
        public ObservableCollection<Point> Points { get; set; } = new ObservableCollection<Point>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public double Interpolation(double xValue)
        {
            Point point = Points.FirstOrDefault(x => x.X >= xValue);
            int index = Points.IndexOf(point);
            return CubicInterpolate(Points[index - 2], Points[index-1], Points[index], Points[index + 1], xValue);
            
        }
        public static double CubicInterpolate(Point point0, Point point1, Point point2, Point point3, double x)
        {
            double a0, a1, a2, a3, y;

            a0 = point3.Y / ((point3.X - point0.X) * (point3.X - point1.X) * (point3.X - point2.X));
            a1 = point2.Y / ((point2.X - point0.X) * (point2.X - point1.X) * (point2.X - point3.X));
            a2 = point1.Y / ((point1.X - point0.X) * (point1.X - point2.X) * (point1.X - point3.X));
            a3 = point0.Y / ((point0.X - point1.X) * (point0.X - point2.X) * (point0.X - point3.X));

            y = a0 * x * x * x + a1 * x * x + a2 * x + a3;

            return y;
        }

        private PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel();
            plotModel.Series.Add(new FunctionSeries(x => Interpolation(x), 1, 254, 0.1));
            return plotModel;
        }
    }
}
