using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DatabaseEditingProgram.database;
using Microsoft.Data.SqlClient;

namespace DatabaseEditingProgram
{
    public partial class DatabaseWindow : Window
    {
        private string databaseName;
        public DatabaseWindow(string databaseName)
        {
            InitializeComponent();
            //CreateExampleGenres();
            this.databaseName = databaseName;
            TitleLabel.Content = $"Connected to database \"{databaseName}\"";

        }

        private void CreateExampleGenres()
        {
            GenreDAO genreDAO = new GenreDAO();
            genreDAO.Save(new Genre("Sci-fi"));
            genreDAO.Save(new Genre("Novel"));
            genreDAO.Save(new Genre("Action"));
            genreDAO.Save(new Genre("Tales"));
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
