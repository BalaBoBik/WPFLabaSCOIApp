using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WPFLabaSCOIApp.ViewModels
{
    internal class BinarizationCriterion
    {
        private bool _isLocal;

        public delegate BitmapSource Criterion(BitmapSource image, int? windowSize, double? sensitivity);
        public Criterion SelectedCriterion { get; set; }
        public string Name { get; set; }
        public int? WindowSize { get; set; }
        public double? Sensitivity { get; set; }
        public bool IsLocal { get { return _isLocal; } }

        private static List<BinarizationCriterion> _criteriaList;
        public static List<BinarizationCriterion> getCriteriaList()
        {
            return _criteriaList ??= new List<BinarizationCriterion>()
            {
                new BinarizationCriterion()
                {
                    Name = "Критерий Гаврилова",
                    _isLocal = false,
                    SelectedCriterion = (BitmapSource image, int? windowSize, double? sensitivity) =>
                    {
                        return GavrilovCriterion(image, windowSize, sensitivity);
                    }
                },
                new BinarizationCriterion()
                {
                    Name = "Критерий Отсу",
                    _isLocal = false,
                    SelectedCriterion = (BitmapSource image, int ? windowSize, double ? sensitivity) =>
                    {
                        return OtsuCriterion(image, windowSize, sensitivity); // Тут есть ошибка
                    }
                },
                new BinarizationCriterion()
                {
                    Name = "Критерий Ниблека",
                    _isLocal = true,
                    WindowSize =15,
                    Sensitivity = -0.2,
                    SelectedCriterion = (BitmapSource image, int ? windowSize, double ? sensitivity) =>
                    {
                        return NiblackCriterion(image, windowSize, sensitivity);
                    }
                },
                new BinarizationCriterion()
                {
                    Name = "Критерий Сауволы",
                    _isLocal = true,
                    WindowSize =15,
                    Sensitivity = 0.25,
                    SelectedCriterion = (BitmapSource image, int ? windowSize, double ? sensitivity) =>
                    {
                        return SauvolaCriterion(image, windowSize, sensitivity);
                    }
                }
            };
        }
        public static BitmapSource GavrilovCriterion(BitmapSource image, int? windowSize, double? sensitivity) // Используется только картинка
        {
            int w = image.PixelWidth;
            int h = image.PixelHeight;
            double threshold = 0.0;

            int stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)image.PixelHeight * stride];
            image.CopyPixels(pixels, stride, 0);

            for (int j = 0; j < h * 4; j += 4)
                for (int i = 0; i < w * 4; i += 4)
                {
                    threshold += (double)(pixels[j * w + i]) / (w * h);
                }
            MessageBox.Show($"{threshold}");
            for (int j = 0; j < h * 4; j += 4)
                for (int i = 0; i < w * 4; i += 4)
                {
                    if (threshold >= pixels[j * w + i])
                    {
                        pixels[j * w + i] = 0;
                        pixels[j * w + i + 1] = 0;
                        pixels[j * w + i + 2] = 0;
                    }
                    else
                    {
                        pixels[j * w + i] = 255;
                        pixels[j * w + i + 1] = 255;
                        pixels[j * w + i + 2] = 255;
                    }
                }
            var result = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, null, pixels, stride);
            return result;
        }

        public static BitmapSource OtsuCriterion(BitmapSource image,int? windowSize, double? sensitivity) // Используется только картинка
        {
            int w = image.PixelWidth;
            int h = image.PixelHeight;
            double threshold=0;
            int[] histogram = GradationTransformWindowVM.CalculateHistogramData(image);
            double[] N = new double[256]; 
            for(int i= 0; i < histogram.Length; i++) 
            {
                N[i] = (double)(histogram[i]) / (w * h);
            }
            int maxColor = Array.IndexOf(N,N.Max());  // Неправильно считает из-за этого
            MessageBox.Show($"{maxColor},{N.Max()}");
            double mT = 0;
            for (int i = 0; i < maxColor; i++)
                mT += i * N[i];
            double maxSigma = 0;
            for (int t= 0;t<=maxColor;t++)
            {
                double w1=0;
                for (int i= 0; i<t; i++) 
                    w1 += N[i];
                double w2 = 1 - w1;
                double m1 = 0;
                for (int i = 0; i < t; i++)
                    m1 += (i*N[i])/(w1);
                double m2 = (mT - m1 * w1) / w2;
                double sigma = w1 * w2 * (m1 - m2) * (m1 - m2);

                if (sigma > maxSigma) 
                {
                    maxSigma = sigma;
                    threshold = t;
                }
            }
            MessageBox.Show($"{threshold}");

            int stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)image.PixelHeight * stride];
            image.CopyPixels(pixels, stride, 0);
            for (int j = 0; j < h * 4; j += 4)
                for (int i = 0; i < w * 4; i += 4)
                {
                    if (threshold >= pixels[j * w + i])
                    {
                        pixels[j * w + i] = 0;
                        pixels[j * w + i + 1] = 0;
                        pixels[j * w + i + 2] = 0;
                    }
                    else
                    {
                        pixels[j * w + i] = 255;
                        pixels[j * w + i + 1] = 255;
                        pixels[j * w + i + 2] = 255;
                    }
                }
            var result = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, null, pixels, stride);
            return result;

        }
        public static BitmapSource NiblackCriterion(BitmapSource image,int? windowSize, double? sensitivity)
        {
            if ((windowSize != null) && (sensitivity != null))
            {
                int w = image.PixelWidth;
                int h = image.PixelHeight;

                int stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
                byte[] pixels = new byte[(int)image.PixelHeight * stride];
                image.CopyPixels(pixels, stride, 0);

                stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
                byte[] resultPixels = new byte[(int)image.PixelHeight * stride];
                image.CopyPixels(resultPixels, stride, 0);

                List<double> values = new List<double>();
                double sigma;
                double t;
                for (int j = 0; j < h * 4; j += 4)
                {
                    for (int i = 0; i < w * 4; i += 4)
                    {
                        values.Clear();
                        sigma= 0;
                        t= 0;
                        for (int y = (int)(j - 4*(windowSize / 2)); y <= (int)(j + 4*(windowSize / 2)); y+=4)
                            for (int x = (int)(i - 4*(windowSize / 2)); x <= (int)(i + 4*(windowSize / 2)); x+=4)
                            {
                                if ((0 <= y) && (y < h*4))
                                    if ((0 <= x) && (x < w*4))
                                    {
                                        values.Add(pixels[y * w + x]);
                                    }
                            }
                        sigma = Math.Sqrt(D(values));
                        t = M(values) + (double)sensitivity * sigma;

                        if (t >= resultPixels[j * w + i])
                        {
                            resultPixels[j * w + i] = 0;
                            resultPixels[j * w + i + 1] = 0;
                            resultPixels[j * w + i + 2] = 0;
                        }
                        else
                        {
                            resultPixels[j * w + i] = 255;
                            resultPixels[j * w + i + 1] = 255;
                            resultPixels[j * w + i + 2] = 255;
                        }
                    }
                }
                var result = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, null, resultPixels, stride);
                return result;
            }
            else throw new Exception("Размер окна или чувствительность не заданы");
        }
        public static BitmapSource SauvolaCriterion(BitmapSource image, int? windowSize, double? sensitivity)
        {
            if ((windowSize != null) && (sensitivity != null))
            {
                int w = image.PixelWidth;
                int h = image.PixelHeight;

                int stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
                byte[] pixels = new byte[(int)image.PixelHeight * stride];
                image.CopyPixels(pixels, stride, 0);

                stride = (int)image.PixelWidth * (image.Format.BitsPerPixel / 8);
                byte[] resultPixels = new byte[(int)image.PixelHeight * stride];
                image.CopyPixels(resultPixels, stride, 0);

                List<double> values = new List<double>();
                double sigma;
                double t;
                for (int j = 0; j < h * 4; j += 4)
                {
                    for (int i = 0; i < w * 4; i += 4)
                    {
                        values.Clear();
                        sigma = 0;
                        t = 0;
                        for (int y = (int)(j - 4 * (windowSize / 2)); y <= (int)(j + 4 * (windowSize / 2)); y += 4)
                            for (int x = (int)(i - 4 * (windowSize / 2)); x <= (int)(i + 4 * (windowSize / 2)); x += 4)
                            {
                                if ((0 <= y) && (y < h * 4))
                                    if ((0 <= x) && (x < w * 4))
                                    {
                                        values.Add(pixels[y * w + x]);
                                    }
                            }
                        sigma = Math.Sqrt(D(values));
                        t = (double)(M(values)*(1+sensitivity*(sigma/128-1)));

                        if (t >= resultPixels[j * w + i])
                        {
                            resultPixels[j * w + i] = 0;
                            resultPixels[j * w + i + 1] = 0;
                            resultPixels[j * w + i + 2] = 0;
                        }
                        else
                        {
                            resultPixels[j * w + i] = 255;
                            resultPixels[j * w + i + 1] = 255;
                            resultPixels[j * w + i + 2] = 255;
                        }
                    }
                }
                var result = BitmapSource.Create(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, null, resultPixels, stride);
                return result;
            }
            else throw new Exception("Размер окна или чувствительность не заданы");
        }
        public static BitmapSource BradleyRothCriterion(BitmapSource image, int? windowSize, double? sensitivity)
        {
            MessageBox.Show($"Bradley");
            return image;
        }
        public static BitmapSource WolfCriterion(BitmapSource image, int? windowSize, double? sensitivity)
        {
            MessageBox.Show($"Wolf");
            return image;
        }
        public static double M (List<double> values)
        {
            double result=0;
            for (int i = 0;i < values.Count; i++) 
            {
                result += values[i] / values.Count;
            }
            return result;
        }
        public static double D(List<double> values)
        {
            double result;
            List<double> squareValues = new List<double>();
            for (int i = 0; i < values.Count; i++) 
            {
                squareValues.Add(values[i] * values[i]);
            }
            result = M(squareValues)-Math.Pow(M(values), 2);
            return result;
        }
    }
}
