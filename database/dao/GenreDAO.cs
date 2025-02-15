using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;


namespace DatabaseEditingProgram.database.dao
{
    public class GenreDAO : IDAO<Genre>
    {

        public GenreDAO()
        {
            CreateTable();
        }
        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            RemoveIncorrectFormat();

            string createGenreTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'genre')
                BEGIN
                    CREATE TABLE genre (
                        id INT PRIMARY KEY IDENTITY, 
                        name VARCHAR(100)
                    );
                END";

            using (SqlCommand command = new SqlCommand(createGenreTable, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Delete(Genre genre)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("DELETE FROM genre WHERE id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", genre.ID);
                command.ExecuteNonQuery();
                genre.ID = 0;
            }
        }

        //I have chosen a different approach here, since yield returning causes an issue when importing data to database
        public IEnumerable<Genre> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            List<Genre> genres = new List<Genre>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM genre", conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Genre genre = new Genre(
                             reader.GetInt32(0),
                             reader.GetString(1)
                            );

                        genres.Add(genre);
                    }
                }
            }
            return genres;
        }

        public Genre? GetByID(int id)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT * FROM genre where id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return new Genre(
                             reader.GetInt32(0),
                             reader.GetString(1)
                            );
                    }
                }
            }
            return null;
        }

        public void Save(Genre genre)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            if (genre.ID == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO genre (name) VALUES (@name); SELECT SCOPE_IDENTITY();", conn))
                {
                    command.Parameters.AddWithValue("@name", genre.Name);
                    genre.ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE genre SET name = @name WHERE id = @id", conn))
                {
                    command.Parameters.AddWithValue("@id", genre.ID);
                    command.Parameters.AddWithValue("@name", genre.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

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



        public void ExportToCsv(string filePath)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT id, name FROM genre", conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        // Header
                        writer.WriteLine("id,name");

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.IsDBNull(1) ? "" : reader.GetString(1);

                            writer.WriteLine($"{id},{name}");
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
                        if (values.Length != 2) throw new FormatException("Incorrect CSV format");

                        int id = int.Parse(values[0].Trim());
                        string name = values[1].Trim();

                        using (SqlCommand checkRecordExistenceCommand = new SqlCommand("SELECT COUNT(*) FROM genre WHERE id = @id", conn))
                        {
                            checkRecordExistenceCommand.Parameters.AddWithValue("@id", id);
                            int count = (int)checkRecordExistenceCommand.ExecuteScalar();
                            string query;

                            if (count > 0)
                            {
                                query = @"UPDATE genre
                                         SET name = @name
                                         WHERE id = @id";

                                using (SqlCommand updateCommand = new SqlCommand(query, conn))
                                {
                                    updateCommand.Parameters.AddWithValue("@id", id);
                                    updateCommand.Parameters.AddWithValue("@name", name);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                query = @"INSERT INTO genre
                                        (name)
                                        VALUES (@name)";

                                using (SqlCommand insertCommand = new SqlCommand(query, conn))
                                {
                                    insertCommand.Parameters.AddWithValue("@name", name);
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
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'genre')
                BEGIN
                    DECLARE @mismatch BIT = 0;

                    -- Check if 'id' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'genre' AND COLUMN_NAME = 'id' 
                        AND DATA_TYPE = 'int' AND COLUMNPROPERTY(object_id('genre'), 'id', 'IsIdentity') = 1
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'name' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'genre' AND COLUMN_NAME = 'name' 
                        AND DATA_TYPE = 'varchar' AND CHARACTER_MAXIMUM_LENGTH = 100
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Drop table if it does not match expected format
                    IF @mismatch = 1
                    BEGIN
                        DROP TABLE IF EXISTS purchase;
                        DROP TABLE IF EXISTS book;
                        DROP TABLE genre;
                    END
                END";

            using (SqlCommand command = new SqlCommand(checkTableSchema, conn))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
