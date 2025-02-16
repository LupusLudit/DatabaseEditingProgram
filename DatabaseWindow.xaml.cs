using System.Windows;
using DatabaseEditingProgram.database;

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
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseSingleton.CloseConnection();
            MessageBox.Show("Connection closed successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            new MainWindow().Show();
            this.Close();
        }
    }
}
