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


        public DatabaseViewModel()
        {
            GenreManager = new GenreTableManager();
        }
    }
}
