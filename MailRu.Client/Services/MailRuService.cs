using MailRu.Client.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailRu.Client.Services
{
    public class MailRuService : IMailService
    {
        public IMailRuWorker GetWorker(Auth auth)
        {
            return new MailRuWorker(auth);
        }
    }

    public class MailRuWorker : IMailRuWorker
    {
        public MailRuWorker(Auth auth)
        {

        }

        public ObservableCollection<Message> GetMessages()
        {
            var result = new ObservableCollection<Message>();
            result.Add(new Message() {Body = "body", FromAddress = "from", Subject = "subject", Time = DateTime.Now });
            return result;
        }
    }
}
