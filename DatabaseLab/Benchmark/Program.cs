using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLab.DataBase;
using DatabaseLab.Forms;
using System.Diagnostics;

namespace Benchmark
{
    class Program
    {
        static Random rnd = new Random();
        static Stopwatch sw = new Stopwatch();


        static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        static void Main(string[] args)
        {
            string tableName = "test";
            Database DB = new Database();
            DB.CreateDatabaseFolder();

            List<Types.Type> types = new List<Types.Type>() { Types.Type.VARCHAR, Types.Type.VARCHAR,
                Types.Type.BOOLEAN, Types.Type.INTEGER };

            DB.CreateTable(tableName, types, new List<string>() { "varchar", "boolean", "integer" });


            Record record = new Record(types);



            for (int i = 0; i < 10000; i++)
            {
                record.Edit(RandomString(25), 0);
                record.Edit(RandomString(10), 1);
                record.Edit(Convert.ToBoolean(rnd.Next() % 2), 2);
                DB.AddRecord(tableName, record);

                sw.Start();
                DB.Search(tableName, i*2);
                sw.Stop();

                Console.WriteLine("Elapsed={0}", sw.Elapsed);
            }

        }
    }
}
