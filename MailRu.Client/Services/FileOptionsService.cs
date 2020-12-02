using MailRu.Client.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
            var auth = new Auth();

            try
            {
                var lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.txt"));
                foreach (var item in lines)
                {
                    var splitted = item.Split('=');
                    if (splitted.Length != 2) continue;

                    if (splitted[0] == "login") auth.Login = splitted[1];
                    if (splitted[0] == "password") auth.Password = splitted[1];
                }
            }
            catch (Exception)
            {
            }

            return auth;
        }

        public void SetAuth(Auth auth)
        {
            var lines = new string[2];
            lines[0] = $"login={auth.Login}";
            lines[1] = $"password={auth.Password}";
            File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.txt"), lines);
        }
    }
}
