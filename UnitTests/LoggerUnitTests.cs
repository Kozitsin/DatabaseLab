using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLab.Logging;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class LoggerUnitTests
    {
        [TestMethod]
        public void LoggerCreation()
        {
            Logger.Write("Test purpose", Logger.Level.Info);

            bool res = File.Exists("./TestLog.dat");

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void LoggerContainInfoData()
        {
            Logger.Write("Test purpose", Logger.Level.Info);

            string s = File.ReadAllText(Logger.pathToWrite);
            bool res = s.Contains("Info: Test purpose");

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void LoggerContainErrorData()
        {
            Logger.Write("Test purpose", Logger.Level.Error);

            string s = File.ReadAllText(Logger.pathToWrite);
            bool res = s.Contains("Error: Test purpose");

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void LoggerContainWarnData()
        {
            Logger.Write("Test purpose", Logger.Level.Warn);

            string s = File.ReadAllText(Logger.pathToWrite);
            bool res = s.Contains("Warn: Test purpose");

            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void LoggerException()
        {
            ArgumentNullException ex = new ArgumentNullException();
            Logger.Write(ex);

            string s = File.ReadAllText(Logger.pathToWrite);
            bool res = s.Contains("Error: " + ex.Message);

            Assert.AreEqual(true, res);
        }

        [TestCleanup]
        public void CleanUp()
        {
            try
            {
                File.Delete("./TestLog.dat");
            }
            catch (Exception)
            {

            }
        }
    }
}
