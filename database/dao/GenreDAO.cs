using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEditingProgram.database.dao
{
    public class GenreDAO : IDAO<Genre>
    {

        public GenreDAO() { }
        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string createCustomerTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'genre')
                BEGIN
                    CREATE TABLE genre (
                        id INT PRIMARY KEY IDENTITY, 
                        name VARCHAR(100)
                    );
                END";

            using (SqlCommand command = new SqlCommand(createCustomerTable, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Delete(Genre element)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Genre> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

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

                        yield return genre;
                    }
                }
            }
        }

        public Genre? GetByID(int id)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            Genre? genre = null;

            using (SqlCommand command = new SqlCommand("SELECT * FROM genre where id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        genre = new databaseEntities.Genre(
                             reader.GetInt32(0),
                             reader.GetString(1)
                            );
                    }
                }
            }
            return genre;
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
    }
}
