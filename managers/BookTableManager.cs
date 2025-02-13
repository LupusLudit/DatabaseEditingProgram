using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;
using System.Collections.ObjectModel;
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
        }
        protected override void AddNew()
        {
            Genre firstGenre = loadedGenres[0];
            Publisher firstPublisher = loadedPublishers[0];

            if (firstGenre != null && firstPublisher != null)
            {
                Book newBook = new Book("New Book", false, 0.0f, firstGenre, firstPublisher);
                DAO.Save(newBook);
                Items.Add(newBook);
            }
            else
            {
                MessageBox.Show("If creating a new book, there must be some genres and publishers in the database first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
