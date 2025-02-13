using System.Windows.Input;

/*
 * Note: this part of the code is NOT entirely mine (AddButtonCommand)
 * Inspiration: https://stackoverflow.com/questions/1468791/icommand-mvvm-implementation
 */

namespace DatabaseEditingProgram.commands
{
    public class AddButtonCommand : ICommand
    {
        private readonly Action _execute; //Action to be executed
        private readonly Func<bool>? _canExecute; //Command is always executable if _canExecute == null

        public event EventHandler? CanExecuteChanged;

        public AddButtonCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        //Will call _canExecute if its not null otherwise, returns true
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        //Executes the _execute action
        public void Execute(object? parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
