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
        public ICommand ImportCommand { get; }
        public ICommand ExportCommand { get; }


        public TableManager(IDAO<T> dao)
        {
            DAO = dao;
            Items = new ObservableCollection<T>(dao.GetAll());

            SaveCommand = new ArgumentButtonCommand<T>(Save);
            DeleteCommand = new ArgumentButtonCommand<T>(Delete);
            AddCommand = new UniversalButtonCommand(AddNew);
            ImportCommand = new UniversalButtonCommand(Import);
            ExportCommand = new UniversalButtonCommand(Export);
        }

        protected abstract void Save(T item);
        protected abstract void Delete(T item);
        protected abstract void AddNew();
        protected abstract void Import();
        protected abstract void Export();
        protected abstract void Reload();


    }
}
