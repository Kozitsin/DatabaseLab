using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLab.Database;
using System.Collections.Generic;
using DatabaseLab.Logging;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class RecordUnitTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Logger.pathToWrite = "./TestLog.dat";
        }

        [TestMethod]
        public void RecordCreationWithAllTypes()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.BOOLEAN, Types.Type.INTEGER, Types.Type.VARCHAR };
            Record record = new Record(types);

            Assert.AreEqual(3 ,record.data.Count);
        }

        [TestMethod]
        public void RecordCreationWithNoTypes()
        {
            List<Types.Type> types = new List<Types.Type>();
            Record record = new Record(types);

            Assert.AreEqual(0, record.data.Count);
        }

        [TestMethod]
        public void RecordCreationWithSeveralEqualTypes()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.BOOLEAN, Types.Type.INTEGER, Types.Type.VARCHAR, Types.Type.BOOLEAN };
            Record record = new Record(types);

            Assert.AreEqual(4, record.data.Count);
        }

        [TestMethod]
        public void RecordEditOneField()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.INTEGER, Types.Type.VARCHAR };
            Record record = new Record(types);

            record.Edit(3, 0);

            Assert.AreEqual(3, record.data[0]);
        }

        [TestMethod]
        public void RecordEditOutOfRange()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.INTEGER, Types.Type.VARCHAR };
            Record record = new Record(types);

            bool res = record.Edit(1, 2);

            Assert.AreEqual(false, res);
        }
        
        [TestMethod]
        public void RecordEditIntAsDoubleFail()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.INTEGER, Types.Type.VARCHAR };
            Record record = new Record(types);

            bool res = record.Edit(1.23, 0);

            Assert.AreNotEqual(true, res);
        }

        [TestMethod]
        public void RecordEditIntAsDoubleWithCast()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.INTEGER, Types.Type.VARCHAR };
            Record record = new Record(types);

            bool res = record.Edit((int)1.23, 0);

            Assert.AreEqual(true, res);
        }

        [TestCleanup]
        public void CleanUp()
        {
            try
            {
                File.Delete("./TestLog.dat");
                Logger.pathToWrite = "Logs.dat";
            }
            catch (Exception)
            {

            }
        }
    }
}
