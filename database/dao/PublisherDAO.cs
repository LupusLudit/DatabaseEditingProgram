using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;

namespace DatabaseEditingProgram.database.dao
{
    public class PublisherDAO : IDAO<Publisher>
    {
        public PublisherDAO() { }
        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string createCustomerTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'publisher')
                BEGIN
                    CREATE TABLE publisher (
                        id INT PRIMARY KEY IDENTITY, 
                        name VARCHAR(100),
                        motto VARCHAR(100),
                        active BIT
                    );
                END";

            using (SqlCommand command = new SqlCommand(createCustomerTable, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Delete(Publisher publisher)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("DELETE FROM publisher WHERE id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", publisher.ID);
                command.ExecuteNonQuery();
                publisher.ID = 0;
            }
        }

        public IEnumerable<Publisher> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT * FROM publisher", conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Publisher publisher = new Publisher(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            (reader.GetInt32(3) == 1)
                            );

                        yield return publisher;
                    }
                }
            }
        }

        public Publisher? GetByID(int id)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT * FROM publisher where id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Publisher(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            (reader.GetInt32(3) == 1)
                            );
                    }
                }
            }
            return null;
        }

        public void Save(Publisher publisher)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            if (publisher.ID == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO publisher (name, motto, active) VALUES (@name, @motto, @active); SELECT SCOPE_IDENTITY();", conn))
                {
                    command.Parameters.AddWithValue("@name", publisher.Name);
                    command.Parameters.AddWithValue("@motto", publisher.Motto);
                    command.Parameters.AddWithValue("@active", (publisher.IsActive ? 1 : 0));
                    publisher.ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE publisher SET name = @name, motto = @motto, active = @active WHERE id = @id", conn))
                {
                    command.Parameters.AddWithValue("@id", publisher.ID);
                    command.Parameters.AddWithValue("@name", publisher.Name);
                    command.Parameters.AddWithValue("@motto", publisher.Motto);
                    command.Parameters.AddWithValue("@active", (publisher.IsActive ? 1 : 0));
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
