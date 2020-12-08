using MailRu.Client.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MailRu.Client.ViewModel
{
    public class OptionsViewModel : BaseViewModel
    {
        public ICommand SaveCommand { get; set; }
        public void SaveCommand_Execute()
        {

        }

        public bool SaveCommand_CanExecute()
        {
            return true;
        }

        public OptionsViewModel()
        {
            SaveCommand = new Command(SaveCommand_Execute, SaveCommand_CanExecute);
        }
    }
}
