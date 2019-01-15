using System;
using System.Windows.Input;

namespace BoincManagerWindows
{
    /// <summary>
    /// In the MVVM Light Toolkit, the ICommand implementation is called RelayCommand.
    /// </summary>
    class RelayCommand : ICommand
    {
        readonly Action action;
        readonly Func<bool> canExecuteEvaluator;

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
        /// </summary>
        /// <param name="methodToExecute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> Will always return true.</remarks>
        public RelayCommand(Action methodToExecute) : this(methodToExecute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="action">The execution logic.</param>
        /// <param name="canExecuteEvaluator">The execution status logic.</param>
        public RelayCommand(Action action, Func<bool> canExecuteEvaluator)
        {
            this.action = action;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }

        ///<summary>
        /// Defines the method that determines whether the command can be executedor not.
        ///</summary>
        ///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        ///<returns>
        /// True if this command can be executed; otherwise, false.
        ///</returns>
        public bool CanExecute(object parameter)
        {
            return canExecuteEvaluator == null ? true : canExecuteEvaluator.Invoke();
        }

        ///<summary>
        /// Defines the method to be called when the command is invoked.
        ///</summary>
        ///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            action.Invoke();
        }

        ///<summary>
        ///  Occurs when changes occur that affect whether or not the command should execute.
        ///</summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
