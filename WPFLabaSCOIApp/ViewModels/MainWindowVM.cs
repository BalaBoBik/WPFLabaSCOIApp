using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFLabaSCOIApp.Models;

namespace WPFLabaSCOIApp.ViewModels
{
    public class MainWindowVM
    {
        public ObservableCollection<ImageVM> Images { get; set; } = new ObservableCollection<ImageVM>();

        private ICommand moveUpCommand;
        private ICommand moveDownCommand;
        private ICommand deleteCommand;
        public void AddImageVM(ImageVM imageVM)
        {
            Images.Add(imageVM);
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

    }
}
