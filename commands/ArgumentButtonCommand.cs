using DatabaseEditingProgram.database.databaseEntities;
using System.Windows.Input;

/*
 * Note: this part of the code is NOT entirely mine (ArgumentButtonCommand)
 * Inspiration: https://stackoverflow.com/questions/1468791/icommand-mvvm-implementation
 */

namespace DatabaseEditingProgram
{
    /// <include file='../docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="ArgumentButtonCommand"]/*'/>
    public class ArgumentButtonCommand<T> : ICommand where T : IDatabaseEntity
    {
        private readonly Action<T?> _execute; //Action to be executed
        private readonly Func<T?, bool>? _canExecute; //Command is always executable if _canExecute == null

        public event EventHandler? CanExecuteChanged; //Event Occurs when the state of the command changes

        public ArgumentButtonCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The command parameter (expected to be of type T).</param>
        /// <returns>True if the command can execute (otherwise false)</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke((T?) parameter) ?? true;
        }

        /// <summary>
        /// Executes the command action (_execute).
        /// </summary>
        /// <param name="parameter">The command parameter (expected to be of type T).</param>
        public void Execute(object? parameter)
        {
            _execute((T?) parameter);
        }

        /// <summary>
        /// Implemented ICommand function, notifies the CanExecuteChanged
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
