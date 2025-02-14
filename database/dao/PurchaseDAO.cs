using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;

namespace DatabaseEditingProgram.database.dao
{
    public class PurchaseDAO : IDAO<Purchase>
    {
        public PurchaseDAO()
        {
            CreateTable();
        }

        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string createPurchaseTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'purchase')
                BEGIN
                    CREATE TABLE purchase (
                        id INT PRIMARY KEY IDENTITY,
                        customer_id INT,
                        book_id INT,
                        surcharge FLOAT,
                        purchase_date DATE,
                        purchase_time TIME,
                        FOREIGN KEY (customer_id) REFERENCES customer(id) ON DELETE CASCADE,
                        FOREIGN KEY (book_id) REFERENCES book(id) ON DELETE CASCADE
                    );
                END";

            using (SqlCommand command = new SqlCommand(createPurchaseTable, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Delete(Purchase purchase)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("DELETE FROM purchase WHERE id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", purchase.ID);
                command.ExecuteNonQuery();
                purchase.ID = 0;
            }
        }

        public IEnumerable<Purchase> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            string selectPurchases = @"
                    SELECT purchase.id, purchase.surcharge, purchase.purchase_date, purchase.purchase_time,
                    customer.id, customer.name, customer.surname, customer.date_of_birth,
                    book.id, book.title, book.is_signed, book.price,
                    genre.id, genre.name,
                    publisher.id, publisher.name, publisher.motto, publisher.active
                    FROM purchase
                    JOIN customer ON purchase.customer_id = customer.id
                    JOIN book ON purchase.book_id = book.id
                    JOIN genre ON book.genre_id = genre.id
                    JOIN publisher ON book.publisher_id = publisher.id";

            using (SqlCommand command = new SqlCommand(selectPurchases, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Genre genre = new Genre(reader.GetInt32(12), reader.GetString(13));
                        Publisher publisher = new Publisher(reader.GetInt32(14), reader.GetString(15), reader.GetString(16), reader.GetBoolean(17));

                        Customer customer = new Customer(reader.GetInt32(4), reader.GetString(5), reader.GetString(6), reader.GetDateTime(7));
                        Book book = new Book(reader.GetInt32(8), reader.GetString(9), reader.GetBoolean(10), (float)reader.GetDouble(11), genre, publisher);

                        Purchase purchase = new Purchase(
                            reader.GetInt32(0),
                            customer,
                            book,
                            (float)reader.GetDouble(1),
                            reader.GetDateTime(2),
                            reader.GetTimeSpan(3)
                        );

                        yield return purchase;
                    }
                }
            }
        }


        public Purchase? GetByID(int id)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            string selectPurchase = @"
                    SELECT purchase.id, purchase.surcharge, purchase.purchase_date, purchase.purchase_time,
                    customer.id, customer.name, customer.surname, customer.date_of_birth,
                    book.id, book.title, book.is_signed, book.price,
                    genre.id, genre.name,
                    publisher.id, publisher.name, publisher.motto, publisher.active
                    FROM purchase
                    JOIN customer ON purchase.customer_id = customer.id
                    JOIN book ON purchase.book_id = book.id
                    JOIN genre ON book.genre_id = genre.id
                    JOIN publisher ON book.publisher_id = publisher.id
                    WHERE purchase.id = @id";

            using (SqlCommand command = new SqlCommand(selectPurchase, conn))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Genre genre = new Genre(reader.GetInt32(12), reader.GetString(13));
                        Publisher publisher = new Publisher(reader.GetInt32(14), reader.GetString(15), reader.GetString(16), reader.GetBoolean(17));

                        Customer customer = new Customer(reader.GetInt32(4), reader.GetString(5), reader.GetString(6), reader.GetDateTime(7));
                        Book book = new Book(reader.GetInt32(8), reader.GetString(9), reader.GetBoolean(10), (float)reader.GetDouble(11), genre, publisher);

                        return new Purchase(
                            reader.GetInt32(0),
                            customer,
                            book,
                            (float)reader.GetDouble(1),
                            reader.GetDateTime(2),
                            reader.GetTimeSpan(3)
                        );
                    }
                }
            }
            return null;
        }


        public void Save(Purchase purchase)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            if (purchase.ID == 0)
            {
                string insertIntoPurchase = @"
                INSERT INTO purchase (customer_id, book_id, surcharge, purchase_date, purchase_time)
                VALUES (@customer_id, @book_id, @surcharge, @purchase_date, @purchase_time);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(insertIntoPurchase, conn))
                {
                    command.Parameters.AddWithValue("@customer_id", purchase.Customer.ID);
                    command.Parameters.AddWithValue("@book_id", purchase.Book.ID);
                    command.Parameters.AddWithValue("@surcharge", purchase.Surcharge);
                    command.Parameters.AddWithValue("@purchase_date", purchase.Date);
                    command.Parameters.AddWithValue("@purchase_time", purchase.Time);

                    purchase.ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            else
            {
                string updatePurchase = @"
                UPDATE purchase
                SET customer_id = @customer_id, book_id = @book_id, surcharge = @surcharge, purchase_date = @purchase_date, purchase_time = @purchase_time
                WHERE id = @id";

                using (SqlCommand command = new SqlCommand(updatePurchase, conn))
                {
                    command.Parameters.AddWithValue("@id", purchase.ID);
                    command.Parameters.AddWithValue("@customer_id", purchase.Customer.ID);
                    command.Parameters.AddWithValue("@book_id", purchase.Book.ID);
                    command.Parameters.AddWithValue("@surcharge", purchase.Surcharge);
                    command.Parameters.AddWithValue("@purchase_date", purchase.Date);
                    command.Parameters.AddWithValue("@purchase_time", purchase.Time);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
