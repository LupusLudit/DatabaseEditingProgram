﻿using DatabaseEditingProgram.database.databaseEntities;
using DatabaseEditingProgram.managers;
using System.Collections.ObjectModel;


namespace DatabaseEditingProgram
{
    /// <include file='docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="DatabaseViewModel"]/*'/>
    public class DatabaseViewModel
    {
        public GenreTableManager GenreManager { get; }
        public PublisherTableManager PublisherManager { get; }
        public CustomerTableManager CustomerManager { get; }
        public BookTableManager BookManager { get; }
        public PurchaseTableManager PurchaseManager { get; }


        public List<bool> BooleanOptions { get; } = new() { true, false };
        public ObservableCollection<Genre> GenresLookUp { get; }
        public ObservableCollection<Publisher> PublishersLookUp { get; }
        public ObservableCollection<Customer> CustomersLookUp { get; }
        public ObservableCollection<Book> BooksLookUp { get; }

        public DatabaseViewModel()
        {
            GenreManager = new GenreTableManager();
            PublisherManager = new PublisherTableManager();
            CustomerManager = new CustomerTableManager();

            GenresLookUp = GenreManager.Items;
            PublishersLookUp = PublisherManager.Items;

            BookManager = new BookTableManager(GenresLookUp, PublishersLookUp);

            CustomersLookUp = CustomerManager.Items;
            BooksLookUp = BookManager.Items;

            PurchaseManager = new PurchaseTableManager(CustomersLookUp, BooksLookUp, GenresLookUp, PublishersLookUp);
        }
    }
}