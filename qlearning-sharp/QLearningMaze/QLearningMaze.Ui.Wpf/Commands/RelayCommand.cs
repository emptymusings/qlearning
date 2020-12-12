using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace QLearningMaze.Ui.Wpf.Commands
{
    //public class RelayCommand : ICommand
    //{
    //    #region Fields 
    //    readonly Action<object> _execute;
    //    readonly Predicate<object> _canExecute;
    //    #endregion // Fields 
    //    #region Constructors 
    //    public RelayCommand(Action<object> execute) : this(execute, null) { }
    //    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    //    {
    //        if (execute == null)
    //            throw new ArgumentNullException("execute");
    //        _execute = execute; _canExecute = canExecute;
    //    }
    //    #endregion // Constructors 
    //    #region ICommand Members 
    //    public bool CanExecute(object parameter)
    //    {
    //        return _canExecute == null ? true : _canExecute(parameter);
    //    }
    //    public event EventHandler CanExecuteChanged
    //    {
    //        add { CommandManager.RequerySuggested += value; }
    //        remove { CommandManager.RequerySuggested -= value; }
    //    }
    //    public void Execute(object parameter) { _execute(parameter); }
    //    #endregion // ICommand Members 
    //}

    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private Action methodToExecute;
        private Func<bool> canExecuteEvaluator;
        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }
        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }
        public bool CanExecute(object parameter)
        {
            if (this.canExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = this.canExecuteEvaluator.Invoke();
                return result;
            }
        }
        public void Execute(object parameter)
        {
            this.methodToExecute.Invoke();
        }
    }
}
