using System.Collections.Generic;

namespace DatabaseLab.Database
{
    public interface IDatabase
    {
        void CreateDatabaseFolder();

        bool CreateTable(string tableName, List<Types.Type> types, List<string> headers);

        bool AddRecord(string tableName, Record record);

        List<Record> Search(string tableName, Record record);

        bool DeleteRecord(string tableName, Record record);

        bool Update(string tableName, Record original, Record modified);

        bool BackUp(string tableName);

        bool Restore(string path);

        bool Import(string tableName);
    }
}
