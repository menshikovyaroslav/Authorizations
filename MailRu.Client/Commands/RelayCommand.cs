using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MailRu.Client.Commands
{
    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private Action<T> methodToExecute;
        private Predicate<T> canExecuteEvaluator;
        public RelayCommand(Action<T> methodToExecute, Predicate<T> canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteEvaluator == null ? true : canExecuteEvaluator((T)parameter);
        }
        public void Execute(object parameter)
        {
            this.methodToExecute((T)parameter);
        }
    }
}
