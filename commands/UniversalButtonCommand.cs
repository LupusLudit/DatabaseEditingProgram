using System.Windows.Input;

/*
 * Note: this part of the code is NOT entirely mine (UniversalButtonCommand)
 * Inspiration: https://stackoverflow.com/questions/1468791/icommand-mvvm-implementation
 */

namespace DatabaseEditingProgram.commands
{
    /// <include file='../docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="UniversalButtonCommand"]/*'/>
    public class UniversalButtonCommand : ICommand
    {
        private readonly Action _execute; //Action to be executed
        private readonly Func<bool>? _canExecute; //Command is always executable if _canExecute == null

        public event EventHandler? CanExecuteChanged; //Event Occurs when the state of the command changes

        public UniversalButtonCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>True if the command can execute (otherwise false)</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        /// <summary>
        /// Executes the command action (_execute).
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public void Execute(object? parameter)
        {
            _execute();
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
