using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;


namespace DatabaseEditingProgram.managers
{
    /// <include file='../docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="PurchaseTableManager"]/*'/>
    public class PurchaseTableManager : TableManager<Purchase>
    {
        private ObservableCollection<Customer> loadedCustomers;
        private ObservableCollection<Book> loadedBooks;
        private ObservableCollection<Genre> loadedGenres;
        private ObservableCollection<Publisher> loadedPublishers;

        /// <summary>
        /// PurchaseTableManager constructor.
        /// Assigns values to the implemented collections and adds OnCollectionChanged methods to them.
        /// By doing that we make sure that whenever an item is deleted from one of these collections (which are bound to the purchase)
        /// the purchase reloads.
        /// </summary>
        /// <param name="loadedCustomers"></param>
        /// <param name="loadedBooks"></param>
        /// <param name="loadedGenres"></param>
        /// <param name="loadedPublishers"></param>
        public PurchaseTableManager(ObservableCollection<Customer> loadedCustomers,
            ObservableCollection<Book> loadedBooks,
            ObservableCollection<Genre> loadedGenres,
            ObservableCollection<Publisher> loadedPublishers) : base(new PurchaseDAO())
        {
            this.loadedCustomers = loadedCustomers;
            this.loadedBooks = loadedBooks;
            this.loadedGenres = loadedGenres;
            this.loadedPublishers = loadedPublishers;

            this.loadedCustomers.CollectionChanged += OnCollectionChanged;
            this.loadedBooks.CollectionChanged += OnCollectionChanged;
            this.loadedGenres.CollectionChanged += OnCollectionChanged;
            this.loadedPublishers.CollectionChanged += OnCollectionChanged;
        }
        protected override void AddNew()
        {
            if (loadedCustomers.Any() && loadedBooks.Any() && loadedGenres.Any() && loadedPublishers.Any())
            {
                Customer firstCustomer = loadedCustomers.First();
                Book firstBook = loadedBooks.First();

                Purchase newPurchase = new Purchase(firstCustomer, firstBook, 0.0f, DateTime.Today, DateTime.Now.TimeOfDay);
                DAO.Save(newPurchase);
                Items.Add(newPurchase);
            }
            else
            {
                MessageBox.Show("If creating a new purchase, there must be some customers and books (genres & publishers) in the database first",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /*
         * Not implemented for this class.
         * I kept the Import and Export functions as protected abstract voids in the abstract class to
         * proof possible implementation in all classes.
         */
        protected override void Import() { }
        protected override void Export() { }

        /*
         * Note: this part of the code is NOT entirely mine (OnCollectionChanged)
         * Inspiration: https://stackoverflow.com/questions/4588359/implementing-collectionchanged
        */

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Reload();
        }
    }
}
