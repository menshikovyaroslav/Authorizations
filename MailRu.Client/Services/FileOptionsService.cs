using MailRu.Client.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailRu.Client.Services
{
    public class FileOptionsService : IOptionsService
    {
        public IOptionsWorker GetSender()
        {
            return new FileOptionsWorker();
        }
    }

    public class FileOptionsWorker : IOptionsWorker
    {
        public FileOptionsWorker()
        {

        }

        public Auth GetAuth()
        {
            var auth = new Auth() {Login = "login1", Password = "pass1" };
            return auth;
        }
    }
}
