using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;


namespace DatabaseEditingProgram.managers
{
    /// <include file='../docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="BookTableManager"]/*'/>
    public class BookTableManager : TableManager<Book>
    {
        private ObservableCollection<Genre> loadedGenres;
        private ObservableCollection<Publisher> loadedPublishers;

        /// <summary>
        /// BookTableManager constructor.
        /// Assigns values to the implemented collections and adds OnCollectionChanged methods to them.
        /// By doing that we make sure that whenever an item is deleted from one of these collections (which are bound to the book)
        /// the book reloads.
        /// </summary>
        /// <param name="loadedGenres"></param>
        /// <param name="loadedPublishers"></param>
        public BookTableManager(ObservableCollection<Genre> loadedGenres, ObservableCollection<Publisher> loadedPublishers)
            : base(new BookDAO())
        {
            this.loadedGenres = loadedGenres;
            this.loadedPublishers = loadedPublishers;

            this.loadedGenres.CollectionChanged += OnCollectionChanged;
            this.loadedPublishers.CollectionChanged += OnCollectionChanged;
        }
        protected override void AddNew()
        {
            if (loadedGenres.Any() && loadedPublishers.Any())
            {
                Genre firstGenre = loadedGenres.First();
                Publisher firstPublisher = loadedPublishers.First();

                Book newBook = new Book("New Book", false, 0.0f, firstGenre, firstPublisher);
                DAO.Save(newBook);
                Items.Add(newBook);
            }
            else
            {
                MessageBox.Show("If creating a new book, there must be some genres and publishers in the database first",
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