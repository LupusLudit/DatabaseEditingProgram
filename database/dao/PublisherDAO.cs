using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;

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

            string createPublisherTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'publisher')
                BEGIN
                    CREATE TABLE publisher (
                        id INT PRIMARY KEY IDENTITY, 
                        name VARCHAR(100),
                        motto VARCHAR(100),
                        active BIT
                    );
                END";

            using (SqlCommand command = new SqlCommand(createPublisherTable, conn))
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

        //I have chosen a different approach here, since yield returning causes an issue when importing data to database
        public IEnumerable<Publisher> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            List<Publisher> publishers = new List<Publisher>();

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

                        publishers.Add(publisher);
                    }
                }
            }
            return publishers;
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
        public void ExportToCsv(string filePath)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT id, name, motto, active FROM publisher", conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        // Header
                        writer.WriteLine("id,name,motto,active");

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            string motto = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            bool active = reader.GetBoolean(3);

                            writer.WriteLine($"{id},{name},{motto},{active}");
                        }
                    }
                }
            }
        }

        public void ImportFromCsv(string filePath)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            try
            {
                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    string? line;
                    bool firstLine = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (firstLine)
                        {
                            firstLine = false;
                            continue;
                        }

                        string[] values = line.Split(',');
                        if (values.Length != 4) throw new FormatException("Incorrect CSV format");
                        else if (!values[3].Trim().Equals("True", StringComparison.OrdinalIgnoreCase)
                            && !values[3].Trim().Equals("False", StringComparison.OrdinalIgnoreCase))
                        {
                            throw new ArgumentException("File is missing a boolean value for IsActive");
                        }

                        int id = int.Parse(values[0].Trim());
                        string name = values[1].Trim();
                        string motto = values[2].Trim();
                        bool active = values[3].Trim().Equals("True", StringComparison.OrdinalIgnoreCase);

                        using (SqlCommand checkRecordExistenceCommand = new SqlCommand("SELECT COUNT(*) FROM publisher WHERE id = @id", conn))
                        {
                            checkRecordExistenceCommand.Parameters.AddWithValue("@id", id);
                            int count = (int)checkRecordExistenceCommand.ExecuteScalar();
                            string query;

                            if (count > 0)
                            {
                                query = @"UPDATE publisher
                                         SET name = @name, motto = @motto, active = @active
                                         WHERE id = @id";

                                using (SqlCommand updateCommand = new SqlCommand(query, conn))
                                {
                                    updateCommand.Parameters.AddWithValue("@id", id);
                                    updateCommand.Parameters.AddWithValue("@name", name);
                                    updateCommand.Parameters.AddWithValue("@motto", motto);
                                    updateCommand.Parameters.AddWithValue("@active", active ? 1 : 0);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                query = @"INSERT INTO publisher
                                        (name, motto, active)
                                        VALUES (@name, @motto, @active)";

                                using (SqlCommand insertCommand = new SqlCommand(query, conn))
                                {
                                    insertCommand.Parameters.AddWithValue("@name", name);
                                    insertCommand.Parameters.AddWithValue("@motto", motto);
                                    insertCommand.Parameters.AddWithValue("@active", active ? 1 : 0);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There has been an error while trying to import from a file. Exception: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
