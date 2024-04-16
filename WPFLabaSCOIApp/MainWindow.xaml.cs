using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFLabaSCOIApp.ViewModels;


namespace WPFLabaSCOIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVM vm;
        public MainWindow()
        {
            InitializeComponent();

            vm = new MainWindowVM();
            DataContext = vm;

            this.AllowDrop = true;
            this.Drop += (s, a) =>
            {
                List<string> failed_images = new List<string>();

                string[] files = (string[])a.Data.GetData(DataFormats.FileDrop);
                foreach (var f in files)
                {
                    FileInfo fi = new FileInfo(f);
                    try
                    {
                        vm.AddImageVM((new ImageVM { Bitmap = new BitmapImage(new Uri(f)), Name = fi.Name }));
                    }
                    catch (Exception e)
                    {
                        failed_images.Add(e.Message);
                    }

                }

                if (failed_images.Count != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var str in failed_images)
                    {
                        sb.AppendLine(str);
                    }

                    MessageBox.Show(sb.ToString(), "Error in images", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

        }
    }
}