using System;
using System.Collections.Generic;
using System.IO;
using DatabaseLab.Logging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DatabaseLab.Database
{
    public static class FileSystemWrapper
    {
        private static string databasePath = System.AppDomain.CurrentDomain.BaseDirectory + '/' + "Data";

        public static void CreateDatabaseFolder()
        {
            try
            {
                if (!Directory.Exists(databasePath))
                {
                    Directory.CreateDirectory(databasePath);
                    Logger.Write("Database folder was successfully created!", Logger.Level.Info);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public static bool CreateTable(string tableName, List<Type> types, List<string> headers)
        {
            string path = databasePath + '/' + tableName + ".dat";
            bool isSuccess = false;
            try
            {
                if (!File.Exists(path))
                {
                    File.Create(path);
                    isSuccess = CreateUtilityFile(tableName, types, headers);
                    Logger.Write(String.Format("Table {0} was successfully created!", tableName), Logger.Level.Info);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            return isSuccess;
        }

        public static bool AddRecord(string tableName, Record record)
        {
            string pathTable = databasePath + '/' + tableName + ".dat";
            string pathUtile = databasePath + '/' + tableName + "_info.dat";

            try
            {
                if (File.Exists(pathTable) && File.Exists(pathUtile))
                {
                    long length = 0;
                    string converted = string.Empty;

                    using (FileStream fs = new FileStream(pathTable, FileMode.Open))
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        length = fs.Length;
                        converted = Types.RecordToStr(record);

                        writer.Write(StringToBytes(converted));
                        Logger.Write(String.Format("Record was successfully added in table {0}", tableName), Logger.Level.Info);
                    }

                    using (FileStream fs = new FileStream(pathUtile, FileMode.Open))
                    using (BinaryWriter writer = new BinaryWriter(fs))
                    {
                        writer.Write(GetHashCode(converted));
                        writer.Write(StringToBytes(' ' + length.ToString() + '\n'));

                        Logger.Write(String.Format("Hash was successfully added!"), Logger.Level.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

            return true;
        }



        private static byte[] StringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string BytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private static byte[] GetHashCode(string s)
        {
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            return sha.ComputeHash(StringToBytes(s));
        }

        private static bool CreateUtilityFile(string tableName, List<Type> types, List<string> headers)
        {
            string path = databasePath + '/' + tableName + "_info.dat";

            try
            {
                if (!File.Exists(path))
                {
                    string s = ConcatHeaders(types, headers);

                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(StringToBytes(s), 0, s.Length);
                    }
                }
                Logger.Write("The utility file was created!", Logger.Level.Info);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

            return true;
        }

        private static string ConcatHeaders(List<Type> types, List<string> headers)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < headers.Count; i++)
            {
                sb.AppendFormat("{0};{1};", types[i], headers[i]);
            }
            sb.AppendLine();

            return sb.ToString();
        }

    }
}
