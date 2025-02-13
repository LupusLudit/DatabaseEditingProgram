using DatabaseEditingProgram.commands;
using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.ObjectModel;
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

        public TableManager(IDAO<T> dao)
        {
            DAO = dao;
            Items = new ObservableCollection<T>(dao.GetAll());

            SaveCommand = new UniversalButtonCommand<T>(Save);
            DeleteCommand = new UniversalButtonCommand<T>(Delete);
            AddCommand = new AddButtonCommand(AddNew);
        }

        protected abstract void Save(T item);
        protected abstract void Delete(T item);
        protected abstract void AddNew();

    }
}
