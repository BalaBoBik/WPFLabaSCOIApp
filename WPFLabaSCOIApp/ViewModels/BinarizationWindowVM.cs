using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFLabaSCOIApp.ViewModels
{
    internal class BinarizationWindowVM : INotifyPropertyChanged
    {
        public BinarizationWindowVM(BitmapSource image) 
        {
            _origin = GrayGradationTransform(image);

            BinarCriterion = BinarizationCriterion.getCriteriaList().First((x) => x.Name == "Критерий Гаврилова");

        }

        private BitmapSource _origin;
        private BitmapSource _bitmap;
        private BinarizationCriterion _binarCriterion;
        private int? _windowSize;
        private double? _sensitivity;
        private bool _isLocal;
        public BitmapSource Origin { get { return _origin; } }
        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                OnPropertyChanged("Bitmap");
            }
        }
        public int? WindowSize
        {
            get { return _windowSize; }
            set
            {
                _windowSize = value;
                Bitmap = _binarCriterion.SelectedCriterion(Origin, WindowSize, Sensitivity);
                OnPropertyChanged("WindowSize");
            }
        }
        public double? Sensitivity
        {
            get { return _sensitivity; }
            set
            {
                _sensitivity = value;
                Bitmap = _binarCriterion.SelectedCriterion(Origin, WindowSize, Sensitivity);
                OnPropertyChanged("Sensitivity");
            }
        }
        public bool IsLocal
        {
            get { return _isLocal; }
            set
            {
                _isLocal = value;
                OnPropertyChanged("IsLocal");
            }
        }

        public BinarizationCriterion BinarCriterion
        {
            get { return _binarCriterion; }
            set
            {
                _binarCriterion = value;
                UpdateAll();
                OnPropertyChanged("BinarCriterion");
            }
        }

        public void UpdateAll()
        {

            _windowSize = _binarCriterion.WindowSize;
            OnPropertyChanged("WindowSize");
            _sensitivity = _binarCriterion.Sensitivity;
            OnPropertyChanged("Sensitivity");
            IsLocal = _binarCriterion.IsLocal;
            Bitmap = _binarCriterion.SelectedCriterion(Origin,WindowSize,Sensitivity) ;
        }
        public List<BinarizationCriterion> CriteriaList => BinarizationCriterion.getCriteriaList();

        public Visibility isLocal => (IsLocal) ? Visibility.Visible : Visibility.Hidden;

        public BitmapSource GrayGradationTransform(BitmapSource image)
        {
            int w = image.PixelWidth;
            int h = image.PixelHeight;

            int stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)image.PixelHeight * stride];
            image.CopyPixels(pixels, stride, 0);

            byte grayByte;

            for (int j = 0; j < h * 4; j += 4)
                for (int i = 0; i < w * 4; i += 4)
                {
                    grayByte = (byte)(0.0721 * pixels[j * w + i] + 0.7154 * pixels[j * w + i + 1] + 0.2125 * pixels[j * w + i + 1]);
                    pixels[j * w + i] = grayByte;
                    pixels[j * w + i + 1] = grayByte;
                    pixels[j * w + i + 2] = grayByte;
                }

            var result = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, null, pixels, stride);
            return result;
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
