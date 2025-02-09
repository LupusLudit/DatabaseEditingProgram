using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseEditingProgram
{
    public partial class DatabaseWindow : Window
    {
        private string databaseName;
        List<Genre> genres = new List<Genre>();
        GenreDAO genreDAO = new GenreDAO();
        public DatabaseWindow(string databaseName)
        {
            InitializeComponent();
            this.databaseName = databaseName;
            TitleLabel.Content = $"Connected to database \"{databaseName}\"";
            CreateExampleGenres();
            LoadGenres();
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


        private void LoadGenres()
        {
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
            MessageBox.Show("Connection closed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            new MainWindow().Show();
            this.Close();
        }
    }
}
