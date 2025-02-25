﻿using DatabaseEditingProgram.database;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
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
        private bool connecting;
        private CancellationTokenSource cts = new CancellationTokenSource();

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            connecting = true;
            ConnectButton.IsEnabled = false;
            HelpButton.IsEnabled = false;

            string server = ServerTxtField.Text;
            string database = DatabaseTxtField.Text;
            string username = UsernameTxtField.Text;
            string password = PasswordBox.Password;

            if (ContainsWhiteSpaces(server, database, username, password))
            {
                MessageBox.Show("Please fill in all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Reset();
                return;
            }

            StatusLabel.Visibility = Visibility.Visible;
            cts = new CancellationTokenSource();
            Task animationTask = AnimateConnectingLabel(cts.Token); //Note: label animation code and the token idea is NOT entirely mine

            await Task.Delay(500);

            UpdateConfig("DataSource", server); //Note: UpdateConfig code is NOT entirely mine
            UpdateConfig("Database", database);
            UpdateConfig("Name", username);
            UpdateConfig("Password", password);

            bool isConnected = await Task.Run(() => TryConnect());

            StopLabelAnimation();
            Reset();

            if (isConnected)
            {
                var result = MessageBox.Show(WarningMessage(), "Confirmation",
                                             MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DatabaseWindow databaseWindow = new DatabaseWindow(database);
                    databaseWindow.Show();
                    this.Close();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                MessageBox.Show("Connection failed, try again later", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string WarningMessage()
        {
            return "Attention: If this is your first time connecting, be aware that this program will modify your database. " +
                   "If your database contains any of the following tables: \"genre\", \"publisher\", \"customer\", \"book\", or \"purchase\", " +
                   "and they do not match the required format, they will be removed and overwritten. " +
                   "If these tables exist in your database but are unrelated to this program, all data within them will be permanently deleted. " +
                   "If you have previously connected to this database, the tables should already be in the correct format, and you can ignore this warning. " +
                   "\n\nDo you want to proceed? Selecting \"NO\" will close the program.";
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (connecting)
            {
                StopLabelAnimation();
                DatabaseSingleton.CloseConnection();
                ConnectButton.IsEnabled = true;
                HelpButton.IsEnabled = true;
                MessageBox.Show("Connection closed by the user", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                connecting = false;
                return;
            }
            else
            {
                this.Close();
            }
        }

        private bool TryConnect()
        {
            try
            {
                DatabaseSingleton.CloseConnection();
                SqlConnection conn = DatabaseSingleton.GetInstance();
                return conn.State == System.Data.ConnectionState.Open;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ContainsWhiteSpaces(string server, string database, string username, string password)
        {
            return string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password);
        }

        private void StopLabelAnimation()
        {
            cts.Cancel();
            StatusLabel.Visibility = Visibility.Collapsed;
        }

        private void Reset()
        {
            connecting = false;
            ConnectButton.IsEnabled = true;
            HelpButton.IsEnabled = true;
        }

        /*
         * Note: this part of the code is NOT entirely mine (Label animation & UpdateConfig),
         * Inspiration: https://dotnettutorials.net/lesson/how-to-cancel-a-task-in-csharp/
         * Inspiration: https://stackoverflow.com/questions/1357240/change-the-value-in-app-config-file-dynamically
         */
        private async Task AnimateConnectingLabel(CancellationToken token)
        {
            string[] states = { "Connecting.", "Connecting..", "Connecting..." };
            int index = 0;

            while (!token.IsCancellationRequested)
            {
                StatusLabel.Content = states[index];
                index = (index + 1) % states.Length;
                await Task.Delay(500); 
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


    }
}