using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFLabaSCOIApp.ViewModels
{
    public class ImageVM : INotifyPropertyChanged
    {
        public ImageVM()
        {
            SelectedOperation = Normal;
        }
        private string _name;
        private BitmapSource _bitmap;
        private double _opacity = 1;
        private int _offsetX;
        private int _offsetY;
        private bool _r = true;
        private bool _g = true;
        private bool _b = true;
        private string _sizeString = "";
        private Operation _selectedOperation { get; set; }
        public delegate BitmapSource Operation(BitmapSource origin);
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
                _opacity = Math.Round(value,2);
                OnPropertyChanged("Opacity");
            }
        }
        public int OpacityInPercent
        {
            get { return (int)(_opacity*100); }
            set
            {
                _opacity = Math.Round((double)value / 100,2);
                OnPropertyChanged("OpacityInPercent");
            }
        }
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Opacity = e.NewValue;
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
        public Operation SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                _selectedOperation = value;
                OnPropertyChanged("SelectedOperation");
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

        static public BitmapSource CreateEmptyBitmap(int width, int height)
        {
            PixelFormat pf = PixelFormats.Bgr32;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * height];
            for (int i = 0; i < rawImage.Length; i++)
            { rawImage[i] = 255; }

            // Create a BitmapSource.
            return BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);

        }

        public BitmapSource OverlayOnBitmap(BitmapSource? origin)
        {
            if (origin != null)
            {
                WriteableBitmap result = new WriteableBitmap(origin);
                WriteableBitmap writeableBitmap = new WriteableBitmap(Bitmap);
                var w = Bitmap.PixelWidth;
                var h = Bitmap.PixelHeight;

                byte[] pixel = new byte[4]; // RGBA
                byte[] newPixel = new byte[4];

                for (int i = 0; i < w; i++)
                    for (int j = 0; j < h; j++)
                    {
                        result.CopyPixels(new Int32Rect(i, j, 1, 1), pixel, 4, 0); // копируем 1 пиксель в байтах
                        writeableBitmap.CopyPixels(new Int32Rect(i, j, 1, 1), newPixel, 4, 0);

                        if (B)
                            pixel[0] = (byte)(pixel[0] * (1 - Opacity) + newPixel[0] * Opacity); // Blue
                        else
                            pixel[0] = (byte)(pixel[0]);

                        if (G)
                            pixel[1] = (byte)(pixel[1] * (1 - Opacity) + newPixel[1] * Opacity); // Green
                        else
                            pixel[1] = (byte)(pixel[1]);

                        if (R)
                            pixel[2] = (byte)(pixel[2] * (1 - Opacity) + newPixel[2] * Opacity); // Red
                        else
                            pixel[2] = (byte)(pixel[2]);

                        //pixel[3] = newPixel[3]; // Alpha

                        result.WritePixels(new Int32Rect(i, j, 1, 1), pixel, 4, 0);
                    }
                return result;
            }
            else
            {
                WriteableBitmap result
            }
        }
    }
}
