using System.Diagnostics;
using System.Windows;
using DatabaseEditingProgram.database;

namespace DatabaseEditingProgram
{
    /// <summary>
    /// Interaction logic for DatabaseWindow.xaml
    /// </summary>
    public partial class DatabaseWindow : Window
    {
        private string databaseName;
        public DatabaseWindow(string databaseName)
        {
            InitializeComponent();
            this.databaseName = databaseName;
            TitleLabel.Content = $"Connected to database \"{databaseName}\"";
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string helpUrl = "https://github.com/LupusLudit/DatabaseEditingProgram";
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = helpUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to open the help page", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
