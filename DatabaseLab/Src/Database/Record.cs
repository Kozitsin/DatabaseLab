using DatabaseLab.Logging;
using System;
using System.Collections.Generic;

namespace DatabaseLab.Database
{
    public class Record
    {
        #region Data Members

        public List<object> data = new List<object>();

        #endregion

        #region Constructor
        
        public Record(List<Types> types)
        {
            for (int i = 0; i < types.Count; i++)
            {
                switch (types[i])
                {
                    case Types.VARCHAR:
                        data.Add(default(string));
                        break;
                    case Types.BOOLEAN:
                        data.Add(default(bool));
                        break;
                    case Types.INTEGER:
                        data.Add(default(int));
                        break;
                    case Types.FLOAT:
                        data.Add(default(float));
                        break;
                    case Types.DOUBLE:
                        data.Add(default(double));
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

        #endregion
    }
}
