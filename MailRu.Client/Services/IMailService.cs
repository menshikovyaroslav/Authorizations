using MailRu.Client.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailRu.Client.Services
{
    public interface IMailService
    {
        IMailRuWorker GetSender(string login, string password);
    }

    public interface IMailRuWorker
    {
        ObservableCollection<Message> GetMessages();
    }
}
