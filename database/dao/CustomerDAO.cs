using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;
using System.Security.Policy;

namespace DatabaseEditingProgram.database.dao
{
    public class CustomerDAO : IDAO<Customer>
    {

        public CustomerDAO() { }
        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string createCustomerTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'customer')
                BEGIN
                    CREATE TABLE customer (
                        id INT PRIMARY KEY IDENTITY, 
                        name VARCHAR(100), 
                        surname VARCHAR(100),
                        date_of_birth DATE
                    );
                END";

            using (SqlCommand command = new SqlCommand(createCustomerTable, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Delete(Customer element)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT * FROM customer", conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Customer customer = new Customer(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetDateTime(3)
                            );

                        yield return customer;
                    }
                }
            }
        }

        public Customer? GetByID(int id)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            Customer? customer = null;

            using (SqlCommand command = new SqlCommand("SELECT * FROM customer where id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customer = new Customer(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetDateTime(3)
                            );
                    }
                }
            }
            return customer;
        }

        public void Save(Customer customer)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            if (customer.ID == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO customer (name, surname, date_of_birth) VALUES (@name, @surname, @date_of_birth); SELECT SCOPE_IDENTITY();", conn))
                {
                    command.Parameters.AddWithValue("@name", customer.Name);
                    command.Parameters.AddWithValue("@surname", customer.Surname);
                    command.Parameters.AddWithValue("@date_of_birth", customer.DateOfBirth);
                    customer.ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE customer SET name = @name, surname = @surname, date_of_birth = @date_of_birth WHERE id = @id", conn))
                {
                    command.Parameters.AddWithValue("@id", customer.ID);
                    command.Parameters.AddWithValue("@name", customer.Name);
                    command.Parameters.AddWithValue("@surname", customer.Surname);
                    command.Parameters.AddWithValue("@date_of_birth", customer.DateOfBirth);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
