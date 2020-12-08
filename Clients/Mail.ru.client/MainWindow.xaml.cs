using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mail.ru.client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _login;
        private string _password;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Login { get { return _login; } set { _login = value; OnPropertyChanged(); } }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged(); } }
        public bool AuthButtonEnabled
        {
            get
            {
                if (Login == "" || Password == "") return false;
                return true;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
