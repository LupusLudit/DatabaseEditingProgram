using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Win32;
using System.Windows;

namespace DatabaseEditingProgram.managers
{
    public class GenreTableManager : TableManager<Genre>
    {
        public GenreTableManager() : base(new GenreDAO()) {}

        protected override void AddNew()
        {
            try
            {
                Genre genre = new Genre("New Genre");
                DAO.Save(genre);
                Items.Add(genre);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        protected override void Delete(Genre genre)
        {
            if (genre == null) return;
            DAO.Delete(genre);
            Items.Remove(genre);
        }
        protected override void Save(Genre genre)
        {
            if (genre == null) return;
            DAO.Save(genre);
        }

        protected override void Reload()
        {
            Items.Clear();
            var allGenres = DAO.GetAll();
            foreach (var genre in allGenres)
            {
                Items.Add(genre);
            }
        }
        protected override void Import()
        {
            if (DAO.ForbiddenTablesNotEmpty())
            {
                MessageBox.Show("Import is not allowed because the book table or the purchase table is not empty", "Import Blocked", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Import Genres from CSV"
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
                Title = "Export Genres to CSV",
                FileName = "genres.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                DAO.ExportToCsv(saveFileDialog.FileName);
            }
        }

    }

}
