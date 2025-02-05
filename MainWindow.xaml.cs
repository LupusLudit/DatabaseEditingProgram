using DatabaseEditingProgram.database;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;


namespace DatabaseEditingProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string server = ServerTxtField.Text;
            string database = DatabaseTxtField.Text;
            string username = UsernameTxtField.Text;
            string password = PasswordBox.Password;

            if (ContainsWhiteSpaces(server, database, username, password)) 
            {
                MessageBox.Show("Please fill in all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UpdateConfig("DataSource", server);
            UpdateConfig("Database", database);
            UpdateConfig("Name", username);
            UpdateConfig("Password", password);

            if (TryConnect())
            {
                MessageBox.Show("Connection successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DatabaseWindow databaseWindow = new DatabaseWindow();
                databaseWindow.Show();
                this.Close();

            }
            else
            {
                MessageBox.Show("Connection failed, try again later", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool TryConnect()
        {
            try
            {
                using (SqlConnection conn = DatabaseSingleton.GetInstance())
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void UpdateConfig(string key, string newValue)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings[key].Value = newValue;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private bool ContainsWhiteSpaces(string server, string database, string username, string password)
        {
            return string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);
        }
    }
}