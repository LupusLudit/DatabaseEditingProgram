using DatabaseEditingProgram.database.databaseEntities;
using Microsoft.Data.SqlClient;
using System.Security.Policy;

namespace DatabaseEditingProgram.database.dao
{
    public class OrderDAO : IDAO<Order>
    {
        public OrderDAO()
        {
            CreateTable();
        }

        public void CreateTable()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            string createOrderTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'order')
                BEGIN
                    CREATE TABLE order (
                        id INT PRIMARY KEY IDENTITY,
                        customer_id INT,
                        book_id INT,
                        surcharge FLOAT,
                        order_date DATE,
                        order_time TIME,
                        FOREIGN KEY (customer_id) REFERENCES customer(id) ON DELETE CASCADE,
                        FOREIGN KEY (book_id) REFERENCES book(id) ON DELETE CASCADE
                    );
                END";

            using (SqlCommand command = new SqlCommand(createOrderTable, conn))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Delete(Order order)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("DELETE FROM order WHERE id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", order.ID);
                command.ExecuteNonQuery();
                order.ID = 0;
            }
        }

        public IEnumerable<Order> GetAll()
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT * FROM order", conn))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int customerID = reader.GetInt32(1);
                        int bookID = reader.GetInt32(2);

                        Customer customer = new CustomerDAO().GetByID(customerID);
                        Book book = new BookDAO().GetByID(bookID);

                        Order order = new Order(
                            reader.GetInt32(0),
                            customer,
                            book,
                            reader.GetFloat(3),
                            reader.GetDateTime(4),
                            reader.GetDateTime(5)
                        );

                        yield return order;
                    }
                }
            }
        }

        public Order? GetByID(int id)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            using (SqlCommand command = new SqlCommand("SELECT * FROM order WHERE id = @id", conn))
            {
                command.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int customerID = reader.GetInt32(1);
                        int bookID = reader.GetInt32(2);

                        Customer customer = new CustomerDAO().GetByID(customerID);
                        Book book = new BookDAO().GetByID(bookID);

                        return new Order(
                            reader.GetInt32(0),
                            customer,
                            book,
                            reader.GetFloat(3),
                            reader.GetDateTime(4),
                            reader.GetDateTime(5)
                        );
                    }
                }
            }
            return null;
        }

        public void Save(Order order)
        {
            SqlConnection conn = DatabaseSingleton.GetInstance();

            if (order.ID == 0)
            {
                string insertOrderQuery = @"
                INSERT INTO order (customer_id, book_id, surcharge, order_date, order_time)
                VALUES (@customer_id, @book_id, @surcharge, @order_date, @order_time);
                SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(insertOrderQuery, conn))
                {
                    command.Parameters.AddWithValue("@customer_id", order.Customer.ID);
                    command.Parameters.AddWithValue("@book_id", order.Book.ID);
                    command.Parameters.AddWithValue("@surcharge", order.Surcharge);
                    command.Parameters.AddWithValue("@order_date", order.Date);
                    command.Parameters.AddWithValue("@order_time", order.Time);

                    order.ID = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            else
            {
                string updateOrderQuery = @"
                UPDATE order
                SET customer_id = @customer_id, book_id = @book_id, surcharge = @surcharge, order_date = @order_date, order_time = @order_time
                WHERE id = @id";

                using (SqlCommand command = new SqlCommand(updateOrderQuery, conn))
                {
                    command.Parameters.AddWithValue("@id", order.ID);
                    command.Parameters.AddWithValue("@customer_id", order.Customer.ID);
                    command.Parameters.AddWithValue("@book_id", order.Book.ID);
                    command.Parameters.AddWithValue("@surcharge", order.Surcharge);
                    command.Parameters.AddWithValue("@order_date", order.Date);
                    command.Parameters.AddWithValue("@order_time", order.Time);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
