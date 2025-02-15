using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;

namespace DatabaseEditingProgram.database.dao
{
    public class PublisherDAO : IDAO<Publisher>
    {
        public PublisherDAO()
        {
            CreateTable();
        }
        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            RemoveIncorrectFormat();

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
                            reader.GetBoolean(3)
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
                            reader.GetBoolean(3)
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
                    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'publisher')
                    BEGIN
                        DECLARE @mismatch BIT = 0;

                        -- Check if 'id' column exists with correct type and is an identity column
                        IF NOT EXISTS (
                            SELECT 1 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'publisher' AND COLUMN_NAME = 'id' 
                            AND DATA_TYPE = 'int' 
                            AND COLUMNPROPERTY(object_id('publisher'), 'id', 'IsIdentity') = 1
                        )
                        BEGIN
                            SET @mismatch = 1;
                        END

                        -- Check if 'name' column exists with correct type
                        IF NOT EXISTS (
                            SELECT 1 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'publisher' AND COLUMN_NAME = 'name' 
                            AND DATA_TYPE = 'varchar'
                        )
                        BEGIN
                            SET @mismatch = 1;
                        END

                        -- Check if 'motto' column exists with correct type
                        IF NOT EXISTS (
                            SELECT 1 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'publisher' AND COLUMN_NAME = 'motto' 
                            AND DATA_TYPE = 'varchar'
                        )
                        BEGIN
                            SET @mismatch = 1;
                        END

                        -- Check if 'active' column exists with correct type
                        IF NOT EXISTS (
                            SELECT 1 
                            FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'publisher' AND COLUMN_NAME = 'active' 
                            AND DATA_TYPE = 'bit'
                        )
                        BEGIN
                            SET @mismatch = 1;
                        END

                        -- Drop table if it does not match expected format
                        IF @mismatch = 1
                        BEGIN
                            DROP TABLE IF EXISTS purchase;
                            DROP TABLE IF EXISTS book;
                            DROP TABLE publisher;
                        END
                    END";

            using (SqlCommand command = new SqlCommand(checkTableSchema, conn))
            {
                command.ExecuteNonQuery();
            }
        }


    }
}
