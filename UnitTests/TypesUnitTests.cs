using DatabaseLab.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class TypesUnitTest
    {
        [TestMethod]
        public void TestNullRecordToString()
        {
            Record record = null;

            string result = Types.RecordToStr(record);

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void TestEmptyRecordToString()
        {
            List<Types.Type> types = new List<Types.Type>();
            Record record = new Record(types);

            string result = Types.RecordToStr(record);

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void TestBooleanRecordToString()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.BOOLEAN };
            Record record = new Record(types);
            record.Edit(true, 0);

            string result = Types.RecordToStr(record);

            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void TestIntegerRecordToString()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.INTEGER };
            Record record = new Record(types);
            record.Edit(123, 0);

            string result = Types.RecordToStr(record);

            Assert.AreEqual("123", result);
        }


        [TestMethod]
        public void TestVarcharRecordToString()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.VARCHAR };
            Record record = new Record(types);
            record.Edit("Ich liebe Wien!", 0);

            string result = Types.RecordToStr(record);

            Assert.AreEqual("Ich liebe Wien!", result);
        }
    }

}
