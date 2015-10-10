using System;
using DatabaseLab.Logging;
using DatabaseLab.DataBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class DatabaseUnitTests
    {
        Database DB = new Database();

        [TestInitialize]
        public void Initialize()
        {
            Logger.pathToWrite = "./TestLogs.dat";

            DB.CreateDatabaseFolder();
            DB.CreateTable("test",
                new List<Types.Type>() { Types.Type.VARCHAR, Types.Type.BOOLEAN, Types.Type.INTEGER },
                new List<string>() { "varchar", "boolean", "integer" });
        }

        [TestMethod]
        public void TestCreatingFolder()
        {
            DB.CreateDatabaseFolder();

            bool value = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + '/' + "Data");

            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestCreatingTable()
        {
            bool value = File.Exists(AppDomain.CurrentDomain.BaseDirectory + '/' + "Data/test.dat");
            value &= File.Exists(AppDomain.CurrentDomain.BaseDirectory + '/' + "Data/test_hash.dat");
            value &= File.Exists(AppDomain.CurrentDomain.BaseDirectory + '/' + "Data/test_headers.dat");
            value &= File.Exists(AppDomain.CurrentDomain.BaseDirectory + '/' + "Data/test_freespace.dat");

            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestAddingData()
        {
            List<Types.Type> types = new List<Types.Type>{ Types.Type.VARCHAR, Types.Type.BOOLEAN, Types.Type.INTEGER };
            Record record = new Record(types);
            bool value = DB.AddRecord("test", record);

            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestSearchingEmptyRecordData()
        {
            List<Types.Type> types = new List<Types.Type> { Types.Type.VARCHAR, Types.Type.BOOLEAN, Types.Type.INTEGER };
            Record record = new Record(types);
            bool value = DB.AddRecord("test", record);

            List<Record> result = DB.Search("test", record.data[0].ToString());

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestSearchingSomeRecordData()
        {
            List<Types.Type> types = new List<Types.Type> { Types.Type.VARCHAR, Types.Type.BOOLEAN, Types.Type.INTEGER };
            Record record1 = new Record(types);
            Record record2 = new Record(types);

            record2.Edit("testing purpose", 0);
            record2.Edit(true, 1);
            record2.Edit(12, 2);

            DB.AddRecord("test", record1);
            DB.AddRecord("test", record2); 

            List<Record> result = DB.Search("test", record2.data[0].ToString());

            Assert.AreEqual(record2, result[0]);
        }

        [TestMethod]
        public void TestDeleteRecord()
        {
            List<Types.Type> types = new List<Types.Type> { Types.Type.VARCHAR, Types.Type.BOOLEAN, Types.Type.INTEGER };
            Record record = new Record(types);

            record.Edit("testing purpose", 0);
            record.Edit(true, 1);
            record.Edit(12, 2);

            DB.AddRecord("test", record);

            bool value = DB.DeleteRecord("test", record.data[0].ToString());

            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestDeleteFictionalRecord()
        {
            List<Types.Type> types = new List<Types.Type> { Types.Type.VARCHAR, Types.Type.BOOLEAN, Types.Type.INTEGER };
            Record record1 = new Record(types);
            Record record2 = new Record(types);

            record2.Edit("testing purpose", 0);
            record2.Edit(true, 1);
            record2.Edit(12, 2);

            DB.AddRecord("test", record2);

            bool value = DB.DeleteRecord("test", record1.data[0].ToString());

            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void TestImportToCSV()
        {
            List<Types.Type> types = new List<Types.Type> { Types.Type.VARCHAR, Types.Type.BOOLEAN, Types.Type.INTEGER };
            Record record1 = new Record(types);
            Record record2 = new Record(types);

            record1.Edit("first record", 0);
            record1.Edit(false, 1);
            record1.Edit(123, 2);

            record2.Edit("testing purpose", 0);
            record2.Edit(true, 1);
            record2.Edit(12, 2);

            DB.AddRecord("test", record1);
            DB.AddRecord("test", record2);

            bool value = DB.Import("test");
            Assert.AreEqual(true, value);
        }

        [TestCleanup]
        public void CleanUp()
        {
            try
            {
                File.Delete("./TestLog.dat");
                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + '/' + "Data", true);

                Logger.pathToWrite = "Logs.dat";

            }
            catch (Exception)
            {

            }
        }
    }
}
