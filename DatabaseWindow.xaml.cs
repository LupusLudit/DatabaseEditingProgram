﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DatabaseEditingProgram
{
    public partial class DatabaseWindow : Window
    {
        private string databaseName;

        public DatabaseWindow(string databaseName)
        {
            InitializeComponent();
            this.databaseName = databaseName;
            TitleLabel.Content = $"Connected to database \"{databaseName}\"";
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
