using DatabaseLab.Logging;
using System.Text;

namespace DatabaseLab.Database
{
    public static class Types
    {
        #region Data Memebers

        private static int intSize = 11;
        private static int strSize = 100;

        #endregion

        #region Private Methods

        private static string BooleanToStr(bool value)
        {
            if (value)
                return "1";
            else
                return "0";
        }

        private static string VarcharToStr(string s)
        {
            if (s.Length > strSize)
            {
                s.Remove(strSize - 1);
                Logger.Write("String was reduced!", Logger.Level.Warn);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(' ', strSize - s.Length);
                s.Insert(s.Length, sb.ToString());
            }
            return s;
        }

        private static string IntegerToStr(int value)
        {
            string s = value.ToString();
            if (s.Length > intSize)
            {
                Logger.Write("Integer is bigger than configured size!", Logger.Level.Warn);
                return string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(' ', intSize - s.Length);
                s.Insert(s.Length, sb.ToString());

                return s;
            }
        }

        private static bool StrToBoolean(string s)
        {

        }

        private static string StrToVarchar(string s)
        {

        }

        private static int StrToInteger(string s)
        {

        }

        #endregion

        #region Public Methods

        public static string RecordToStr(Record record)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < record.data.Count; i++)
                {
                    if (record.data.GetType() == System.Type.GetType("bool"))
                    {
                        sb.Append(BooleanToStr((bool)record.data[i]));
                    }
                    else if (record.data.GetType() == System.Type.GetType("string"))
                    {
                        sb.Append(VarcharToStr((string)record.data[i]));
                    }
                    else if (record.data.GetType() == System.Type.GetType("int"))
                    {
                        sb.Append(IntegerToStr((int)record.data[i]));
                    }
                }

                return sb.ToString();
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex);
                return string.Empty;
            }

        }

        public static string StrToRecord(string s)
        {

        }

        #endregion

        #region Entities

        public enum Type
        {
            VARCHAR = 0,
            BOOLEAN = 1,
            INTEGER = 2,
        }

        #endregion
    }
}
