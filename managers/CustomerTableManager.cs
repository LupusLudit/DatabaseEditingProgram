using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Win32;
using System.Windows;

namespace DatabaseEditingProgram.managers
{
    public class CustomerTableManager : TableManager<Customer>
    {
        public CustomerTableManager() : base(new CustomerDAO()) { }

        protected override void AddNew()
        {
            try
            {
                Customer customer = new Customer("New", "Customer", DateTime.Today);
                DAO.Save(customer);
                Items.Add(customer);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        protected override void Delete(Customer customer)
        {
            if (customer == null) return;
            DAO.Delete(customer);
            Items.Remove(customer);
        }

        protected override void Save(Customer customer)
        {
            if (customer == null) return;
            DAO.Save(customer);
        }

        protected override void Reload()
        {
            Items.Clear();
            var allCustomers = DAO.GetAll();
            foreach (var customer in allCustomers)
            {
                Items.Add(customer);
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
                Title = "Import Customers from CSV"
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
                Title = "Export Customers to CSV",
                FileName = "customers.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                DAO.ExportToCsv(saveFileDialog.FileName);
            }
        }
    }
}
