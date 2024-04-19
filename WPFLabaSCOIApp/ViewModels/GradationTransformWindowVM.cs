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
namespace WPFLabaSCOIApp.ViewModels
{
    internal class GradationTransformWindowVM : INotifyPropertyChanged
    {
        GradationTransformWindowVM()
        {
            Points.Add(new Point(0, 0));
            Points.Add(new Point(255, 255));
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
        public double CubicSplineInterpolation(double xValue)
        {

            for (int i = 0; i < Points.Count - 1; i++)
            {
                if (Points[i].X <= xValue && xValue <= Points[i + 1].X)
                {
                    double h = Points[i + 1].X - Points[i].X;
                    double t = (xValue - Points[i].X) / h;

                    double a = Points[i].Y;
                    double b = (Points[i + 1].Y - Points[i].Y) / h - h * (2 * Points[i].Y + Points[i + 1].Y) / 6;
                    double c = (Points[i + 1].Y - Points[i].Y) / (2 * h);
                    double d = (Points[i + 1].Y - Points[i].Y) / (6 * h);

                    return a + b * t + c * t * t + d * t * t * t;
                }
            }

            throw new ArgumentException("xValue is out of range of the given Points");
        }
    }
}
