using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BIPortal.Services
{
    public class DBConnection
    {
        public string ConnectString { get; set; }
        public string ERROR { get; set; }
        public SqlConnection SQLDBConnect { get; set; }
        public SqlCommand command;

        public DBConnection(string connectString)
        {
            ConnectString = connectString;
            SQLDBConnect = new SqlConnection(connectString);
        }

        public void OpenDBConnect()
        {
            try
            {
                SQLDBConnect = new SqlConnection(ConnectString);
                SQLDBConnect.Open();

                command = new SqlCommand();
                command.Connection = SQLDBConnect;
            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
        }

        public void CloseDBConnect()
        {
            try
            {
                if (SQLDBConnect.State == ConnectionState.Open)
                {
                    SQLDBConnect.Close();
                    SQLDBConnect.Dispose();
                    SqlConnection.ClearAllPools();
                }
                if (command != null)
                {
                    command.Parameters.Clear();
                }
            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
        }

        /// <summary>
        /// used to execute a stored procedure to change value of data records
        /// as update, delete, insert
        /// </summary>
        /// <param name="spName">name of store procedure</param>
        /// <param name="dicParameters">list of parameters containt the pairs of (name,value) that
        ///     mapping with paramenters of the store procedure
        /// </param>
        /// <param name="dicParaOutputs">Note that: this param enable be null.
        /// It is the list of paramenter in which the para is declared as output parameter</param>
        /// <returns>the number of  record that be effected when the store procedure be excuted</returns>
        public int ExecSPNonQuery(string spName, Dictionary<string, object> dicParameters, ref Dictionary<string, object> dicParaOutputs, bool isCloseConnect = false)
        {
            int result = 0;
            SqlParameter _sqlParameter = null;
            try
            {
                OpenDBConnect();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;

                Dictionary<string, object> dicTemp = new Dictionary<string, object>();
                foreach (string paraName in dicParameters.Keys)
                {
                    _sqlParameter = new SqlParameter("@" + paraName, dicParameters[paraName] == null ? DBNull.Value : dicParameters[paraName]);
                    command.Parameters.Add(_sqlParameter);
                }

                foreach (string paraName in dicParaOutputs.Keys)
                {
                    _sqlParameter = new SqlParameter("@" + paraName, dicParaOutputs[paraName] == null ? DBNull.Value : dicParaOutputs[paraName]);
                    _sqlParameter.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(_sqlParameter);
                    dicTemp.Add(paraName, _sqlParameter);
                }

                result = command.ExecuteNonQuery();
                dicParaOutputs.Clear();
                foreach (string paraName in dicTemp.Keys)
                {
                    object outvalue = command.Parameters["@" + paraName].Value;
                    dicParaOutputs.Add(paraName, outvalue);
                }
                dicTemp.Clear();
            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
            finally
            {
                if (isCloseConnect == true)
                    CloseDBConnect();
            }
            return result;
        }

        public SqlDataReader ExecSPReader(string spName, Dictionary<string, object> dicParameters, bool isCloseConnect = false)
        {
            SqlDataReader result = null;
            try
            {
                OpenDBConnect();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;

                //add input para
                foreach (string paraName in dicParameters.Keys)
                {
                    SqlParameter _sqlParameter = new SqlParameter("@" + paraName, dicParameters[paraName]);
                    command.Parameters.Add(_sqlParameter);
                }
                result = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
            finally
            {
                if (isCloseConnect == true)
                    CloseDBConnect();
            }
            return result;
        }

        public DataSet ExecSelectSP(string spName, Dictionary<string, object> dicParameters, ref Dictionary<string, object> dicParaOutputs, bool isCloseConnect = false)
        {
            DataSet result = null;
            try
            {
                //command.CommandType = CommandType.StoredProcedure;
                //command.CommandText = spName;
                OpenDBConnect();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = spName;

                Dictionary<string, object> dicTemp = new Dictionary<string, object>();

                foreach (string paraName in dicParameters.Keys)
                {
                    SqlParameter _sqlParameter = new SqlParameter("@" + paraName, dicParameters[paraName]);
                    command.Parameters.Add(_sqlParameter);
                }
                foreach (string paraName in dicParaOutputs.Keys)
                {
                    SqlParameter _sqlParameter = new SqlParameter("@" + paraName, dicParaOutputs[paraName]);
                    _sqlParameter.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(_sqlParameter);
                    dicTemp.Add(paraName, _sqlParameter);
                }
                result = new DataSet();
                SqlDataAdapter _sqlDataAdapter = new SqlDataAdapter(command);
                _sqlDataAdapter.Fill(result);

                dicParaOutputs.Clear();
                foreach (string paraname in dicTemp.Keys)
                {
                    object outvalue = command.Parameters["@" + paraname].Value;
                    dicParaOutputs.Add(paraname, outvalue);
                }

                dicTemp.Clear();
            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
            finally
            {
                if (isCloseConnect == true)
                    CloseDBConnect();
            }

            return result;
        }

        public object ExecSPReturnValue(string spName, Dictionary<string, object> dicParameters, ref Dictionary<string, object> dicParaOutputs, bool isCloseConnect = false)
        {
            object result = null;
            try
            {
                OpenDBConnect();

                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = spName;

                Dictionary<string, object> dicTemp = new Dictionary<string, object>();

                foreach (string paraName in dicParameters.Keys)
                {
                    SqlParameter _sqlParameter = new SqlParameter("@" + paraName, dicParameters[paraName]);
                    command.Parameters.Add(_sqlParameter);
                }
                foreach (string paraName in dicParaOutputs.Keys)
                {
                    SqlParameter _sqlParameter = new SqlParameter("@" + paraName, dicParaOutputs[paraName]);
                    _sqlParameter.Direction = ParameterDirection.InputOutput;
                    command.Parameters.Add(_sqlParameter);
                    dicTemp.Add(paraName, _sqlParameter);
                }
                SqlParameter paraReturn = new SqlParameter("@Return_Value", -1);
                paraReturn.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(paraReturn);

                command.Connection.Open();
                command.ExecuteNonQuery();
                result = paraReturn.Value;

                dicParaOutputs.Clear();
                foreach (string paraname in dicTemp.Keys)
                {
                    object outvalue = command.Parameters[paraname].Value;
                    dicParaOutputs.Add(paraname, outvalue);
                }

                dicTemp.Clear();
            }
            catch (Exception ex)
            {
                ERROR = ex.ToString();
            }
            finally
            {
                if (isCloseConnect == true)
                    CloseDBConnect();
            }

            return result;
        }
    }
}