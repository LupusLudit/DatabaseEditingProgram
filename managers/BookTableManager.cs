using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;


namespace DatabaseEditingProgram.managers
{
    public class BookTableManager : TableManager<Book>
    {
        private ObservableCollection<Genre> loadedGenres;
        private ObservableCollection<Publisher> loadedPublishers;

        public BookTableManager(ObservableCollection<Genre> loadedGenres, ObservableCollection<Publisher> loadedPublishers)
            : base(new BookDAO())
        {
            this.loadedGenres = loadedGenres;
            this.loadedPublishers = loadedPublishers;

            this.loadedGenres.CollectionChanged += OnGenreCollectionChanged;
            this.loadedPublishers.CollectionChanged += OnPublisherCollectionChanged;
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
                MessageBox.Show("If creating a new book, there must be some genres and publishers in the database first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        protected override void Delete(Book book)
        {
            if (book == null) return;
            DAO.Delete(book);
            Items.Remove(book);
        }

        protected override void Save(Book book)
        {
            if (book == null) return;
            DAO.Save(book);
        }

        private void ReloadBooks()
        {
            Items.Clear();
            var allBooks = DAO.GetAll();
            foreach (var book in allBooks)
            {
                Items.Add(book);
            }
        }

        /*
         * Note: this part of the code is NOT entirely mine (OnGenreCollectionChanged & OnPublisherCollectionChanged)
         * Inspiration: https://stackoverflow.com/questions/4588359/implementing-collectionchanged
         */

        private void OnGenreCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (Genre removedGenre in e.OldItems)
                    {
                        var booksToRemove = Items.Where(b => b.Genre.ID == removedGenre.ID).ToList();
                        foreach (var book in booksToRemove)
                        {
                            DAO.Delete(book);
                        }
                    }
                    ReloadBooks();
                }
            }
        }

        private void OnPublisherCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (Publisher removedPublisher in e.OldItems)
                    {
                        var booksToRemove = Items.Where(b => b.Publisher.ID == removedPublisher.ID).ToList();
                        foreach (var book in booksToRemove)
                        {
                            DAO.Delete(book);
                        }
                    }
                    ReloadBooks();
                }
            }
        }

    }
}