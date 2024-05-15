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
    /// Interaction logic for LinearFiltratingWindow.xaml
    /// </summary>
    public partial class LinearFiltratingWindow : Window
    {
        private LinearFiltratingWindowVM vm;
        public LinearFiltratingWindow(BitmapSource image)
        {
            InitializeComponent();
            vm = new LinearFiltratingWindowVM(image);
            DataContext = vm;
        }
    }
}
