using System.Data.SqlClient;

namespace BIPortal.Services
{
    public class DBBaseService
    {
        public string ERROR { get; set; }
        public DBConnection DBConnection { get; set; }
        public bool IsCloseDBAfterExecute { get; set; }

        public DBBaseService(DBConnection dBConnection)
        {
            DBConnection = dBConnection;
            IsCloseDBAfterExecute = true;
        }

        public DBBaseService(DBConnection connection, string dbDWHConnectionString)
        {

        }

        public bool CheckPermission(int menuid, int userid, int roleid, bool isCheckRole)
        {
            bool output = true;
            return output;
        }

        public object GetReaderValue(SqlDataReader dataReader, string columnName)
        {
            object output = null;
            int index = dataReader.GetOrdinal(columnName);
            if (dataReader.IsDBNull(index)) { return output; }
            output = dataReader[index];
            return output;

        }
    }
}