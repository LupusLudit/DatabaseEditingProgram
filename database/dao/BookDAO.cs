using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;
namespace DatabaseEditingProgram.database.dao
{
    public class BookDAO : IDAO<Book>
    {
        public BookDAO()
        {
            CreateTable();
        }

        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            RemoveIncorrectFormat();

            string createCustomerTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'book')
                BEGIN
                    CREATE TABLE book (
                        id INT PRIMARY KEY IDENTITY, 
                        title VARCHAR(255), 
                        is_signed BIT,
                        price FLOAT,
                        genre_id INT,
                        publisher_id INT,
                        FOREIGN KEY (genre_id) REFERENCES genre(id) ON DELETE CASCADE,
                        FOREIGN KEY (publisher_id) REFERENCES publisher(id) ON DELETE CASCADE
                    );
                END";

            using (SqlCommand command = new SqlCommand(createCustomerTable, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Delete(Book book)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("DELETE FROM book WHERE id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", book.ID);
                command.ExecuteNonQuery();
                book.ID = 0;
            }
        }

        public IEnumerable<Book> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string selectBooks = @"
                    SELECT book.id, book.title, book.is_signed, book.price, 
                    genre.id, genre.name,
                    publisher.id, publisher.name, publisher.motto, publisher.active
                    FROM book
                    JOIN genre ON book.genre_id = genre.id
                    JOIN publisher ON book.publisher_id = publisher.id";

            using (SqlCommand command = new SqlCommand(selectBooks, conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Genre genre = new Genre(reader.GetInt32(4), reader.GetString(5));
                        Publisher publisher = new Publisher(reader.GetInt32(6), reader.GetString(7), reader.GetString(8), reader.GetBoolean(9));

                        yield return new Book(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetBoolean(2),
                            (float)reader.GetDouble(3),
                            genre,
                            publisher
                        );
                    }
                }
            }
        }


        public Book? GetByID(int id)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string selectBook = @"
                    SELECT book.id, book.title, book.is_signed, book.price, 
                    genre.id, genre.name,
                    publisher.id, publisher.name, publisher.motto, publisher.active
                    FROM book
                    JOIN genre ON book.genre_id = genre.id
                    JOIN publisher ON book.publisher_id = publisher.id
                    WHERE book.id = @id";

            using (SqlCommand command = new SqlCommand(selectBook, conn))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Genre genre = new Genre(reader.GetInt32(4), reader.GetString(5));
                        Publisher publisher = new Publisher(reader.GetInt32(6), reader.GetString(7), reader.GetString(8), reader.GetBoolean(9));

                        return new Book(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetBoolean(2),
                            (float)reader.GetDouble(3),
                            genre,
                            publisher
                        );
                    }
                }
            }
            return null;
        }

        public void Save(Book element)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            if (element.ID == 0)
            {
                string insertBookQuery = @"
                INSERT INTO book (title, is_signed, price, genre_id, publisher_id)
                VALUES (@title, @is_signed, @price, @genre_id, @publisher_id);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(insertBookQuery, conn))
                {
                    command.Parameters.AddWithValue("@title", element.Title);
                    command.Parameters.AddWithValue("@is_signed", element.IsSigned);
                    command.Parameters.AddWithValue("@price", element.Price);
                    command.Parameters.AddWithValue("@genre_id", element.Genre.ID);
                    command.Parameters.AddWithValue("@publisher_id", element.Publisher.ID);

                    element.ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            else
            {
                string updateBookQuery = @"
                UPDATE book
                SET title = @title, is_signed = @is_signed, price = @price, genre_id = @genre_id, publisher_id = @publisher_id
                WHERE id = @id";

                using (SqlCommand command = new SqlCommand(updateBookQuery, conn))
                {
                    command.Parameters.AddWithValue("@id", element.ID);
                    command.Parameters.AddWithValue("@title", element.Title);
                    command.Parameters.AddWithValue("@is_signed", element.IsSigned);
                    command.Parameters.AddWithValue("@price", element.Price);
                    command.Parameters.AddWithValue("@genre_id", element.Genre.ID);
                    command.Parameters.AddWithValue("@publisher_id", element.Publisher.ID);

                    command.ExecuteNonQuery();
                }
            }
        
        }

        //Not implemented for this class
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

        public void RemoveIncorrectFormat()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string checkTableSchema = @"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'book')
                BEGIN
                    DECLARE @mismatch BIT = 0;

                    -- Check if 'id' column exists with the correct type and identity property
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'book' AND COLUMN_NAME = 'id' 
                        AND DATA_TYPE = 'int' AND COLUMNPROPERTY(object_id('book'), 'id', 'IsIdentity') = 1
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'title' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'book' AND COLUMN_NAME = 'title' 
                        AND DATA_TYPE = 'varchar' AND CHARACTER_MAXIMUM_LENGTH = 255
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'is_signed' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'book' AND COLUMN_NAME = 'is_signed' 
                        AND DATA_TYPE = 'bit'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'price' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'book' AND COLUMN_NAME = 'price' 
                        AND DATA_TYPE = 'float'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'genre_id' column exists and is linked correctly
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'book' AND COLUMN_NAME = 'genre_id' 
                        AND DATA_TYPE = 'int'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'publisher_id' column exists and is linked correctly
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'book' AND COLUMN_NAME = 'publisher_id' 
                        AND DATA_TYPE = 'int'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Drop table if it does not match the expected format
                    IF @mismatch = 1
                    BEGIN
                        DROP TABLE IF EXISTS purchase;
                        DROP TABLE book;
                    END
                END";

            using (SqlCommand command = new SqlCommand(checkTableSchema, conn))
            {
                command.ExecuteNonQuery();
            }
        }

    }
}