using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Input;
using WPFLabaSCOIApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WPFLabaSCOIApp.ViewModels
{
    internal class LinearFiltratingWindowVM : INotifyPropertyChanged
    {
        public LinearFiltratingWindowVM(BitmapSource image)
        {
            _origin = image;
            _bitmap = Origin;
            _matrixWidth = 3;
            _matrixHeight = 3;
            _sigma = 1;
            _matrix = SimpleDistribution(MatrixWidth,MatrixHeight);
        }
        private BitmapSource _origin;
        private BitmapSource _bitmap;
        private int _matrixWidth;
        private int _matrixHeight;
        private double _sigma;
        private DoubleValue[] _matrix;
        private ICommand gaussDistributionCommand;
        private ICommand applyMaskCommand;
        private ICommand simpleDistributionCommand;
        private ICommand applyMedianFilteringCommand;

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
        public int MatrixWidth
        {
            get { return _matrixWidth; }
            set
            {
                if (value % 2 != 0)
                {
                    _matrixWidth = value;
                    Matrix = SimpleDistribution(MatrixWidth, MatrixHeight);
                    OnPropertyChanged("MatrixWidth");
                }
                else MessageBox.Show("!!!Ширина матрицы должна быть нечетной!!!");
            }
        }
        public int MatrixHeight
        {
            get { return _matrixHeight; }
            set
            {
                if (value % 2 != 0)
                {
                    _matrixHeight = value;
                    Matrix = SimpleDistribution(MatrixWidth, MatrixHeight);
                    OnPropertyChanged("MatrixHeight");
                }
                else MessageBox.Show("!!!Высота матрицы должна быть нечетной!!!");
            }
        }
        public double Sigma
        {
            get { return _sigma; }
            set
            {
                _sigma = value;
                OnPropertyChanged("Sigma");
            }
        }
        public DoubleValue[] Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                OnPropertyChanged("Matrix");
            }
        }

        public byte Median (List<byte> bytes)
        {
            bytes.Sort();
            byte median;
            if (bytes.Count % 2 == 0)
            {
                median = (byte)(Math.Round((bytes[bytes.Count / 2 - 1] + bytes[bytes.Count / 2]) / 2.0, 0));
            }
            else
            {
                median = bytes[bytes.Count / 2];
            }

            return median;
        }
        public BitmapSource ApplyMedianFiltering(BitmapSource image)
        {
            int w = image.PixelWidth;
            int h = image.PixelHeight;

            int stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)image.PixelHeight * stride];
            image.CopyPixels(pixels, stride, 0);

            stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] newPixels = new byte[(int)image.PixelHeight * stride];
            List<byte> R= new List<byte>();
            List<byte> G = new List<byte>();
            List<byte> B = new List<byte>();
            int yy;
            int xx;
            for (int j = 0; j < h * 4; j += 4)
                for (int i = 0; i < w * 4; i += 4)
                {
                    B = new List<byte>();
                    G = new List<byte>();
                    R = new List<byte>();
                    for (int y = 0; y < MatrixHeight; y++)
                    {
                        yy = j + 4 * (y - (MatrixHeight / 2));
                        if (yy < 0)
                            yy = Math.Abs(yy);
                        else if (yy >= 4 * h)
                            yy = j - 4 * (y - (MatrixHeight / 2));

                        for (int x = 0; x < MatrixWidth; x++)
                        {
                            xx = i + 4 * (x - (MatrixWidth / 2));
                            if (xx < 0)
                                xx = Math.Abs(xx);
                            else if (xx >= 4 * w)
                                xx = i - 4 * (x - (MatrixWidth / 2));

                            B.Add(pixels[yy * w + xx + 0]);
                            G.Add(pixels[yy * w + xx + 1]);
                            R.Add(pixels[yy * w + xx + 2]);
                        }
                    }
                    newPixels[j * w + i + 0] = Median(B);
                    newPixels[j * w + i + 1] = Median(G);
                    newPixels[j * w + i + 2] = Median(R);

                }
            var result = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, null, newPixels, stride);
            return result;
        }
        public BitmapSource ApplyMask(BitmapSource image)
        {
            int w = image.PixelWidth;
            int h = image.PixelHeight;

            int stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)image.PixelHeight * stride];
            image.CopyPixels(pixels, stride, 0);

            stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] newPixels = new byte[(int)image.PixelHeight * stride];
            double[] newBytes = new double [3];
            int yy;
            int xx;

            for (int j = 0; j < h * 4; j += 4)
                for (int i = 0; i < w * 4; i += 4)
                {
                    newBytes = new double[] { 0.0, 0.0, 0.0 };
                    for (int y = 0; y < MatrixHeight; y++)
                    {
                        yy = j + 4 * (y - (MatrixHeight / 2));
                        if (yy < 0)
                            yy = Math.Abs(yy);
                        else if (yy >= 4 * h)
                            yy = j - 4 * (y - (MatrixHeight / 2));
                        for (int x = 0; x < MatrixWidth; x++)
                        {
                            xx = i + 4 * (x - (MatrixWidth / 2));
                            if (xx < 0)
                                xx = Math.Abs(xx);
                            else if (xx >= 4 * w)
                                xx = i - 4 * (x - (MatrixWidth / 2));
                            for (int k = 0; k < 3; k++)
                            {
                                int a = pixels[yy * w + xx + k];
                                double b = Matrix[y * MatrixWidth + x].Value;
                                newBytes[k] += (b * a);
                            }
                        }
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        newPixels[j * w + i + k] = (byte)Math.Round(newBytes[k], 0);
                    }
                }
            var result = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, null, newPixels, stride);
            return result;
        }


        public static DoubleValue[] SimpleDistribution(int w, int h)
        {
            DoubleValue[] result = new DoubleValue[w * h];

            double g;
            g = 1.0 / (w * h);
            g = Math.Round(g, 5);
            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    result[i * w + j] = new DoubleValue(g);
                }
            }
            return result;
        }
        public static DoubleValue[] GaussDistribution(int w, int h, double sigma)
        {
            DoubleValue[] result = new DoubleValue[w * h];
            double s = 0;
            double g;

            for (int i = 0; i < h; ++i)
            {
                for (int j = 0; j < w; ++j)
                {
                    g = 1.0 / (Math.PI * 2.0 * sigma * sigma) * Math.Exp(-1.0 * ((-h/2+i) * (-h / 2 + i) + (-w / 2 + j) * (-w / 2 + j)) / (2 * sigma * sigma));
                    g= Math.Round(g,5);
                    s += g;
                    result[i*w+j] = new DoubleValue(g);
                }
            }
            MessageBox.Show($"{s}");
            return result;
        }

        public ICommand GaussDistributionCommand
        {
            get
            {
                return gaussDistributionCommand ??= new RelayCommand(t => true, (obj) => { Matrix=GaussDistribution(MatrixWidth, MatrixHeight, Sigma); });
            }
        }
        public ICommand SimpleDistributionCommand
        {
            get
            {
                return simpleDistributionCommand ??= new RelayCommand(t => true, (obj) => { Matrix = SimpleDistribution(MatrixWidth, MatrixHeight); });
            }
        }
        public ICommand ApplyMaskCommand
        {
            get
            {
                return applyMaskCommand ??= new RelayCommand(t => true, (obj) => { Bitmap = ApplyMask(Origin); });
            }
        }
        public ICommand ApplyMedianFilteringCommand
        {
            get
            {
                return applyMedianFilteringCommand ??= new RelayCommand(t => true, (obj) => { Bitmap = ApplyMedianFiltering(Origin); });
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
    public class DoubleValue : INotifyPropertyChanged
    {
        public DoubleValue() { Value = 0.0; }
        public DoubleValue(double value) { Value = value; }
        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
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
