using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;

namespace DatabaseEditingProgram.managers
{
    public class PublisherTableManager : TableManager<Publisher>
    {
        public PublisherTableManager() : base(new PublisherDAO()) { }

        protected override void AddNew()
        {
            Publisher publisher = new Publisher("New Publisher", "New Motto", false);
            DAO.Save(publisher);
            Items.Add(publisher);
        }

        protected override void Delete(Publisher publisher)
        {
            if (publisher == null) return;
            DAO.Delete(publisher);
            Items.Remove(publisher);
        }

        protected override void Save(Publisher publisher)
        {
            if (publisher == null) return;
            DAO.Save(publisher);
        }
    }
}
