
namespace DatabaseEditingProgram.database.databaseEntities
{
    public class Publisher : IDatabaseEntity
    {
        private int id;
        private string name;
        private string motto;
        private bool isActive;

        public int ID { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Motto { get { return motto; } set { motto = value; } }
        public bool IsActive { get { return isActive; } set { isActive = value; } }


        public Publisher(int id, string name, string motto, bool isActive)
        {
            this.id = id;
            this.name = name;
            this.motto = motto;
            this.isActive = isActive;
        }

        public Publisher(string name, string motto, bool isActive)
        {
            this.id = 0;
            this.name = name;
            this.motto = motto;
            this.isActive = isActive;
        }

        public override string? ToString()
        {
            return $"{Name}";
        }
    }
}
