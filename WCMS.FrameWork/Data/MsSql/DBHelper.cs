using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.IO;

using WCMS.FrameWork.UtilityLibrary;
using WCMS.FrameWork.Data.Configuration;

namespace WCMS.FrameWork.Data.MsSql
{
    /// <summary>
    /// DB Wrapper Class
    /// </summary>
    public class DBHelper : IDisposable
    {
        #region [Constant]
        /// <summary>
        /// 기본 Conneciontstring Key 값
        /// </summary>
        const string DefaultConnectionString = "DefaultConnection";
        #endregion

        #region [Properties]
        private DbProviderFactory _provider;
        private DbConnection _connection;
        private DbCommand _command;

        /// <summary>
        /// 데이터 소스 클래스의 공급자 구현에 대한 인스턴스를 만드는 데 사용되는 메서드의 집합을 나타냅니다.
        /// </summary>
        public DbProviderFactory Provider
        {
            get
            {
                return _provider;
            }
        }

        /// <summary>
        /// 데이터베이스에 대한 연결을 나타냅니다. 
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>
        /// SQL 문이나 데이터 소스에 대해 실행할 저장 프로시저를 나타냅니다. 
        /// </summary>
        public DbCommand Command
        {
            get
            {
                return _command;
            }
        }

        private bool _handleErrors = true;
        private string _lstError = "";

        /// <summary>
        /// Exception 처리 유무값을 설정 및 가져옵니다.
        /// </summary>
        public bool HandleExceptions
        {
            get { return _handleErrors; }
            set { _handleErrors = value; }
        }

        /// <summary>
        /// Error 정보를 가져옵니다.
        /// </summary>
        public string LastError
        {
            get { return _lstError; }
        }
        #endregion
        
        #region [Constructors]
        /// <summary>
        /// 생성자
        /// EntFramework.config 에서 ConnectionString 정보 조회
        /// </summary>
        public DBHelper()
        {
            CreateConnection(DefaultConnectionString);
        }

        /// <summary>
        /// 생성자
        /// EntFramework.config 에서 ConnectionString 정보 조회
        /// </summary>
        /// <param name="connectionString">ConnectionString Key</param>
        public DBHelper(string connectionString)
        {
            CreateConnection(connectionString);
        }

        /// <summary>
        /// 생성자
        /// default : EntFramework.config 에서 ConnectionString 정보 조회
        /// bWebConfig 값이 true 이면 Web.config 에서 ConnectionString 정보 조회
        /// </summary>
        /// <param name="connectionString">ConnectionString Key</param>
        /// <param name="bWebConfig">Web.config 사용 유무</param>
        public DBHelper(string connectionString, bool bWebConfig)
        {
            CreateConnection(connectionString, bWebConfig);
        }
        #endregion

        #region [Connection / Close]
        /// <summary>
        /// DB 연결 생성
        /// DbConnection 개체를 만들고 ConnectionString 속성이 연결 문자열에 설정됩니다.
        /// 현재 연결과 관련된 DbCommand 개체를 만들고 반환합니다. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bWebConfig"></param>
        private void CreateConnection(string key, bool bWebConfig = false)
        {
            ConnectionStringSettings css;
            if (bWebConfig)
                css = ConfigurationManager.ConnectionStrings[key];
            else {
                css = new ConnectionStringSettings("name", DBConfigSettings.ConnectionStrings.GetConnectionString(key), DBConfigSettings.ConnectionStrings.GetProviderName(key));
            }

            if (css == null)
                throw new ArgumentException("Invalid or missing connection string . Check if it exists in configuration file.");

            try
            {
                _provider = DbProviderFactories.GetFactory(css.ProviderName);

                _connection = _provider.CreateConnection();
                _connection.ConnectionString = css.ConnectionString;

                _command = _connection.CreateCommand();
            }
            catch (Exception ex)
            {
                throw ex;
                // throw new CustomException("DB", "DB Conn Error");
            }
        }

