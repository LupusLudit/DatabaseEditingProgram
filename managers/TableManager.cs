using DatabaseEditingProgram.commands;
using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace DatabaseEditingProgram.managers
{
    public abstract class TableManager<T> where T : IDatabaseEntity
    {
        public ObservableCollection<T> Items { get; protected set; } //ObservableCollection will automatically update the UI table
        protected readonly IDAO<T> DAO;
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ReloadCommand { get;  }

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

        protected abstract void AddNew();
        protected abstract void Import();
        protected abstract void Export();


    }
}
