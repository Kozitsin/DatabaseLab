using System.Collections.Generic;

namespace DatabaseLab.DataBase
{
    public interface IDatabase
    {
        void CreateDatabaseFolder();

        bool CreateTable(string tableName, List<Types.Type> types, List<string> headers);

        bool AddRecord(string tableName, Record record);

        List<Record> Search(string tableName, string s);

        bool DeleteRecord(string tableName, string s);

        bool Update(string tableName, Record original, Record modified);

        bool BackUp(string tableName);

        bool Restore(string tableName);

        bool Import(string tableName);
    }
}
