using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLab.Database;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class RecordUnitTests
    {
        [TestMethod]
        public void RecordCreationWithAllTypes()
        {
            List<Types> types = new List<Types>() { Types.BOOLEAN, Types.DOUBLE, Types.FLOAT, Types.INTEGER, Types.VARCHAR };
            Record record = new Record(types);

            Assert.AreEqual(5 ,record.data.Count);
        }

        [TestMethod]
        public void RecordCreationWithNoTypes()
        {
            List<Types> types = new List<Types>();
            Record record = new Record(types);

            Assert.AreEqual(0, record.data.Count);
        }

        [TestMethod]
        public void RecordCreationWithSeveralEqualTypes()
        {
            List<Types> types = new List<Types>() { Types.BOOLEAN, Types.DOUBLE, Types.FLOAT, Types.INTEGER, Types.VARCHAR, Types.BOOLEAN };
            Record record = new Record(types);

            Assert.AreEqual(6, record.data.Count);
        }

        [TestMethod]
        public void RecordEditOneField()
        {
            List<Types> types = new List<Types>() { Types.DOUBLE, Types.VARCHAR };
            Record record = new Record(types);

            record.Edit(3.4, 0);

            Assert.AreEqual(3.4, record.data[0]);
        }

        [TestMethod]
        public void RecordEditOutOfRange()
        {
            List<Types> types = new List<Types>() { Types.DOUBLE, Types.VARCHAR };
            Record record = new Record(types);

            bool res = record.Edit(0.1, 2);

            Assert.AreEqual(false, res);
        }
        
        [TestMethod]
        public void RecordEditIntAsDoubleFail()
        {
            List<Types> types = new List<Types>() { Types.DOUBLE, Types.VARCHAR };
            Record record = new Record(types);

            bool res = record.Edit(1, 0);

            Assert.AreNotEqual(true, res);
        }

        [TestMethod]
        public void RecordEditIntAsDoubleWithCast()
        {
            List<Types> types = new List<Types>() { Types.DOUBLE, Types.VARCHAR };
            Record record = new Record(types);

            bool res = record.Edit((double)1, 0);

            Assert.AreEqual(true, res);
        }
    }
}
