using DatabaseLab.Database;
using DatabaseLab.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class TypesUnitTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Logger.pathToWrite = "./TestLog.dat";
        }

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

            Assert.AreEqual("123        ", result);
        }


        [TestMethod]
        public void TestVarcharRecordToString()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.VARCHAR };

            string s = "Ich liene Wien!";
            Record record = new Record(types);
            record.Edit(s, 0);

            string result = Types.RecordToStr(record);

            StringBuilder sb = new StringBuilder(s);
            sb.Append(' ', 100 - s.Length);
            s = sb.ToString();

            Assert.AreEqual(s, result);
        }

        [TestMethod]
        public void TestEmptyStringToRecord()
        {
            string s = string.Empty;
            List<Types.Type> types = new List<Types.Type>();
            Record record = Types.StrToRecord(s, types);

            Assert.AreEqual(new Record(types), record);
        }

        [TestMethod]
        public void TestCorrectStringToRecord()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.VARCHAR, Types.Type.INTEGER, Types.Type.BOOLEAN };

            string s = "ich liebe Wien!";
            int i = int.MinValue;
            bool b = true;

            Record record = new Record(types);
            record.Edit(s, 0);
            record.Edit(i, 1);
            record.Edit(b, 2);

            StringBuilder sb = new StringBuilder(s);
            sb.Append(' ', 100 - s.Length);
            s = sb.ToString();

            string actual = s + i.ToString() + "1";

            Assert.AreEqual(record, Types.StrToRecord(actual, types));
        }

        [TestMethod]
        public void TestTransitivityOfConverting()
        {
            List<Types.Type> types = new List<Types.Type>() { Types.Type.VARCHAR, Types.Type.INTEGER, Types.Type.BOOLEAN };

            string s = "ich liebe Wien!";
            int i = int.MinValue;
            bool b = true;

            Record record = new Record(types);
            record.Edit(s, 0);
            record.Edit(i, 1);
            record.Edit(b, 2);

            StringBuilder sb = new StringBuilder(s);
            sb.Append(' ', 100 - s.Length);
            s = sb.ToString();

            string actual = s + i.ToString() + "1";

            Assert.AreEqual(record, Types.StrToRecord(Types.RecordToStr(record), types));
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
