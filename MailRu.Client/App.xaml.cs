using MailRu.Client.Services;
using MailRu.Client.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MailRu.Client
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost _hosting;

        public static IHost Hosting
        {
            get
            {
                if (_hosting != null) return _hosting;
                _hosting = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs()).ConfigureServices(ConfigureServices).Build();
                return _hosting;
            }
        }

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<IMailService, MailRuService>();
        }

        public static IServiceProvider Services { get { return Hosting.Services; } }
    }
}
