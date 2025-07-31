using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IpisCentralDisplayController.commands
{
    //internal class RelayCommand
    //{
    //    private readonly Action<object?> _execute;
    //    private readonly Func<object?, bool>? _canExecute;
    //    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    //    {
    //        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    //        _canExecute = canExecute;
    //    }
    //    public bool CanExecute(object? parameter)
    //    {
    //        return _canExecute?.Invoke(parameter) ?? true;
    //    }
    //    public void Execute(object? parameter)
    //    {
    //        _execute(parameter);
    //    }
    //    public event EventHandler? CanExecuteChanged;

    //    public void RaiseCanExecuteChanged()
    //    {
    //        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    //    }
    //}

    //Find out what is the difference between these RelayCommands

    public class RelayCommand : ICommand
    {
        private Action<object?> execute;
        private Func<object?, bool> canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute = null!)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            //throw new NotImplementedException();
            //  execute(parameter);
            execute(parameter);
        }

        // Add this method to manually raise the CanExecuteChanged event
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }




    }

 //   RaiseCanExecuteChanged

}
