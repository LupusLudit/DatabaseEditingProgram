using DatabaseEditingProgram.database.databaseEntities;
//Note: This code was provided

namespace DatabaseEditingProgram.database.dao
{
    public interface IDAO<T> where T: IDatabaseEntity
    {
        void CreateTable();
        T? GetByID(int id);
        IEnumerable<T> GetAll();
        void Save(T element);
        void Delete(T element);

    }
}
