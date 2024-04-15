using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WPFLabaSCOIApp.ViewModels
{
    public class ImageVM
    {
        private string _name;
        private BitmapSource _bitmap;
        private double _opacity = 1;
        private int _offsetX;
        private int _offsetY;
        private bool _r = true;
        private bool _g = true;
        private bool _b = true;
        private string _sizeString = "";
        delegate BitmapSource Operetion(BitmapSource origin);

        public bool R
        {
            get { return _r; }
            set 
            { 
                _r = value;
                OnPropertyChanged("R");
            }
        }
        public bool G
        {
            get { return _g; }
            set
            {
                _g = value;
                OnPropertyChanged("G");
            }
        }
        public bool B
        {
            get { return _b; }
            set
            {
                _b = value;
                OnPropertyChanged("B");
            }
        }
        public int OffsetX
        {
            get { return _offsetX; }
            set
            {
                _offsetX = value;
                OnPropertyChanged("OffsetX");
            }
        }
        public int OffsetY
        {
            get { return _offsetY; }
            set
            {
                _offsetY = value;
                OnPropertyChanged("OffsetY");
            }
        }
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                OnPropertyChanged("Opacity");
            }
        }
        public BitmapSource Bitmap
        {
            get { return (BitmapSource)_bitmap; }
            set
            {
                _bitmap = value;
                OnPropertyChanged("Bitmap");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        private BitmapSource Normal(BitmapSource origin)
        {
            WriteableBitmap result= new WriteableBitmap(origin);
            WriteableBitmap writeableBitmap = new WriteableBitmap(Bitmap);
            var w = _bitmap.PixelWidth;
            var h=_bitmap.PixelHeight;

            byte[] pixel = new byte[4]; // RGBA
            byte[] newPixel = new byte[4];

            for (int i=0; i < w;i++)
                for(int j=0; j < h;j++)
                {
                    result.CopyPixels(new Int32Rect(i, j, 1, 1), pixel, 4, 0); // копируем 1 пиксель в байтах
                    writeableBitmap.CopyPixels(new Int32Rect(i, j, 1, 1), newPixel, 4, 0);

                    pixel[0] =  newPixel[0]; // Blue
                    pixel[1] = newPixel[1]; // Green
                    pixel[2] = newPixel[2]; // Red
                    pixel[3] = newPixel[3]; // Alpha

                    result.WritePixels(new Int32Rect(i, j, 1, 1), pixel, 4, 0);
                }
            return result;
        }
    }
}
