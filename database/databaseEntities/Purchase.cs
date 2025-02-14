using System.ComponentModel;

namespace DatabaseEditingProgram.database.databaseEntities
{
    public class Purchase : IDatabaseEntity, INotifyPropertyChanged
    {
        private int id;
        private Customer customer;
        private Book book;
        private float surcharge;
        private DateTime date;
        private TimeSpan time;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int ID { get { return id; } set { id = value; } }
        public Customer Customer { get { return customer; } set { customer = value; } }
        public Book Book { get { return book; } set { book = value; } }
        public float Surcharge
        {
            get { return surcharge; }
            set 
            {
                surcharge = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Price))); //Ensuring the Price will be updated dynamically 
            }
        }
        public float Price { get { return surcharge + book.Price; } }
        public DateTime Date { get { return date; } set { date = value; } }
        public TimeSpan Time { get { return time; } set { time = value; } }


        public Purchase(int id, Customer customer, Book book, float surcharge, DateTime date, TimeSpan time) 
        {
            this.id = id;
            this.customer = customer;
            this.book = book;
            this.surcharge = surcharge;
            this.date = date;
            this.time = time;
        }

        public Purchase(Customer customer, Book book, float surcharge, DateTime date, TimeSpan time)
        {
            this.id = 0;
            this.customer = customer;
            this.book = book;
            this.surcharge = surcharge;
            this.date = date;
            this.time = time;
        }
    }
}
