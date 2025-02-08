using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;

namespace DatabaseEditingProgram.database.dao
{
    public class BookDAO: IDAO<Book>
    {
        public BookDAO() { }

        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

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

            using (SqlCommand command = new SqlCommand("SELECT * FROM book", conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int genreID = reader.GetInt32(4);
                        int publisherID = reader.GetInt32(5);

                        Genre genre = new GenreDAO().GetByID(genreID);
                        Publisher publisher = new PublisherDAO().GetByID(publisherID);

                        Book book = new Book(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetBoolean(2),
                            reader.GetFloat(3),
                            genre,
                            publisher
                        );

                        yield return book;
                    }
                }
            }
        }


        public Book? GetByID(int id)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT * FROM book", conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int genreID = reader.GetInt32(4);
                        int publisherID = reader.GetInt32(5);

                        Genre genre = new GenreDAO().GetByID(genreID);
                        Publisher publisher = new PublisherDAO().GetByID(publisherID);

                        return new Book(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetBoolean(2),
                            reader.GetFloat(3),
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

    }
}
