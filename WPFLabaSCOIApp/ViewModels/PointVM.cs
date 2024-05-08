using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFLabaSCOIApp.ViewModels
{
    internal class PointVM
    {
        public PointVM()
        {
            _point = new Point();
        }
        public PointVM(Point point) 
        {
            _point = point;
        }
        public PointVM(int x, int y)
        {
            _point = new Point(x, y);
        }
        private Point _point;

        public int X 
        {
            get { return _point.X; }
            set
            {
                if ((_point.X != 0) && (_point.X != 255))
                {
                    _point.X = value;
                    OnPropertyChanged("X");
                }
                else
                { System.Windows.MessageBox.Show("Нельзя менять значение X у краевых точек"); }
            }
        }

        public int Y 
        {
            get { return _point.Y; }
            set
            {
                _point.Y = value;
                OnPropertyChanged("Y");
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
