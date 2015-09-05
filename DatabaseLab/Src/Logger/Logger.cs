using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DatabaseLab.Logging
{
    public static class Logger
    {
        #region Datamembers

        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
        public static string PathToWrite { get; set; }
        
        public enum Level
        {
            Info = 0,
            Warn = 1,
            Error = 2
        }

        #endregion

        #region Public Methods

        public static void Write(Exception ex)
        {
            Write(ex.Message, Level.Error);
        }

        public static void Write(string msg, Level lvl)
        {
            StringBuilder sb = new StringBuilder(DateTime.Now.ToString());
            switch (lvl)
            {
                case Level.Info:
                        sb.Append(" Info: ");
                        break;
                case Level.Warn:
                        sb.Append(" Warn: ");
                        break;
                case Level.Error:
                        sb.Append(" Error: ");
                        break;
            }
            sb.Append(msg);
            Write(sb.ToString());
        }

        #endregion

        #region Private Methods

        private static void Write(string msg)
        {
            locker.EnterWriteLock();
            try
            {
                StreamWriter file = new StreamWriter(PathToWrite, true);
                file.WriteLine(msg);
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        #endregion
    }
}
