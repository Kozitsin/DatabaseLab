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
    public static class Database
    {
        #region Data Members

        private static string databasePath = AppDomain.CurrentDomain.BaseDirectory + '/' + "Data";

        #endregion

        #region Public Methods

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

        public static bool CreateTable(string tableName, List<Types.Type> types, List<string> headers)
        {
            string path = databasePath + '/' + tableName + ".dat";
            bool isSuccess = false;
            try
            {
                using (FileStream fs = File.Create(path))
                {
                    isSuccess = CreateUtilityFiles(tableName, types, headers);
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
            // TODO: need refactoring + wrapping hash table info

            string pathTable = databasePath + '/' + tableName + ".dat";
            string pathHash = databasePath + '/' + tableName + "_hash.dat";
            string pathFreeSpace = databasePath + '/' + tableName + "_freespace.dat";

            try
            {
                if (File.Exists(pathTable) && File.Exists(pathHash) && File.Exists(pathFreeSpace))
                {
                    string converted = string.Empty;

                    int emptyPosition = SeekEmptySpace(pathFreeSpace);
                    if (emptyPosition == -1)
                    {

                        long length = 0;

                        using (FileStream fs = new FileStream(pathTable, FileMode.Append))
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            length = fs.Length;
                            converted = Types.RecordToStr(record);

                            writer.Write(StringToBytes(converted));
                            Logger.Write(String.Format("Record was successfully added in table {0}", tableName), Logger.Level.Info);
                        }

                        using (FileStream fs = new FileStream(pathHash, FileMode.Append))
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            writer.Write(GetHashCode(converted));
                            writer.Write(StringToBytes(' ' + length.ToString() + '\n'));

                            Logger.Write(String.Format("Hash was successfully added!"), Logger.Level.Info);
                        }
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(pathTable, FileMode.Append))
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            converted = Types.RecordToStr(record);

                            writer.Write(StringToBytes(converted), emptyPosition, converted.Length);
                            Logger.Write(String.Format("Record was successfully added in table {0}", tableName), Logger.Level.Info);
                        }

                        using (FileStream fs = new FileStream(pathHash, FileMode.Append))
                        using (BinaryWriter writer = new BinaryWriter(fs))
                        {
                            writer.Write(GetHashCode(converted));
                            writer.Write(StringToBytes(' ' + emptyPosition.ToString() + '\n'));

                            Logger.Write(String.Format("Hash was successfully added!"), Logger.Level.Info);
                        }
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

        public static List<Record> Search(string tableName, Record record)
        {
            // TODO: work around problem with hashes and reading them
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";
                string pathHash = databasePath + '/' + tableName + "_hash.dat";

                string hash = BytesToString(GetHashCode(Types.RecordToStr(record)));

                string line = string.Empty;

                using (StreamReader sr = new StreamReader(pathHash, Encoding.Unicode))
                {
                    while ((line = sr.ReadLine()) != null)
                        if (line.Contains(hash))
                            break;
                }

                if (string.IsNullOrEmpty(line))
                {
                    return null;
                }
                else
                {
                    line = line.Remove(0, hash.Length + 1);

                    int[] pointers = Array.ConvertAll(line.Split(' '), int.Parse);
                    List<Record> result = new List<Record>();

                    int length = GetLengthOfRecord(tableName);
                    List<Types.Type> types = GetTypesOfRecord(tableName);

                    byte[] buffer = new byte[length * 2];

                    using (FileStream fs = new FileStream(pathTable, FileMode.Open))
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        foreach (var item in pointers)
                        {
                            reader.BaseStream.Seek(item, SeekOrigin.Begin);
                            reader.Read(buffer, 0, buffer.Length);
                            result.Add(Types.StrToRecord(BytesToString(buffer), types));
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }

        }

        public static bool DeleteRecord(string tableName, Record record)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods

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

        private static bool CreateUtilityFiles(string tableName, List<Types.Type> types, List<string> headers)
        {
            string path = databasePath + '/' + tableName;

            try
            {
                if (!File.Exists(path + "_headers.dat"))
                {
                    string s = ConcatHeaders(types, headers);
                    using (FileStream fs = File.Create(path + "_headers.dat"))
                    {
                        byte[] array = StringToBytes(s);
                        fs.Write(array, 0, array.Length);
                    }
                    Logger.Write("The header file was created!", Logger.Level.Info);
                }

                if (!File.Exists(path + "_hash.dat"))
                {
                    using (FileStream fs = File.Create(path + "_hash.dat"))
                        Logger.Write("The hash file was created!", Logger.Level.Info);
                }

                if (!File.Exists(path + "_freespace.dat"))
                {
                    using (FileStream fs = File.Create(path + "_freespace.dat"))
                        Logger.Write("The free space file was created!", Logger.Level.Info);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

            return true;
        }

        private static string ConcatHeaders(List<Types.Type> types, List<string> headers)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < headers.Count; i++)
            {
                sb.AppendFormat("{0};{1};", types[i], headers[i]);
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private static int SeekEmptySpace(string path)
        {
            try
            {
                string[] lines = File.ReadAllLines(path);
                if (lines.Count() <= 0)
                {
                    return -1;
                }
                else
                {
                    File.WriteAllLines(path, lines.Take(lines.Count() - 1));
                    return Convert.ToInt32(lines[lines.Count() - 1]);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return -1;
            }
        }

        private static int GetLengthOfRecord(string tableName)
        {
            try
            {
                List<Types.Type> types = GetTypesOfRecord(tableName);
                int length = 0;
                foreach (var item in types)
                {
                    switch (item)
                    {
                        case Types.Type.BOOLEAN:
                            length += Types.boolSize;
                            break;
                        case Types.Type.INTEGER:
                            length += Types.intSize;
                            break;
                        case Types.Type.VARCHAR:
                            length += Types.strSize;
                            break;
                    }
                }

                return length;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return -1;
            }
        }

        private static List<Types.Type> GetTypesOfRecord(string tableName)
        {
            string pathHeader = databasePath + '/' + tableName + "_headers.dat";
            try
            {
                List<Types.Type> result = new List<Types.Type>();
                using (StreamReader sr = new StreamReader(pathHeader, Encoding.Unicode))
                {
                    string s = sr.ReadToEnd();
                    string[] types = s.Split(';');

                    for (int i = 0; i < types.Count(); i += 2)
                    {
                        switch (types[i])
                        {
                            case "BOOLEAN":
                                result.Add(Types.Type.BOOLEAN);
                                break;
                            case "INTEGER":
                                result.Add(Types.Type.INTEGER);
                                break;
                            case "VARCHAR":
                                result.Add(Types.Type.VARCHAR);
                                break;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
        }

        #endregion
    }
}
