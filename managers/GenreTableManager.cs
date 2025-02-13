using DatabaseEditingProgram.database.dao;
using DatabaseEditingProgram.database.databaseEntities;

namespace DatabaseEditingProgram.managers
{
    public class GenreTableManager : TableManager<Genre>
    {
        public GenreTableManager() : base(new GenreDAO()) {}

        protected override void Save(Genre genre)
        {
            if (genre == null) return;
            DAO.Save(genre);
        }

        protected override void Delete(Genre genre)
        {
            if (genre == null) return;
            DAO.Delete(genre);
            Items.Remove(genre);
        }

        //TODO: fix adding a new genre

        protected override void AddNew()
        {
            Genre newGenre = new Genre("New Genre");
            DAO.Save(newGenre);
            Items.Add(newGenre);
        }
    }

}
