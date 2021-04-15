using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace WebAPI.Classes
{
    class DataManager
    {
        #region Members
        private string connectionString;
        private string _exceptionMessage = "";
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private SqlDataAdapter sqlDataAdapter;
        private DataSet dataSet;

        public enum DataTypes { bigint, binary, bit, Char, datetime, Float, Decimal, image, Int, money, nchar, ntext, nvarchar, numeric, real, smalldatetime, smallint, smallmoney, sql_variant, text, timestamp, tinyint, uniqueidentifier, varbinary, varchar };

        #endregion

        #region Constructors
        public DataManager()
        {
        }

        #endregion

        #region Properties

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        public string exceptionMessage
        {
            get
            {
                return _exceptionMessage;
            }
            set
            {
                _exceptionMessage = value;
            }
        }
        #endregion

        #region Public Methods

        public DataSet ExecuteQuery(string queryString, string dataSetName)
        {
            dataSet = new DataSet();
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                sqlDataAdapter = new SqlDataAdapter(queryString, sqlConnection);
                sqlDataAdapter.Fill(dataSet, dataSetName);
                return dataSet;
            }
            catch (Exception Ex)
            {
                _exceptionMessage = Ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }
            return dataSet;
        }

        public DataSet ExecuteProcedure(string storedProcedure, string dataSetName, Hashtable parameterList)
        {
            dataSet = new DataSet();
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                addParameters(sqlCommand, parameterList);
                sqlConnection.Open();
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataSet, dataSetName);
                return dataSet;
            }
            catch (Exception Ex)
            {

                _exceptionMessage = Ex.Message;
            }
            finally
            {
                sqlConnection.Close();

            }
            return dataSet;
        }

        public int ExecuteStoredProcedure_ExecuteNonQuery(string storedProcedure, Hashtable parameterList)
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                addParameters(sqlCommand, parameterList);
                sqlConnection.Open();
                return sqlCommand.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                _exceptionMessage = Ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }
            return 0;
        }

        public string ExecuteProcs(string storedProcedure, string returnParameterName, Hashtable parameterList)
        {
            string _returnValue = "";
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                addParameters(sqlCommand, parameterList);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                _returnValue = (string)sqlCommand.Parameters["@" + returnParameterName].Value;
                return _returnValue;
            }
            catch (Exception Ex)
            {
                _exceptionMessage = Ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }
            return _returnValue;
        }

        public object ExecuteProcSingle(string storedProcedure, Hashtable parameterList)
        {
            object _returnValue = null;
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                addParameters(sqlCommand, parameterList);
                sqlConnection.Open();
                _returnValue = sqlCommand.ExecuteScalar();
                return _returnValue;
            }
            catch (Exception Ex)
            {
                _exceptionMessage = Ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }
            return _returnValue;
        }

        public string ExecuteStoredProcedure(string storedProcedure)
        {
            string _returnValue = "";
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                //sqlCommand = new SqlCommand(queryString, sqlConnection);
                sqlConnection.Open();
                _returnValue = sqlCommand.ExecuteScalar().ToString();
                return _returnValue;
            }
            catch (Exception Ex)
            {
                _exceptionMessage = Ex.Message;
            }
            finally
            {
                sqlConnection.Close();

            }
            return _returnValue;
        }

        public XmlDocument ReturnSingleXMLValue(string storedProcedure)
        {
            XmlDocument xdoc = new XmlDocument();
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(storedProcedure, sqlConnection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlConnection.Open();
                //_returnValue = sqlCommand.ExecuteXmlReader().ToString();

                XmlReader xmlr = sqlCommand.ExecuteXmlReader();
                xmlr.Read();
                xdoc.Load(xmlr);   
                return xdoc;
            }
            catch (Exception Ex)
            {
                _exceptionMessage = Ex.Message;
            }
            finally
            {
                sqlConnection.Close();

            }
            return xdoc;
        }

        public void ExecuteSQLCommand(string p_sqlCommand)
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlCommand = new SqlCommand(p_sqlCommand, sqlConnection);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                _exceptionMessage = Ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        #endregion

        #region Private Methods

        private void addParameters(SqlCommand sqlCommand, Hashtable parameterList)
        {
            IDictionaryEnumerator e = parameterList.GetEnumerator();
            while (e.MoveNext())
            {
                sqlCommand.Parameters.AddWithValue(e.Key.ToString(), e.Value.ToString());
            }
        }

        #endregion
    }
}
