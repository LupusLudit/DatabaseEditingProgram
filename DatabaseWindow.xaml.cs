using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DatabaseEditingProgram.database;

namespace DatabaseEditingProgram
{
    public partial class DatabaseWindow : Window
    {
        private string databaseName;
        private ObservableCollection<Genre> genres = new ObservableCollection<Genre>(); //Automatically updates
        GenreDAO genreDAO = new GenreDAO();
        public DatabaseWindow(string databaseName)
        {
            InitializeComponent();
            this.databaseName = databaseName;
            TitleLabel.Content = $"Connected to database \"{databaseName}\"";
            //CreateExampleGenres();
            LoadGenres();
        }

        private void GenreTable_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                Genre editedGenre = (Genre)e.Row.Item;
                genreDAO.Save(editedGenre);
            }
        }


        private void CreateExampleGenres()
        {
            genreDAO.CreateTable();
            if (!genreDAO.GetAll().Any())
            {
                genreDAO.Save(new Genre("Sci-fi"));
                genreDAO.Save(new Genre("Novel"));
                genreDAO.Save(new Genre("Action"));
                genreDAO.Save(new Genre("Tales"));
            }
        }

        private void GenreTable_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && GenreTable.SelectedItem is Genre selectedGenre)
            {
                genreDAO.Delete(selectedGenre);
                genres.Remove(selectedGenre);
            }
        }


        private void LoadGenres()
        {
            genres.Clear();
            foreach (var genre in genreDAO.GetAll())
            {
                genres.Add(genre);
            }

            GenreTable.ItemsSource = genres;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e) { }
        private void ExportButton_Click(object sender, RoutedEventArgs e) { }
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseSingleton.CloseConnection();
            MessageBox.Show("Connection closed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            new MainWindow().Show();
            this.Close();
        }
    }
}
