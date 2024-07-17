using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace WPFLabaSCOIApp.ViewModels
{
    internal class FoireImageWindowVM : INotifyPropertyChanged
    {
        public FoireImageWindowVM(BitmapSource image)
        {
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            Bitmap = GetFourierImage(image);
            startTime.Stop();
            var resultTime = startTime.Elapsed;

            // elapsedTime - строка, которая будет содержать значение затраченного времени
            ElapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                resultTime.Hours,
                resultTime.Minutes,
                resultTime.Seconds,
                resultTime.Milliseconds);
        }
        private BitmapSource _bitmap;
        private string _elapsedTime;
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                _elapsedTime = value;
                OnPropertyChanged("ElapsedTime");
            }
        }
        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                OnPropertyChanged("Bitmap");
            }
        }
        public BitmapSource GetFourierImage(BitmapSource image) 
        {
            int w = image.PixelWidth;
            int h = image.PixelHeight;

            int stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)image.PixelHeight * stride];
            image.CopyPixels(pixels, stride, 0);

            Complex[][] G = ConvertRGBABytesToComplexMatrix(pixels,w,h);
            G = DiscreteFoirerTransform2D(G);
            pixels=ConvertComplexMatrixToRGBABytes(G);

            var result = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, null, pixels, stride);
            return result;
        }
        //DFT_1D
        public Complex[] DiscreteFoirerTransform1D(Complex[] x)
        {
            var N = x.Length;
            Complex[] G = new Complex[N];
            for (int u = 0; u < N; u++)
            {
                for (int k = 0; k < N; k++)
                {
                    double TwoPIuk = (-2 * Math.PI * u * k) / N;    
                    G[u] += (new Complex((Math.Cos(TwoPIuk)), Math.Sin(TwoPIuk))*x[k]);
                }
                G[u] /= N;
            }
            return G;

        }
        //DFT_2D
        public Complex[][] DiscreteFoirerTransform2D(Complex[][] X)
        {
            int N = X.Length;     //Строки
            int M = X[0].Length;     //Столбцы

            Complex[][] G = new Complex[N][];
            Complex[][] Tmp = new Complex[N][];
            Complex[][] ColumnTmp = new Complex[M][];
            Complex[][] VerticalG = new Complex[M][];   
            for(int i = 0; i < N; i++)
            {
                G[i] = new Complex[M];
            }
            for (int i = 0; i < M; i++)
            {
                ColumnTmp[i] = new Complex[N];
                VerticalG[i] = new Complex[N];
            }


            for (int n=0; n<N; n++)
                for(int m =0; m<M; m++)
                {
                    X[n][m] *= Math.Pow(-1, n + m);
                }

            for(int i=0; i<N; i++)
            {
                Tmp[i] = DiscreteFoirerTransform1D(X[i]);
            }
            var x = Tmp;
            for (int n = 0; n < N; n ++)
                for (int m = 0; m < M; m ++)
                {
                    ColumnTmp[m][n] = Tmp[n][m];
                }

            for (int i = 0; i < M; i++)
            {
                VerticalG[i]= DiscreteFoirerTransform1D(ColumnTmp[i]);
            }
            for (int n = 0; n < N; n ++)
                for (int m = 0; m < M; m ++)
                {
                    G[n][m] = VerticalG[m][n];
                }
            return G;
        }

        public Complex[][] ConvertRGBABytesToComplexMatrix(byte[] pixels,int width,int height)  // Плохой код
        {
            Complex[][] Matrix = new Complex[height][];
            for(int i=0; i<height; i++)
            {
                Matrix[i] = new Complex[width * 3];
            }
            for(int i=0; i<height*4;i+=4)
                for(int j=0 ; j<width*4;j+=4) 
                {
                    Matrix[i / 4][j / 4 + 0] = new Complex(pixels[i * width + j + 0], 0);

                    Matrix[i / 4][j / 4 + 1] = new Complex(pixels[i * width + j + 1], 0);

                    Matrix[i / 4][j / 4 + 2] = new Complex(pixels[i * width + j + 2], 0);
                }
            return Matrix;
        }
        public byte[] ConvertComplexMatrixToRGBABytes(Complex[][] Matrix)    // Плохой код
        {
            int height= Matrix.Length;
            int width= Matrix[0].Length;
            byte[] pixels = new byte[height*width/3*4];
            int pixelIndex = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j += 3)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        pixels[pixelIndex + k] = (byte)(125*Matrix[i][j + k].Magnitude);
                    }
                    pixels[pixelIndex + 3] = 255;
                    pixelIndex += 4;
                }
            }
            return pixels;
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
