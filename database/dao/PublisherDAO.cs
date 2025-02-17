using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace DatabaseEditingProgram.database.dao
{
    /// <include file='../../docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="PublisherDAO"]/*'/>
    public class PublisherDAO : IDAO<Publisher>
    {
        public PublisherDAO()
        {
            CreateTable();
        }

        /// <summary>
        /// Creates the publisher table if it does not exist.
        /// </summary>
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

        /// <summary>
        /// Deletes a publisher from the database.
        /// </summary>
        /// <param name="publisher">The publisher to delete.</param>
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
        /// <summary>
        /// Retrieves all publishers from the database.
        /// </summary>
        /// <returns>A collection of publishers.</returns>
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

        /// <summary>
        /// Retrieves a publisher based on its ID.
        /// </summary>
        /// <param name="id">The publisher ID.</param>
        /// <returns>The publisher if found (otherwise null).</returns>
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

        /// <summary>
        /// Saves a publisher to the database. If the publisher does not exist, it is inserted. If it does, it is updated.
        /// </summary>
        /// <param name="publisher">The publisher to save.</param>
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

        /// <summary>
        /// Checks if forbidden tables contain any records.
        /// </summary>
        /// <returns>True if forbidden tables are not empty (otherwise false).</returns>
        public bool ForbiddenTablesNotEmpty()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string checkEmptyTablesQuery = @"
                SELECT 
                    (SELECT COUNT(*) FROM book) + 
                    (SELECT COUNT(*) FROM purchase)";

            using (SqlCommand command = new SqlCommand(checkEmptyTablesQuery, conn))
            {
                int rowCount = (int)command.ExecuteScalar();
                return rowCount > 0;
            }
        }

        /// <summary>
        /// Exports the publisher data to a CSV file.
        /// </summary>
        /// <param name="filePath">The file path to save the CSV.</param>
        /// <exception cref="ArgumentException">Is thrown when exported data contain commas.</exception>
        public void ExportToCsv(string filePath)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            string commaPattern = @",.*|.*,";
            Regex regex = new Regex(commaPattern);
            try
            {
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
                                if (regex.IsMatch(name) || regex.IsMatch(motto))
                                {
                                    throw new ArgumentException("When exporting data, no text fields must contain a comma");
                                }
                                else writer.WriteLine($"{id},{name},{motto},{active}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Imports the publisher data from a CSV file.
        /// </summary>
        /// <param name="filePath">The file path to load the CSV.</param>
        /// <exception cref="FormatException">Is thrown when the CSV file being imported does not meet necessary requirements.</exception>
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

                        if (!values[3].Trim().Equals("True", StringComparison.OrdinalIgnoreCase)
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

        /// <summary>
        /// Ensures the publisher table follows the expected schema. If it does not, the table gets deleted.
        /// Serves as a security check - if there is already a table with the name "publisher" that does not fit
        /// our schema and we do not drop it, it might cause some issues.
        /// </summary>
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
