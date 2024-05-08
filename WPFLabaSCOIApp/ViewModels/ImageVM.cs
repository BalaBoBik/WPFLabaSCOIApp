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
using System.Windows.Media.Media3D;

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

            var w = Bitmap.PixelWidth;
            var h = Bitmap.PixelHeight;
            //Ориг в байты
            int stride = (int)origin.PixelWidth * (origin.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)origin.PixelHeight * stride];

            origin.CopyPixels(pixels, stride, 0);
            //Накладываемое изображение в байты
            int newStride = (int)Bitmap.PixelWidth * (Bitmap.Format.BitsPerPixel / 8);
            byte[] newPixels = new byte[(int)Bitmap.PixelHeight * newStride];

            Bitmap.CopyPixels(newPixels, newStride, 0);

            int pixelIndex = 0;
            int newPixelIndex = 0;
            for (int j = 0; j < h * 4; j += 4)
            {
                for (int i = 0; i < w * 4; i += 4)
                {
                    try
                    {
                        pixels[pixelIndex] = OverlayOperation.SelectedOperation(pixels[pixelIndex], newPixels[newPixelIndex], Opacity, B);
                        pixels[pixelIndex + 1] = OverlayOperation.SelectedOperation(pixels[pixelIndex + 1], newPixels[newPixelIndex + 1], Opacity, G);
                        pixels[pixelIndex + 2] = OverlayOperation.SelectedOperation(pixels[pixelIndex + 2], newPixels[newPixelIndex + 2], Opacity, R);
                    }
                    catch 
                    {

                        MessageBox.Show($"{pixelIndex}\n{i} ; {j}");
                    }
                    pixelIndex += 4;
                    newPixelIndex += 4;
                }
                pixelIndex = j * origin.PixelWidth;
            }
            var result = BitmapSource.Create(origin.PixelWidth, origin.PixelHeight,origin.DpiX, origin.DpiY, origin.Format, null,pixels, stride);
            return result;
        }
    }
}
