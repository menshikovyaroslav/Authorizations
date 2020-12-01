using MailRu.Client.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailRu.Client.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<Directory> _directories;

        public ObservableCollection<Directory> Directories
        {
            get { return _directories; }
            set { _directories = value; }
        }

        public MainViewModel()
        {
            Directories = new ObservableCollection<Directory>();
            Directories.Add(new Directory() { Name="Входящие" });
            Directories.Add(new Directory() { Name = "Исходящие" });
            Directories.Add(new Directory() { Name = "Удаленные" });
        }

    }
}
