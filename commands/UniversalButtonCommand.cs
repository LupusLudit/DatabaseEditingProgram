using DatabaseEditingProgram.database.databaseEntities;
using System.Windows.Input;

/*
 * Note: this part of the code is NOT entirely mine (UniversalButtonCommand)
 * Inspiration: https://stackoverflow.com/questions/1468791/icommand-mvvm-implementation
 */

namespace DatabaseEditingProgram
{
    public class UniversalButtonCommand<T> : ICommand where T : IDatabaseEntity
    {
        private readonly Action<T?> _execute; //Action to be executed
        private readonly Func<T?, bool>? _canExecute; //Command is always executable if _canExecute == null

        public event EventHandler? CanExecuteChanged;

        public UniversalButtonCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        //Will call _canExecute if its not null otherwise, returns true
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke((T?) parameter) ?? true;
        }

        //Executes the _execute action
        public void Execute(object? parameter)
        {
            _execute((T?) parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
