using DatabaseLab.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseLab.DataBase
{
    public class Record : IEquatable<Record>
    {
        #region Data Members

        public List<object> data = new List<object>();

        #endregion

        #region Constructor
        
        public Record(List<Types.Type> types)
        {
            for (int i = 0; i < types.Count; i++)
            {
                switch (types[i])
                {
                    case Types.Type.VARCHAR:
                        data.Add(string.Empty);
                        break;
                    case Types.Type.BOOLEAN:
                        data.Add(default(bool));
                        break;
                    case Types.Type.INTEGER:
                        data.Add(default(int));
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        public bool Edit(object obj, int index)
        {
            try
            {
                if (obj.GetType() == data[index].GetType())
                {
                    data[index] = obj;
                    return true;
                }
                else
                {
                    Logger.Write(string.Format("Type mistmaches: {0} when {1} expected", obj.GetType().ToString(), data[index].ToString()), Logger.Level.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        #region IEquable interface
        public override int GetHashCode()
        {
            int hashcode = 0;
            for (int i = 0; i < data.Count; i++)
            {
                hashcode ^= data[i].GetHashCode();
            }

            return hashcode;
        }

        public bool Equals(Record other)
        {
            if (other == null)
            {
                return false;
            }
            return data.SequenceEqual(other.data);
        }

        public override bool Equals(object obj)
        {
            Record record = obj as Record;
            if (record != null)
            {
                return Equals(record);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #endregion
    }
}
