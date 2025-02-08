using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEditingProgram.database.databaseEntities
{
    public class Book: IDatabaseEntity
    {
        private int id;
        private string title;
        private bool isSigned;
        private float price;
        private Genre genre;
        private Genre publisher;

        public int ID { get { return id; } set { id = value; } }
        public string Title { get { return title; } set { title = value; } }
        public bool IsSigned { get { return isSigned; } set { isSigned = value; } }
        public float Price { get { return price; } set { price = value; } }
        public Genre Genre { get { return genre; } set { genre = value; } }
        public Genre Publisher { get { return publisher; } set { publisher = value; } }

        public Book(int id, string title, bool isSigned, float price, Genre genre, Genre publisher)
        {
            this.id = id;
            this.title = title;
            this.isSigned = isSigned;
            this.price = price;
            this.genre = genre;
            this.publisher = publisher;
        }

        public Book(string title, bool isSigned, float price, Genre genre, Genre publisher)
        {
            this.id = 0;
            this.title = title;
            this.isSigned = isSigned;
            this.price = price;
            this.genre = genre;
            this.publisher = publisher;
        }
    }
}
