using MailRu.Client.Model;
using MailRu.Client.Services;
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
        private ObservableCollection<Message> _messages;

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

        IMailService _mailRuService;

        public MainViewModel(IMailService mailRuService)
        {
            _mailRuService = mailRuService;

            Directories = new ObservableCollection<Directory>();
            Directories.Add(new Directory() { Name="Входящие" });
            Directories.Add(new Directory() { Name = "Исходящие" });
            Directories.Add(new Directory() { Name = "Удаленные" });

            Messages = new ObservableCollection<Message>();

            var mailServer = _mailRuService.GetSender("login", "password");
            Messages = mailServer.GetMessages();
        }

    }
}
