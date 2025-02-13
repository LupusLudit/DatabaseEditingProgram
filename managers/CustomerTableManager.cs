using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;

namespace DatabaseEditingProgram.managers
{
    public class CustomerTableManager : TableManager<Customer>
    {
        public CustomerTableManager() : base(new CustomerDAO()) { }

        protected override void AddNew()
        {
            Customer customer = new Customer("New", "Customer", DateTime.Today);
            DAO.Save(customer);
            Items.Add(customer);
        }

        protected override void Delete(Customer customer)
        {
            if (customer == null) return;
            DAO.Delete(customer);
            Items.Remove(customer);
        }

        protected override void Save(Customer customer)
        {
            if (customer == null) return;
            DAO.Save(customer);
        }
    }
}
