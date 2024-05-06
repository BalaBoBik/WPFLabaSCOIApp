using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFLabaSCOIApp.ViewModels;

namespace WPFLabaSCOIApp.Windows
{
    /// <summary>
    /// Interaction logic for GradationTransformWindow.xaml
    /// </summary>
    public partial class GradationTransformWindow : Window
    {
        private GradationTransformWindowVM vm;
        public GradationTransformWindow(BitmapSource image)
        {
            InitializeComponent();
            vm = new GradationTransformWindowVM(image);
            DataContext = vm;
        }
        public PlotModel GraphModel { get; set; }
    }
}
