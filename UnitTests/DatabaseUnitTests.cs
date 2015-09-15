using System;
using DatabaseLab.Logging;
using DatabaseLab.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class DatabaseUnitTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Logger.pathToWrite = "./TestLogs.dat";

            Database.CreateDatabaseFolder();
            Database.CreateTable("test",
                new List<Types.Type>() { Types.Type.BOOLEAN, Types.Type.INTEGER, Types.Type.VARCHAR },
                new List<string>() { "boolean", "integer", "varchar" });
        }

        [TestMethod]
        public void TestCreatingFolder()
        {
            Database.CreateDatabaseFolder();

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
            List<Types.Type> types = new List<Types.Type>{ Types.Type.BOOLEAN, Types.Type.INTEGER, Types.Type.VARCHAR };
            Record record = new Record(types);
            bool value = Database.AddRecord("test", record);

            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestSearchingEmptyRecordData()
        {
            List<Types.Type> types = new List<Types.Type> { Types.Type.BOOLEAN, Types.Type.INTEGER, Types.Type.VARCHAR };
            Record record = new Record(types);
            bool value = Database.AddRecord("test", record);

            List<Record> result = Database.Search("test", record);

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void TestSearchingSomeRecordData()
        {
            List<Types.Type> types = new List<Types.Type> { Types.Type.BOOLEAN, Types.Type.INTEGER, Types.Type.VARCHAR };
            Record record1 = new Record(types);
            Record record2 = new Record(types);

            record2.Edit(true, 0);
            record2.Edit(12, 1);
            record2.Edit("testing purpose", 2);

            Database.AddRecord("test", record1);
            Database.AddRecord("test", record2);

            List<Record> result = Database.Search("test", record2);

            Assert.AreEqual(record2, result[0]);
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
