using DatabaseLab.Logging;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DatabaseLab.DataBase
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

        public bool DeleteTable(string tableName)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";
                string pathHash = databasePath + '/' + tableName + "_hash.dat";
                string pathFreeSpace = databasePath + '/' + tableName + "_freespace.dat";
                string pathHeaders = databasePath + '/' + tableName + "_headers.dat";

                File.Delete(pathTable);
                File.Delete(pathHash);
                File.Delete(pathFreeSpace);
                File.Delete(pathHeaders);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
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

                    WriteData(tableName, ref position, record);
                    WriteHash(tableName, position, record.data[0].ToString());

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

        public List<Record> Search(string tableName, string s)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";

                int[] pointers = FindIndex(tableName, s);
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
                        //reader.BaseStream.Seek(item, SeekOrigin.Begin);
                        reader.BaseStream.Position = item;
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

        public int Search(string tableName, int uid)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";

                int recordLength = GetLengthOfRecord(tableName);
                int position = uid * recordLength;

                return position;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return -1;
            }
        }

        public bool DeleteRecord(string tableName, string s)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";
                string pathHash = databasePath + '/' + tableName + "_hash.dat";
                string pathFreeSpace = databasePath + '/' + tableName + "_freespace.dat";

                int[] position = FindIndex(tableName, s);

                using (FileStream fs = new FileStream(pathTable, FileMode.Open))
                using (BinaryWriter writer = new BinaryWriter(fs, Encoding.Unicode))
                {
                    byte[] buffer = StringToBytes(GenerateEmptyString(tableName));

                    for (int i = 0; i < position.Length; i++)
                    {
                        //writer.BaseStream.Seek(position[i], SeekOrigin.Begin);
                        writer.BaseStream.Position = position[i];
                        writer.Write(buffer, 0, buffer.Length);
                    }
                }

                using (StreamWriter writer = new StreamWriter(pathFreeSpace))
                {
                    for (int i = 0; i < position.Length; i++)
                        writer.WriteLine(position[i]);
                }

                using (StreamWriter writer = new StreamWriter(pathHash))
                {
                    RemoveHash(pathHash, GetHashCode(s));
                }
                return true;
            }
            catch(Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public bool Update(string tableName, int uid, Record modified)
        {
            try
            {
                bool value = false;

                long position = Search(tableName, uid);

                if (position != -1)
                {
                    WriteData(tableName, ref position, modified);
                    WriteHash(tableName, position, modified.data[0].ToString());

                    value = true;
                }

                return value;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public bool BackUp(string tableName)
        {
            string pathTable = databasePath + '/' + tableName + ".dat";
            string pathHash = databasePath + '/' + tableName + "_hash.dat";
            string pathFreeSpace = databasePath + '/' + tableName + "_freespace.dat";
            string pathHeaders = databasePath + '/' + tableName + "_headers.dat";
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddFile(pathTable);
                    zip.AddFile(pathHash);
                    zip.AddFile(pathFreeSpace);
                    zip.AddFile(pathHeaders);

                    zip.Save(tableName + ".zip");
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public bool Restore(string tableName)
        {
            string path = databasePath + '/' + tableName + ".zip";
            try
            {
                ZipFile zip = ZipFile.Read(path);
                zip.ExtractAll(databasePath);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }

        }

        public bool Import(string tableName)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";
                string pathHeaders = databasePath + '/' + tableName + "_headers.dat";
                string pathCSV = databasePath + '/' + tableName + ".csv";
                string pathFreeSpace = databasePath + '/' + tableName + "_freespace.dat";


                List<long> freeSpaceList = new List<long>();
                using (StreamReader reader = new StreamReader(pathFreeSpace))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                        freeSpaceList.Add(Convert.ToInt64(line));
                }

                using (StreamWriter writer = new StreamWriter(pathCSV))
                {
                    string[] headers = GetHeaders(pathHeaders);

                    for (int i = 0; i < headers.Length; i++)
                    {
                        writer.Write(headers[i]);
                        writer.Write(';');
                    }
                    if (headers != null)
                        writer.WriteLine();

                    using (FileStream fs = new FileStream(pathTable, FileMode.Open))
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        int sizeOfRecord = GetLengthOfRecord(tableName);
                        byte[] buffer = new byte[sizeOfRecord * 2];

                        List<Types.Type> types = GetTypesOfRecord(tableName);
                        Record temp = null;

                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {

                            reader.Read(buffer, 0, buffer.Length);
                            string s = BytesToString(buffer);

                            if (!freeSpaceList.Contains(reader.BaseStream.Position))
                            {
                                temp = Types.StrToRecord(s, types);

                                for (int j = 0; j < temp.data.Count; j++)
                                {
                                    writer.Write(temp.data[j].ToString().TrimEnd(' '));
                                    writer.Write(';');
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public Record GetRecord(string tableName, int position)
        {
            try
            {
                string pathTable = databasePath + '/' + tableName + ".dat";

                int length = GetLengthOfRecord(tableName);
                List<Types.Type> types = GetTypesOfRecord(tableName);

                Record result = new Record(types);

                byte[] buffer = new byte[length * 2];

                using (FileStream fs = new FileStream(pathTable, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs, Encoding.Unicode))
                {
                    //reader.BaseStream.Seek(length, SeekOrigin.Begin);
                    reader.BaseStream.Position = length;
                    reader.Read(buffer, 0, buffer.Length);
                    result = Types.StrToRecord(BytesToString(buffer), types);
                }
                return result;

            }
            catch(Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
        }
        #endregion

        #region Private Methods

        private byte[] StringToBytes(string str)
        {
            return Encoding.Unicode.GetBytes(str);
        }

        private string BytesToString(byte[] bytes)
        {
            return Encoding.Unicode.GetString(bytes);
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

        private string[] GetHeaders(string pathHeaders)
        {
            try
            {
                string[] headers = null;
                using (StreamReader reader = new StreamReader(pathHeaders))
                {
                    string[] temp = reader.ReadLine().Split(';');
                    headers = temp.Where((value, index) => index % 2 == 0).ToArray();
                }
                return headers;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return null;
            }
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

        public List<Types.Type> GetTypesOfRecord(string tableName)
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

        private int[] FindIndex(string tableName, string s)
        {
            try
            {
                string pathHash = databasePath + '/' + tableName + "_hash.dat";
                string hash = BytesToString(GetHashCode(s));
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

        private string GenerateEmptyString(string tableName)
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

        private void WriteData(string tableName, ref long position, Record record)
        {
            string pathTable = databasePath + '/' + tableName + ".dat";

            using (FileStream fs = new FileStream(pathTable, FileMode.Open))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                if (position == -1)
                    position = fs.Length;

                // create a uid as last element
                record.data[record.data.Count - 1] = (int)position / GetLengthOfRecord(tableName);

                string converted = Types.RecordToStr(record);

                writer.BaseStream.Position = position;
                writer.Write(StringToBytes(converted));
                Logger.Write("Record was successfully added in table", Logger.Level.Info);
            }
        }

        private void WriteHash(string tableName, long position, string s)
        {
            string pathHash = databasePath + '/' + tableName + "_hash.dat";

            try
            {
                byte[] hash = GetHashCode(s);
                int counter = 0;
                if (!HashExists(pathHash, hash, out counter))
                {
                    using (StreamWriter writer = new StreamWriter(pathHash, true, Encoding.Unicode))
                    {
                        writer.WriteLine(BytesToString(hash) + ' ' + position);
                    }
                }
                else
                {
                    string prevValue = RemoveHash(pathHash, hash);

                    using (StreamWriter writer = new StreamWriter(pathHash, true, Encoding.Unicode))
                    {
                        writer.Write(prevValue);
                        writer.Write(' ');
                        writer.WriteLine(position.ToString());
                    }
                }
                Logger.Write("Hash was successfully added!", Logger.Level.Info);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private string RemoveHash(string pathHash, byte[] hash)
        {
            int counter = 0;
            if (HashExists(pathHash, hash, out counter))
            {
                string[] arrLine = File.ReadAllLines(pathHash);
                File.WriteAllLines(pathHash, arrLine.Take(counter - 1), Encoding.Unicode);
                File.AppendAllLines(pathHash, arrLine.Skip(counter), Encoding.Unicode);

                return arrLine[counter - 1];
            }
            return string.Empty;
        }

        #endregion
    }
}