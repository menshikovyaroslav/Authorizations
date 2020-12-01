using MailRu.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailRu.Client.Services
{
    public interface IOptionsService
    {
        IOptionsWorker GetSender();
    }

    public interface IOptionsWorker
    {
        Auth GetAuth();
    }
}
