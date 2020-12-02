using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailRu.Client.ViewModel
{
    public class ViewModelLocator
    {
        public MainViewModel MainVM => App.Services.GetRequiredService<MainViewModel>();
        public OptionsViewModel OptionsVM => App.Services.GetRequiredService<OptionsViewModel>();
    }
}
