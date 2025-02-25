﻿using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace DatabaseEditingProgram.database.dao
{
    /// <include file='../../docs/DatabaseProgramDocs.xml' path='MyDocs/MyMembers[@name="CustomerDAO"]/*'/>
    public class CustomerDAO : IDAO<Customer>
    {

        public CustomerDAO()
        {
            CreateTable();
        }

        /// <summary>
        /// Creates the customer table if it does not exist.
        /// </summary>
        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            RemoveIncorrectFormat();

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

        /// <summary>
        /// Deletes a customer from the database.
        /// </summary>
        /// <param name="customer">The customer to delete.</param>
        public void Delete(Customer customer)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("DELETE FROM customer WHERE id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", customer.ID);
                command.ExecuteNonQuery();
                customer.ID = 0;
            }
        }

        //I have chosen a different approach here, since yield returning causes an issue when importing data to database
        /// <summary>
        /// Retrieves all customers from the database.
        /// </summary>
        /// <returns>A collection of customers.</returns>
        public IEnumerable<Customer> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            List<Customer> customers = new List<Customer>();

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
                        customers.Add(customer);
                    }
                 }
            }
            return customers;
        }

        /// <summary>
        /// Retrieves a customer based on its ID.
        /// </summary>
        /// <param name="id">The customer ID.</param>
        /// <returns>The customer if found (otherwise null).</returns>
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

        /// <summary>
        /// Saves a customer to the database. If the customer does not exist, it is inserted. If it does, it is updated.
        /// </summary>
        /// <param name="customer">The book to save.</param>
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
        /// Exports the customer data to a CSV file.
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
                using (SqlCommand command = new SqlCommand("SELECT id, name, surname, date_of_birth FROM customer", conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                        {
                            // Header
                            writer.WriteLine("id,name,surname,date_of_birth");

                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                string surname = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                string dateOfBirth = reader.IsDBNull(3) ? "" : reader.GetDateTime(3).ToString("yyyy-MM-dd");
                                if (regex.IsMatch(name) || regex.IsMatch(surname) || regex.IsMatch(dateOfBirth))
                                {
                                    throw new ArgumentException("When exporting data, no text fields must contain a comma");
                                }
                                else writer.WriteLine($"{id},{name},{surname},{dateOfBirth}");
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
        /// Imports the customer data from a CSV file.
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

                        int id = int.Parse(values[0].Trim());
                        string name = values[1].Trim();
                        string surname = values[2].Trim();
                        DateTime dateOfBirth = DateTime.Parse(values[3].Trim());

                        using (SqlCommand checkRecordExistenceCommand = new SqlCommand("SELECT COUNT(*) FROM customer WHERE id = @id", conn))
                        {
                            checkRecordExistenceCommand.Parameters.AddWithValue("@id", id);
                            int count = (int)checkRecordExistenceCommand.ExecuteScalar();
                            string query;

                            if (count > 0)
                            {
                                query = @"UPDATE customer
                                         SET name = @name, surname = @surname, date_of_birth = @date_of_birth
                                         WHERE id = @id";

                                using (SqlCommand updateCommand = new SqlCommand(query, conn))
                                {
                                    updateCommand.Parameters.AddWithValue("@id", id);
                                    updateCommand.Parameters.AddWithValue("@name", name);
                                    updateCommand.Parameters.AddWithValue("@surname", surname);
                                    updateCommand.Parameters.AddWithValue("@date_of_birth", dateOfBirth);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                query = @"INSERT INTO customer
                                        (name, surname, date_of_birth)
                                        VALUES (@name, @surname, @date_of_birth)";

                                using (SqlCommand insertCommand = new SqlCommand(query, conn))
                                {
                                    insertCommand.Parameters.AddWithValue("@name", name);
                                    insertCommand.Parameters.AddWithValue("@surname", surname);
                                    insertCommand.Parameters.AddWithValue("@date_of_birth", dateOfBirth);
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
        /// Ensures the customer table follows the expected schema. If it does not, the table gets deleted.
        /// Serves as a security check - if there is already a table with the name "customer" that does not fit
        /// our schema and we do not drop it, it might cause some issues.
        /// </summary>
        public void RemoveIncorrectFormat()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();
            string checkTableSchema = @"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'customer')
                BEGIN
                    DECLARE @mismatch BIT = 0;

                    -- Check if 'id' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'customer' AND COLUMN_NAME = 'id' 
                        AND DATA_TYPE = 'int' AND COLUMNPROPERTY(object_id('customer'), 'id', 'IsIdentity') = 1
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'name' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'customer' AND COLUMN_NAME = 'name' 
                        AND DATA_TYPE = 'varchar' AND CHARACTER_MAXIMUM_LENGTH = 100
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'surname' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'customer' AND COLUMN_NAME = 'surname' 
                        AND DATA_TYPE = 'varchar' AND CHARACTER_MAXIMUM_LENGTH = 100
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Check if 'date_of_birth' column exists with the correct type
                    IF NOT EXISTS (
                        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'customer' AND COLUMN_NAME = 'date_of_birth' 
                        AND DATA_TYPE = 'date'
                    )
                    BEGIN
                        SET @mismatch = 1;
                    END

                    -- Drop table if it does not match expected format
                    IF @mismatch = 1
                    BEGIN
                        DROP TABLE IF EXISTS purchase;
                        DROP TABLE customer;
                    END
                END";

            using (SqlCommand command = new SqlCommand(checkTableSchema, conn))
            {
                command.ExecuteNonQuery();
            }
        }

    }
}
