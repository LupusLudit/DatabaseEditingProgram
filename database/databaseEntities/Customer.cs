namespace DatabaseEditingProgram.database.databaseEntities
{
    public class Customer : IDatabaseEntity
    {
        private int id;
        private string name;
        private string surname;
        private DateTime dateOfBirth;


        public int ID { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Surname { get { return surname; } set { surname = value; } }
        public DateTime DateOfBirth { get { return dateOfBirth; } set { dateOfBirth = value; } }

        public Customer(int id, string name, string surname, DateTime dateOfBirth)
        {
            this.id = id;
            this.name = name;
            this.surname = surname;
            this.dateOfBirth = dateOfBirth;
        }

        public Customer(string name, string surname, DateTime dateOfBirth)
        {
            this.id = 0;
            this.name = name;
            this.surname = surname;
            this.dateOfBirth = dateOfBirth;
        }
    }
}
