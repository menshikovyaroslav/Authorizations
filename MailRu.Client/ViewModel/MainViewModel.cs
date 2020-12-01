using MailRu.Client.Model;
using MailRu.Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MailRu.Client.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
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
