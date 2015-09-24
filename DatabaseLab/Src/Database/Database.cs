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
    public class Database : IDatabase
    {
        #region Data Members

        private static string databasePath = AppDomain.CurrentDomain.BaseDirectory + '/' + "Data";

        #endregion

        #region Public Methods

        public void CreateDatabaseFolder()
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

        public bool CreateTable(string tableName, List<Types.Type> types, List<string> headers)
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

        public bool AddRecord(string tableName, Record record)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";
                string pathHash = databasePath + '/' + tableName + "_hash.dat";
                string pathFreeSpace = databasePath + '/' + tableName + "_freespace.dat";

                if (File.Exists(pathTable) && File.Exists(pathHash) && File.Exists(pathFreeSpace))
                {
                    long position = SeekEmptySpace(pathFreeSpace);

                    WriteData(pathTable, ref position, record);
                    WriteHash(pathHash, position, record);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public List<Record> Search(string tableName, Record record)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";

                int[] pointers = FindIndex(tableName, record);
                if (pointers == null)
                {
                    return null;
                }

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
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
        }

        public bool DeleteRecord(string tableName, Record record)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";
                string pathHash = databasePath + '/' + tableName + "_hash.dat";
                string pathFreeSpace = databasePath + '/' + tableName + "_freespace.dat";

                int[] position = FindIndex(tableName, record);

                using (StreamWriter writer = new StreamWriter(pathTable))
                {
                    for (int i = 0; i < position.Length; i++)
                    {
                        writer.BaseStream.Seek(position[i], SeekOrigin.Begin);
                        writer.Write(GenerateEmptyString(tableName, record));
                    }
                }

                using (StreamWriter writer = new StreamWriter(pathFreeSpace))
                {
                    for (int i = 0; i < position.Length; i++)
                        writer.WriteLine(position[i]);
                }

                using (StreamWriter writer = new StreamWriter(pathHash))
                {
                    RemoveHash(pathHash, GetHashCode(Types.RecordToStr(record)));
                }
                return true;
            }
            catch(Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public bool Update(string tableName, Record original, Record modified)
        {
            try
            {
                bool value = true;

                value &= DeleteRecord(tableName, original);
                value &= AddRecord(tableName, modified);

                return value;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        bool BackUp(string tableName)
        {
            throw new NotImplementedException();
        }

        bool Restore(string path)
        {
            throw new NotImplementedException();
        }


        bool Import(string tableName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods

        private byte[] StringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private string BytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private byte[] GetHashCode(string s)
        {
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            return sha.ComputeHash(StringToBytes(s));
        }

        private bool CreateUtilityFiles(string tableName, List<Types.Type> types, List<string> headers)
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

        private string ConcatHeaders(List<Types.Type> types, List<string> headers)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < headers.Count; i++)
            {
                sb.AppendFormat("{0};{1};", types[i], headers[i]);
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private int SeekEmptySpace(string path)
        {
            try
            {
                string data = string.Empty;
                int dataPosition = -1;

                using (StreamReader reader = new StreamReader(path))
                {
                    data = reader.ReadLine();
                }

                string[] rawData = data.Split(' ');
                if (rawData.Length > 0)
                {
                    dataPosition = Convert.ToInt32(rawData.Last());
                }

                // make overwrite
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    if (rawData.Length > 1)
                        writer.WriteLine(rawData.Take(rawData.Length - 1));
                    else
                        writer.WriteLine();

                }
                return dataPosition;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return -1;
            }
        }

        private int GetLengthOfRecord(string tableName)
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

        private List<Types.Type> GetTypesOfRecord(string tableName)
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

        // calculate hash and return index of record
        private int[] FindIndex(string tableName, Record record)
        {
            try
            {
                string pathHash = databasePath + '/' + tableName + "_hash.dat";
                string hash = BytesToString(GetHashCode(Types.RecordToStr(record)));
                string line = string.Empty;

                using (StreamReader sr = new StreamReader(pathHash))
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
                    return pointers;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
        }

        private string GenerateEmptyString(string tableName, Record record)
        {
            StringBuilder sb = new StringBuilder();
            int length = GetLengthOfRecord(tableName);

            sb.Append(' ', length);

            return sb.ToString();
        }

        private bool HashExists(string path, byte[] hashCode, out int counter)
        {
            counter = 0;
            try
            {
                string line = string.Empty;
                string hash = BytesToString(hashCode);

                using (StreamReader sr = new StreamReader(path, Encoding.Unicode))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        counter++;
                        if (line.Contains(hash))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        private void WriteData(string pathTable, ref long position, Record record)
        {
            using (FileStream fs = new FileStream(pathTable, FileMode.Open))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                if (position == -1)
                    position = fs.Length;

                string converted = Types.RecordToStr(record);

                writer.BaseStream.Position = position;
                writer.Write(StringToBytes(converted));
                Logger.Write("Record was successfully added in table", Logger.Level.Info);
            }
        }

        private void WriteHash(string pathHash, long position, Record record)
        {
            byte[] hash = GetHashCode(Types.RecordToStr(record));

            using (StreamWriter writer = new StreamWriter(pathHash))
            {
                int counter = 0;
                if (!HashExists(pathHash, hash, out counter))
                {
                    writer.WriteLine(BytesToString(hash) + ' ' + position);
                }
                else
                {
                    string prevValue = RemoveHash(pathHash, hash);
                    writer.WriteLine(prevValue + ' ' + position);
                }
                Logger.Write("Hash was successfully added!", Logger.Level.Info);
            }
        }

        private string RemoveHash(string pathHash, byte[] hash)
        {
            int counter = 0;
            if (HashExists(pathHash, hash, out counter))
            {
                string[] arrLine = File.ReadAllLines(pathHash);
                File.WriteAllLines(pathHash, arrLine.Take(counter));
                File.WriteAllLines(pathHash, arrLine.Skip(counter + 1));

                return arrLine[counter];
            }
            return string.Empty;
        }

        #endregion
    }
}
