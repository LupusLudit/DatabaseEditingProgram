using DatabaseEditingProgram.commands;
using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DatabaseEditingProgram.managers
{
    /// <include file='../docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="TableManager"]/*'/>
    public abstract class TableManager<T> where T : IDatabaseEntity
    {
        public ObservableCollection<T> Items { get; protected set; } //ObservableCollection will automatically update the UI table
        protected readonly IDAO<T> DAO;
        
        //Commands are being called in the DatabaseWindow.xaml
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ReloadCommand { get;  }

        /// <summary>
        /// Table manager constructor.
        /// All commands are assigned here.
        /// </summary>
        /// <param name="dao"></param>
        public TableManager(IDAO<T> dao)
        {
            DAO = dao;
            Items = new ObservableCollection<T>(dao.GetAll());

            SaveCommand = new ArgumentButtonCommand<T>(Save);
            DeleteCommand = new ArgumentButtonCommand<T>(Delete);
            AddCommand = new UniversalButtonCommand(AddNew);
            ImportCommand = new UniversalButtonCommand(Import);
            ExportCommand = new UniversalButtonCommand(Export);
            ReloadCommand = new UniversalButtonCommand(Reload);
        }

        //*Description of these methods also applies to all classes implementing this abstract class

        /// <summary>
        /// Saves the specified entity to the database.
        /// </summary>
        /// <param name="item">The entity to save.</param>
        protected void Save(T item)
        {
            try
            {
                if (item == null) return;
                DAO.Save(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Deletes the specified entity from the database and removes it from the collection.
        /// </summary>
        /// <param name="item">The entity to delete.</param>
        protected void Delete(T item)
        {
            try
            {
                if (item == null) return;
                DAO.Delete(item);
                Items.Remove(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Reloads the data from the database and updates the collection.
        /// </summary>
        protected void Reload()
        {
            try
            {
                Items.Clear();
                var allBooks = DAO.GetAll();
                foreach (var book in allBooks)
                {
                    Items.Add(book);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        protected abstract void AddNew();

        /// <summary>
        /// Imports data from an external source.
        /// </summary>
        protected abstract void Import();

        /// <summary>
        /// Exports data to an external destination.
        /// </summary>
        protected abstract void Export();
    }
}
