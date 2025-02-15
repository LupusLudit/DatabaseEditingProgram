﻿using DatabaseEditingProgram.database.databaseEntities;
/*
 * Note: This code was provided
 * Note 2: Beware that the classes implementing this interface might be similar to the classes in the provided code
 */

namespace DatabaseEditingProgram.database.dao
{
    public interface IDAO<T> where T: IDatabaseEntity
    { 
        void CreateTable();
        void RemoveIncorrectFormat();
        T? GetByID(int id);
        IEnumerable<T> GetAll();
        void Save(T element);
        void Delete(T element);
        void ExportToCsv(string filePath);
        void ImportFromCsv(string filePath);
    }
}
