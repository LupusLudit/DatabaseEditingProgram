using DatabaseEditingProgram.managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEditingProgram
{
    class DatabaseViewModel
    {
        public GenreTableManager GenreManager { get; }
        public PublisherTableManager PublisherManager { get; }

        public CustomerTableManager CustomerManager { get; }

        public List<bool> BooleanOptions { get; } = new() { true, false };

        public DatabaseViewModel()
        {
            GenreManager = new GenreTableManager();
            PublisherManager = new PublisherTableManager();
            CustomerManager = new CustomerTableManager();
        }
    }
}