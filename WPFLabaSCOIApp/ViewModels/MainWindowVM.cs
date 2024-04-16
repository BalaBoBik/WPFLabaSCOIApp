using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using WPFLabaSCOIApp.Models;

namespace WPFLabaSCOIApp.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {

        private BitmapSource _finalImage;
        private ICommand moveUpCommand;
        private ICommand moveDownCommand;
        private ICommand deleteCommand;
        public ObservableCollection<ImageVM> Images { get; set; } = new ObservableCollection<ImageVM>();
        

        public BitmapSource FinalImage
        {
            get { return _finalImage; }
            set 
            { 
                _finalImage = value;
                OnPropertyChanged(nameof(FinalImage));
            }
        }
        public MainWindowVM()
        {
            Images.CollectionChanged += Images_CollectionChanged;
        }

        public void AddImageVM(ImageVM imageVM)
        {
            Images.Add(imageVM);
            imageVM.PropertyChanged += (s, prop_name) =>
            {
                if (prop_name.PropertyName == nameof(ImageVM.Opacity)
                    || prop_name.PropertyName == nameof(ImageVM.OpacityInPercent)
                    || prop_name.PropertyName == nameof(ImageVM.SelectedOperation)
                    || prop_name.PropertyName == nameof(ImageVM.OffsetX)
                    || prop_name.PropertyName == nameof(ImageVM.OffsetY)
                    || prop_name.PropertyName == nameof(ImageVM.R)
                    || prop_name.PropertyName == nameof(ImageVM.G)
                    || prop_name.PropertyName == nameof(ImageVM.B))
                {
                    this.UpdateImage();
                }
            };
        }
        void Images_CollectionChanged (object? sender,NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems?[0] is ImageVM newImage )
                    {
                        if (FinalImage == null)
                            FinalImage = newImage.Bitmap;
                        else
                            FinalImage = newImage.Normal(FinalImage);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count <1)
                        FinalImage = null;
                    else
                        UpdateImage();
                    break;
                case NotifyCollectionChangedAction.Reset:

                    FinalImage = null;
                    break;
                default:
                    UpdateImage();
                    break;
            }
        }
        private BitmapSource CalculateLayers()
        {
            int w = Images.Max(x => x.Bitmap.PixelWidth + x.OffsetX);
            int h = Images.Max(x => x.Bitmap.PixelHeight + x.OffsetY);
            BitmapSource result = ImageVM.CreateEmptyBitmap(w, h);
            for (int i=0;i<Images.Count;i++)
            {
                result = Images[i].SelectedOperation(result);
            }
            return result;
        }
        public void UpdateImage()
        {
            FinalImage = CalculateLayers();
        }
        
        public ICommand MoveUpCommand
        {
            get
            {
                return moveUpCommand ??= new RelayCommand((obj) => Images.FirstOrDefault() != (obj as ImageVM), (obj) =>
                {
                    var param = obj as ImageVM;
                    var ii = Images.IndexOf(param);
                    Images.Move(ii, ii - 1);
                });
            }
        }

        public ICommand MoveDownCommand
        {
            get
            {
                return moveDownCommand ??= new RelayCommand((obj) => Images.LastOrDefault() != (obj as ImageVM), (obj) =>
                {
                    var param = obj as ImageVM;
                    var ii = Images.IndexOf(param);
                    Images.Move(ii, ii + 1);
                });
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return deleteCommand ??= new RelayCommand(t => true, (obj) => Images.Remove(obj as ImageVM));
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
