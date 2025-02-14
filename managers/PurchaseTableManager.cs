using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;


namespace DatabaseEditingProgram.managers
{
    public class PurchaseTableManager : TableManager<Purchase>
    {
        private ObservableCollection<Customer> loadedCustomers;
        private ObservableCollection<Book> loadedBooks;

        public PurchaseTableManager(ObservableCollection<Customer> loadedCustomers, ObservableCollection<Book> loadedBooks)
            : base(new PurchaseDAO())
        {
            this.loadedCustomers = loadedCustomers;
            this.loadedBooks = loadedBooks;

            this.loadedCustomers.CollectionChanged += OnCustomerCollectionChanged;
            this.loadedBooks.CollectionChanged += OnBookCollectionChanged;
        }
        protected override void AddNew()
        {
            if (loadedCustomers.Any() && loadedBooks.Any())
            {
                Customer firstCustomer = loadedCustomers.First();
                Book firstBook = loadedBooks.First();

                Purchase newPurchase = new Purchase(firstCustomer, firstBook, 0.0f, DateTime.Today, DateTime.Now.TimeOfDay);
                DAO.Save(newPurchase);
                Items.Add(newPurchase);
            }
            else
            {
                MessageBox.Show("If creating a new purchase, there must be some customers and books in the database first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected override void Delete(Purchase purchase)
        {
            if (purchase == null) return;
            DAO.Delete(purchase);
            Items.Remove(purchase);
        }

        protected override void Save(Purchase purchase)
        {
            if (purchase == null) return;
            DAO.Save(purchase);
        }

        private void ReloadPurchases()
        {
            Items.Clear();
            var allPurchases = DAO.GetAll();
            foreach (var purchase in allPurchases)
            {
                Items.Add(purchase);
            }
        }

        /*
         * Note: this part of the code is NOT entirely mine (OnCustomerCollectionChanged & OnBookCollectionChanged)
         * Inspiration: https://stackoverflow.com/questions/4588359/implementing-collectionchanged
        */

        private void OnCustomerCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (Customer removedCustomer in e.OldItems)
                    {
                        var purchasesToRemove = Items.Where(p => p.Customer.ID == removedCustomer.ID).ToList();
                        foreach (var purchase in purchasesToRemove)
                        {
                            DAO.Delete(purchase);
                        }
                    }
                    ReloadPurchases();
                }
            }
        }

        private void OnBookCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (Book removedBook in e.OldItems)
                    {
                        var purchasesToRemove = Items.Where(p => p.Book.ID == removedBook.ID).ToList();
                        foreach (var purchase in purchasesToRemove)
                        {
                            DAO.Delete(purchase);
                        }
                    }
                    ReloadPurchases();
                }
            }
        }
    }
}
