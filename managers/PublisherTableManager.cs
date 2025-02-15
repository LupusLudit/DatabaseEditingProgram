using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Win32;

namespace DatabaseEditingProgram.managers
{
    public class PublisherTableManager : TableManager<Publisher>
    {
        public PublisherTableManager() : base(new PublisherDAO()) { }

        protected override void AddNew()
        {
            Publisher publisher = new Publisher("New Publisher", "New Motto", false);
            DAO.Save(publisher);
            Items.Add(publisher);
        }

        protected override void Delete(Publisher publisher)
        {
            if (publisher == null) return;
            DAO.Delete(publisher);
            Items.Remove(publisher);
        }

        protected override void Save(Publisher publisher)
        {
            if (publisher == null) return;
            DAO.Save(publisher);
        }

        protected override void Reload()
        {
            Items.Clear();
            var allPublishers = DAO.GetAll();
            foreach (var publisher in allPublishers)
            {
                Items.Add(publisher);
            }
        }

        protected override void Import()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Import Publishers from CSV"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DAO.ImportFromCsv(openFileDialog.FileName);
                Reload();
            }
        }

        protected override void Export()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Export Publishers to CSV",
                FileName = "publishers.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                DAO.ExportToCsv(saveFileDialog.FileName);
            }
        }
    }
}