        /// <summary>
        /// DB 연결 닫기
        /// </summary>
        private void Close()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _command.Dispose();
                _connection.Close();
                _connection.Dispose();
                _provider = null;
            }
        }
        #endregion

        #region [Parameter]

        private readonly Func<DbCommandBuilder, int, string> getParameterName = CreateDelegate("GetParameterName");
        private readonly Func<DbCommandBuilder, int, string> getParameterPlaceholder = CreateDelegate("GetParameterPlaceholder");

        private static Func<DbCommandBuilder, int, string> CreateDelegate(string methodName)
        {
            MethodInfo method = typeof(DbCommandBuilder).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, Type.DefaultBinder, new Type[] { typeof(Int32) }, null);
            return (Func<DbCommandBuilder, int, string>)Delegate.CreateDelegate(typeof(Func<DbCommandBuilder, int, string>), method);
        }

        /// <summary>
        /// CommandType == CommandType.Text 일 경우 자동 파라메터 생성
        /// ex) query = "select * from tErrorLog where CreateDate between {0} and {1}"
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="command"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        private void ProcessParameters(DbProviderFactory factory, DbCommand command, string query, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                command.CommandText = query;
            }
            else
            {
                IFormatProvider formatProvider = CultureInfo.InvariantCulture;
                DbCommandBuilder commandBuilder = factory.CreateCommandBuilder();
                string queryText = query;

                for (int index = 0; index < parameters.Length; index++)
                {
                    string name = getParameterName(commandBuilder, index);
                    string placeholder = getParameterPlaceholder(commandBuilder, index);
                    string i = index.ToString("D", formatProvider);

                    DbParameter param = command.CreateParameter();
                    param.ParameterName = name;
                    param.Value = parameters[index];
                    command.Parameters.Add(param);

                    queryText = queryText.Replace("{" + i + "}", placeholder);
                }

                command.CommandText = queryText.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public void AddParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            DbParameter param = _command.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            param.Direction = direction;
            _command.Parameters.Add(param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(string name, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            DbParameter param = Provider.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            param.Direction = direction;
            return param;
        }
        #endregion

        #region [ExecuteReader]
        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataReader 로 반환
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <returns>DataReader</returns>
        public DbDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataReader 로 반환
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>DataReader</returns>
        public DbDataReader ExecuteReader(string commandText, params object[] parameters)
        {
            return ExecuteReader(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataReader 로 반환
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <returns>DataReader</returns>
        public DbDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            return ExecuteReader(commandType, commandText, null);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataReader 로 반환
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>DataReader</returns>
        public DbDataReader ExecuteReader(CommandType commandType, string commandText, params object[] parameters)
        {
            DbDataReader reader = null;

            try
            {
                _command.CommandType = commandType;
                _command.CommandText = commandText;
                if (commandType == CommandType.Text && parameters != null)
                {
                    ProcessParameters(_provider, _command, commandText, parameters);
                }

                _connection.Open();
                reader = _command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (DbException dbEx)
            {
                if (_handleErrors) _lstError = dbEx.Message;
                else throw dbEx; // CustomException DB, dbEx
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }

            return reader;
        }
        #endregion

        #region [ExecuteDataSet]
        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataSet으로 반환
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataSet으로 반환
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(string commandText, params object[] parameters)
        {
            return ExecuteDataSet(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataSet으로 반환
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            return ExecuteDataSet(commandType, commandText, null);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataSet으로 반환
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText, params object[] parameters)
        {
            DataSet ds = new DataSet();

            try
            {
                _command.CommandType = commandType;
                _command.CommandText = commandText;
                if (commandType == CommandType.Text && parameters != null)
                {
                    ProcessParameters(_provider, _command, commandText, parameters);
                }

                _connection.Open();
                using (DbDataAdapter adapter = Provider.CreateDataAdapter())
                {
                    adapter.SelectCommand = _command;
                    adapter.Fill(ds);
                }
            }
            catch (DbException dbEx)
            {
                if (_handleErrors) _lstError = dbEx.Message;
                else throw dbEx; // CustomException DB, dbEx
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }

            return ds;
        }
        #endregion

        #region [ExecuteDataTable]
        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataTable로 반환
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataTable로 반환
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable(string commandText, params object[] parameters)
        {
            return ExecuteDataTable(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataTable로 반환
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable(CommandType commandType, string commandText)
        {
            return ExecuteDataTable(commandType, commandText, null);
        }

        /// <summary>
        /// CommandText 를 Connection에 보내고, 결과값을 DataTable로 반환
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteDataTable(CommandType commandType, string commandText, params object[] parameters)
        {
            DataSet ds = new DataSet();

            try
            {
                _command.CommandType = commandType;
                _command.CommandText = commandText;
                if (commandType == CommandType.Text && parameters != null)
                {
                    ProcessParameters(_provider, _command, commandText, parameters);
                }

                _connection.Open();
                using (DbDataAdapter adapter = Provider.CreateDataAdapter())
                {
                    adapter.SelectCommand = _command;
                    adapter.Fill(ds);
                }
            }
            catch (DbException dbEx)
            {
                if (_handleErrors) _lstError = dbEx.Message;
                else throw dbEx; // CustomException DB, dbEx
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }

            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }
        #endregion

        #region [ExecuteScalar]
        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합에서 첫 번째 행의 첫 번째 열을 반환합니다. 
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <returns>object</returns>
        public object ExecuteScalar(string commandText) 
        {
            return ExecuteScalar(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합에서 첫 번째 행의 첫 번째 열을 반환합니다. 
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>object</returns>
        public object ExecuteScalar(string commandText, params object[] parameters)
        {
            return ExecuteScalar(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합에서 첫 번째 행의 첫 번째 열을 반환합니다. 
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <returns>object</returns>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return ExecuteScalar(commandType, commandText, null);
        }

        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합에서 첫 번째 행의 첫 번째 열을 반환합니다. 
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>object</returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params object[] parameters)
        {
            try
            {
                _command.CommandType = commandType;
                _command.CommandText = commandText;
                if (commandType == CommandType.Text && parameters != null)
                {
                    ProcessParameters(_provider, _command, commandText, parameters);
                }

                _connection.Open();
                return _command.ExecuteScalar();
            }
            catch (DbException dbEx)
            {
                if (_handleErrors) _lstError = dbEx.Message;
                else throw dbEx; // CustomException DB, dbEx
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }
            return null;
        }
        #endregion

        #region [ExecuteScalar<T>]
        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합에서 첫 번째 행의 첫 번째 열을 반환합니다. 
        /// </summary>
        /// <typeparam name="T">Generic object, 결과값 형타입</typeparam>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <returns>T(Generic object) 타입 값</returns>
        public T ExecuteScalar<T>(string commandText) where T : IConvertible
        {
            return ExecuteScalar<T>(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합에서 첫 번째 행의 첫 번째 열을 반환합니다. 
        /// </summary>
        /// <typeparam name="T">Generic object, 결과값 형타입</typeparam>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>T(Generic object) 타입 값</returns>
        public T ExecuteScalar<T>(string commandText, params object[] parameters) where T : IConvertible
        {
            return ExecuteScalar<T>(CommandType.Text, commandText, parameters);
        }

        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합에서 첫 번째 행의 첫 번째 열을 반환합니다. 
        /// </summary>
        /// <typeparam name="T">Generic object, 결과값 형타입</typeparam>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <returns>T(Generic object) 타입 값</returns>
        public T ExecuteScalar<T>(CommandType commandType, string commandText) where T : IConvertible
        {
            return ExecuteScalar<T>(commandType, commandText, null);
        }

        /// <summary>
        /// 쿼리를 실행하고 쿼리에서 반환된 결과 집합에서 첫 번째 행의 첫 번째 열을 반환합니다. 
        /// </summary>
        /// <typeparam name="T">Generic object, 결과값 형타입</typeparam>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>T(Generic object) 타입 값</returns>
        public T ExecuteScalar<T>(CommandType commandType, string commandText, params object[] parameters) where T : IConvertible
        {
            try
            {
                _command.CommandType = commandType;
                _command.CommandText = commandText;
                if (commandType == CommandType.Text && parameters != null)
                {
                    ProcessParameters(_provider, _command, commandText, parameters);
                }

                _connection.Open();
                var result = _command.ExecuteScalar();
                return GlobalConvert.ChangeType<T>(result);
            }
            catch (DbException dbEx)
            {
                if (_handleErrors) _lstError = dbEx.Message;
                else throw dbEx; // CustomException DB, dbEx
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }
            return GlobalConvert.DefaultValue<T>();
        }
        #endregion

        #region [ExecuteNonQuery]
        /// <summary>
        /// 연결에 대한 Transact-SQL 문을 실행하고 영향을 받는 행의 수를 반환합니다. 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>영향 받는 행의 수</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, false, IsolationLevel.Serializable, null);
        }

        /// <summary>
        /// 연결에 대한 Transact-SQL 문을 실행하고 영향을 받는 행의 수를 반환합니다. 
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <param name="parameters">쿼리문 파라메터에 할당되는 object 배열</param>
        /// <returns>영향 받는 행의 수</returns>
        public int ExecuteNonQuery(string commandText, params object[] parameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, false, IsolationLevel.Serializable, parameters);
        }

        /// <summary>
        /// 연결에 대한 Transact-SQL 문을 실행하고 영향을 받는 행의 수를 반환합니다. 
        /// </summary>
        /// <param name="commandText">Transact-SQL statement</param>
        /// <param name="IsTransRequired">트랜잭션 처리 유무</param>
        /// <param name="parameters">쿼리문 파라메터에 할당되는 object 배열</param>
        /// <returns>영향 받는 행의 수</returns>
        public int ExecuteNonQuery(string commandText, bool IsTransRequired = false, params object[] parameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, IsTransRequired, IsolationLevel.Serializable, parameters);
        }

        /// <summary>
        /// 연결에 대한 Transact-SQL 문을 실행하고 영향을 받는 행의 수를 반환합니다. 
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <returns>영향 받는 행의 수</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(commandType, commandText, false, IsolationLevel.Serializable, null);
        }

        /// <summary>
        /// 연결에 대한 Transact-SQL 문을 실행하고 영향을 받는 행의 수를 반환합니다. 
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <param name="IsTransRequired">트랜잭션 처리 유무</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>영향 받는 행의 수</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, bool IsTransRequired = false, params object[] parameters)
        {
            return ExecuteNonQuery(commandType, commandText, IsTransRequired, IsolationLevel.Serializable, parameters);
        }

        /// <summary>
        /// 연결에 대한 Transact-SQL 문을 실행하고 영향을 받는 행의 수를 반환합니다. 
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <param name="IsTransRequired">트랜잭션 처리 유무</param>
        /// <param name="isolationLevel">트랜잭션의 격리 수준</param>
        /// <returns>영향 받는 행의 수</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, bool IsTransRequired = false, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            return ExecuteNonQuery(commandType, commandText, IsTransRequired, isolationLevel, null);
        }

        /// <summary>
        /// 연결에 대한 Transact-SQL 문을 실행하고 영향을 받는 행의 수를 반환합니다. 
        /// </summary>
        /// <param name="commandType">CommandType</param>
        /// <param name="commandText">Transact-SQL statement, table name or stored procedure</param>
        /// <param name="IsTransRequired">트랜잭션 처리 유무</param>
        /// <param name="isolationLevel">트랜잭션의 격리 수준</param>
        /// <param name="parameters">SP(저장프로시저)나 SQL 문 파라메터에 할당되는 object 배열</param>
        /// <returns>영향 받는 행의 수</returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, bool IsTransRequired = false, IsolationLevel isolationLevel = IsolationLevel.Serializable, params object[] parameters)
        {
            int rowsAffected = 0;
            DbTransaction transaction = null;

            try
            {
                _command.CommandType = commandType;
                _command.CommandText = commandText;
                if (commandType == CommandType.Text && parameters != null)
                {
                    ProcessParameters(_provider, _command, commandText, parameters);
                }

                _connection.Open();
                if (IsTransRequired)
                {
                    // lets begin a transaction here
                    transaction = Connection.BeginTransaction(isolationLevel);
                    // assosiate this command with transaction
                    _command.Transaction = transaction;
                }

                rowsAffected = _command.ExecuteNonQuery();

                if (IsTransRequired) transaction.Commit();
            }
            catch (DbException dbEx)
            {
                if (IsTransRequired) transaction.Rollback();

                if (_handleErrors) _lstError = dbEx.Message;
                else throw dbEx; // CustomException DB, dbEx
            }
            catch (Exception ex)
            {
                if (IsTransRequired) transaction.Rollback();

                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }

            return rowsAffected;
        }
        #endregion

        #region [IDisposable]
        // Flag: Has Dispose already been called?
        bool disposed = false;
        

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return; 

            if (disposing)
            {
                // Free any other managed objects here.
                //
                _command.Dispose();
                _command = null;
                _connection.Close();
                _connection.Dispose();
                _connection = null;
                _provider = null;
            }

            // Free any unmanaged objects(all native resources) here.
            //
            disposed = true;
        }
        #endregion
    }
}

