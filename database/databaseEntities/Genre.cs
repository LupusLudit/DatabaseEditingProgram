using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEditingProgram.database.databaseEntities
{
    public class Genre : IDatabaseEntity
    {
        private int id;
        private string name;

        public int ID { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; } }

        public Genre(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public Genre(string name)
        {
            this.id = 0;
            this.name = name;
        }
    }
}
