using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;

namespace DatabaseEditingProgram.database.dao
{
    /// <include file='../../docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="PurchaseDAO"]/*'/>
    public class PurchaseDAO : IDAO<Purchase>
    {
        public PurchaseDAO()
        {
            CreateTable();
        }

        /// <summary>
        /// Creates the purchase table if it does not exist.
        /// </summary>
        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            RemoveIncorrectFormat();

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
        /// <summary>
        /// Deletes a purchase from the database.
        /// </summary>
        /// <param name="purchase">The purchase to delete.</param>
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


        /// <summary>
        /// Retrieves all purchases from the database.
        /// </summary>
        /// <returns>A collection of purchases.</returns>
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

        /// <summary>
        /// Retrieves a purchase based on its ID.
        /// </summary>
        /// <param name="id">The purchase ID.</param>
        /// <returns>The purchase if found (otherwise null).</returns>
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

        /// <summary>
        /// Saves a purchase to the database. If the purchase does not exist, it is inserted. If it does, it is updated.
        /// </summary>
        /// <param name="purchase">The purchase to save.</param>
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

        /*
         * Not implemented for this class.
         * Their existence, however, proofs possible implementation for all DAO classes.
         */
        public void ExportToCsv(string filePath) { }
        public void ImportFromCsv(string filePath) { }
        public bool ForbiddenTablesNotEmpty()
        {
            return true;
        }

        /*
         * Note: this part of the code is NOT entirely mine (RemoveIncorrectFormat),
         * it was partially AI generated and I took inspiration from these sites:
         * Inspiration: https://learn.microsoft.com/en-us/sql/relational-databases/system-information-schema-views/system-information-schema-views-transact-sql?view=sql-server-ver16
         * Inspiration: https://database.guide/understanding-information_schema-in-sql/
         * Inspiration: https://www.geeksforgeeks.org/how-to-use-information_schema-views-in-sql-server/
         */

        /// <summary>
        /// Ensures the purchase table follows the expected schema. If it does not, the table gets deleted.
        /// Serves as a security check - if there is already a table with the name "purchase" that does not fit
        /// our schema and we do not drop it, it might cause some issues.
        /// </summary>
        public void RemoveIncorrectFormat()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            string checkTableSchema = @"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'purchase')
                BEGIN
                    DECLARE @mismatch BIT = 0;

                    -- Check if 'id' column exists with the correct type and is an identity column
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'purchase' AND COLUMN_NAME = 'id' 
                        AND DATA_TYPE = 'int' AND COLUMNPROPERTY(object_id('purchase'), 'id', 'IsIdentity') = 1
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'customer_id' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'purchase' AND COLUMN_NAME = 'customer_id' 
                        AND DATA_TYPE = 'int'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'book_id' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'purchase' AND COLUMN_NAME = 'book_id' 
                        AND DATA_TYPE = 'int'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'surcharge' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'purchase' AND COLUMN_NAME = 'surcharge' 
                        AND DATA_TYPE = 'float'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'purchase_date' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'purchase' AND COLUMN_NAME = 'purchase_date' 
                        AND DATA_TYPE = 'date'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'purchase_time' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'purchase' AND COLUMN_NAME = 'purchase_time' 
                        AND DATA_TYPE = 'time'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Drop table if it does not match the expected format
                    IF @mismatch = 1
                    BEGIN
                        DROP TABLE purchase;
                    END
                END";

            using (SqlCommand command = new SqlCommand(checkTableSchema, conn))
            {
                command.ExecuteNonQuery();
            }
        }

    }
}
