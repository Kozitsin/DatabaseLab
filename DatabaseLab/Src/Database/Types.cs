using DatabaseLab.Logging;
using System.Text;
using System;
using System.Collections.Generic;

namespace DatabaseLab.DataBase
{
    public static class Types
    {
        #region Data Memebers

        public static int intSize = 11;
        public static int strSize = 100;
        public static int boolSize = 1;
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
                s = s.Remove(strSize - 1);
                Logger.Write("String was reduced!", Logger.Level.Warn);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(' ', strSize - s.Length);
                s = s.Insert(s.Length, sb.ToString());
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
                s = s.Insert(s.Length, sb.ToString());

                return s;
            }
        }

        private static bool? StrToBoolean(string s)
        {
            if (s == "1")
                return true;
            else if (s == "0")
                return false;
            else
                return null;
        }

        private static string StrToVarchar(string s)
        {
            s = s.TrimEnd(' ');
            return s;
        }

        private static int? StrToInteger(string s)
        {
            try
            {
                s = s.TrimEnd(' ');
                return Convert.ToInt32(s);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }

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
                    if (record.data[i].GetType() == System.Type.GetType("System.Boolean"))
                    {
                        sb.Append(BooleanToStr((bool)record.data[i]));
                    }
                    else if (record.data[i].GetType() == System.Type.GetType("System.String"))
                    {
                        sb.Append(VarcharToStr((string)record.data[i]));
                    }
                    else if (record.data[i].GetType() == System.Type.GetType("System.Int32"))
                    {
                        sb.Append(IntegerToStr((int)record.data[i]));
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return string.Empty;
            }

        }

        public static Record StrToRecord(string s, List<Type> types)
        {
            try
            {
                Record record = new Record(types);

                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i] == Type.BOOLEAN)
                    {
                        bool? value = StrToBoolean(s.Substring(0, 1));

                        if (value.HasValue)
                        {
                            record.Edit(value.Value, i);
                            s = s.Remove(0, 1);
                        }
                    }
                    else if (types[i] == Type.VARCHAR)
                    {
                        string value = StrToVarchar(s.Substring(0, strSize));

                        record.Edit(value, i);
                        s = s.Remove(0, strSize);
                    }
                    else if (types[i] == Type.INTEGER)
                    {
                        int? value = StrToInteger(s.Substring(0, intSize));

                        if (value.HasValue)
                        {
                            record.Edit(value.Value, i);
                            s = s.Remove(0, intSize);
                        }
                    }
                }
                return record;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
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
