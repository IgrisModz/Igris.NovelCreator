using System;
using System.Windows.Input;

namespace Igris.Mvvm
{
    /// <summary>
    /// The base class for commands.
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        /// Invoked on can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Checks if the executeMethod can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Invokes the executeMethod.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public abstract void Execute(object parameter);

        ///// <summary>
        ///// Raises the <see cref="CanExecuteChanged"/> event.
        ///// </summary>
        //public virtual void RaiseCanExecuteChanged()
        //{
        //    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        //}
    }
}
