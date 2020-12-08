using MailRu.Client.Commands;
using MailRu.Client.Model;
using MailRu.Client.Services;
using MailRu.Client.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MailRu.Client.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand ShowOptionsCommand { get; set; }
        public ICommand ExitProgramCommand { get; set; }

        public void ShowOptionsCommand_Execute()
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.Show();


        }

        public bool ShowOptionsCommand_CanExecute()
        {
            return true;
        }

        public void ExitProgramCommand_Execute()
        {
            Application.Current.Shutdown();
        }

        public bool ExitProgramCommand_CanExecute()
        {
            return true;
        }

        private ObservableCollection<Directory> _directories;
        private ObservableCollection<Message> _messages;
        private Auth _auth;

        public ObservableCollection<Directory> Directories
        {
            get { return _directories; }
            set { _directories = value; }
        }

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public Auth Auth
        {
            get { return _auth; }
            set { _auth = value; }
        }

        IMailService _mailService;
        IOptionsService _optionsService;

        public MainViewModel(IMailService mailService, IOptionsService optionsService)
        {
            ShowOptionsCommand = new Command(ShowOptionsCommand_Execute, ShowOptionsCommand_CanExecute);
            ExitProgramCommand = new Command(ExitProgramCommand_Execute, ExitProgramCommand_CanExecute);

            _mailService = mailService;
            _optionsService = optionsService;

            Directories = new ObservableCollection<Directory>();
            Directories.Add(new Directory() { Name="Входящие" });
            Directories.Add(new Directory() { Name = "Исходящие" });
            Directories.Add(new Directory() { Name = "Удаленные" });

            Messages = new ObservableCollection<Message>();

            CheckOptions();

            var mailServer = _mailService.GetWorker(Auth);
            Messages = mailServer.GetMessages();
        }

        private void CheckOptions()
        {
            var options = _optionsService.GetSender();
            Auth = options.GetAuth();

            if (Auth.Login is null || Auth.Password is null)
            {
                MessageBox.Show("Options !");
            }
        }
    }
}
