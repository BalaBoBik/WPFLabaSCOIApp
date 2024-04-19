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
            OverlayOperation = ByteOperation.getOperationsList().First((x) => x.Name=="Обычное");
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
        private ByteOperation _overlayOperation;
       
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
        public ByteOperation OverlayOperation
        {
            get { return _overlayOperation; }
            set
            {
                _overlayOperation = value;
                OnPropertyChanged("OverlayOperation");
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

        public BitmapSource OverlayOnBitmap(BitmapSource origin)
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
                    result.CopyPixels(new Int32Rect(i+OffsetX, j+OffsetY, 1, 1), pixel, 4, 0); // копируем 1 пиксель в байтах
                    writeableBitmap.CopyPixels(new Int32Rect(i, j, 1, 1), newPixel, 4, 0);

                    pixel[0] = OverlayOperation.SelectedOperation(pixel[0], newPixel[0], Opacity, B);// Blue

                    pixel[1] = OverlayOperation.SelectedOperation(pixel[1], newPixel[1], Opacity, G); // Green

                    pixel[2] = OverlayOperation.SelectedOperation(pixel[2], newPixel[2], Opacity, R); // Red

                    //pixel[3] = newPixel[3]; // Alpha

                    result.WritePixels(new Int32Rect(i+OffsetX, j+OffsetY, 1, 1), pixel, 4, 0);
                }
            return result;
        }
    }
}
