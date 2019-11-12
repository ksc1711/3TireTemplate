using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Reflection;

using Dapper;
using DapperExtensions;
using System.Configuration;

using WCMS.FrameWork.Data.Configuration;

namespace WCMS.FrameWork.Data.Mapper.Dapper
{
    /// <summary>
    /// Dapper Wrapper Class
    /// </summary>
    public class DapperHelper : IDisposable
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
        private DynamicParameters _params;

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
        /// DynamicParameters 개체를 가져옵니다.
        /// </summary>
        public object Params
        {
            get
            {
                return _params;
            }
        }

        private bool _handleErrors = false;
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
        public DapperHelper()
        {
            CreateConnection(DefaultConnectionString);
        }

        /// <summary>
        /// 생성자
        /// EntFramework.config 에서 ConnectionString 정보 조회
        /// </summary>
        /// <param name="connectionString">ConnectionString Key</param>
        public DapperHelper(string connectionString)
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
        public DapperHelper(string connectionString, bool bWebConfig)
        {
            CreateConnection(connectionString, bWebConfig);
        }

        /// <summary>
        /// 생성자
        /// default : EntFramework.config 에서 ConnectionString 정보 조회
        /// bWebConfig 값이 true 이면 Web.config 에서 ConnectionString 정보 조회
        /// </summary>
        /// <param name="connectionString">ConnectionString Key</param>
        /// <param name="bWebConfig">Web.config 사용 유무</param>
        /// <param name="bConnectionName">ConnectionString Name 여부(true : name, false : connectionString)</param>
        public DapperHelper(string connectionString, bool bWebConfig, bool bConnectionName)
        {
            CreateConnection(connectionString, bWebConfig, bConnectionName);
        }
        #endregion

        #region [Connection / Close]
        /// <summary>
        /// DB 연결 생성
        /// DbConnection 개체를 만들고 ConnectionString 속성이 연결 문자열에 설정됩니다.
        /// </summary>
        /// <param name="key">ConnectionString Key</param>
        /// <param name="bWebConfig">Web.Config 참조 여부</param>
        /// <param name="bConnectionName">ConnectionString Name 여부(true : name, false : connectionString)</param>
        private void CreateConnection(string key, bool bWebConfig = false, bool bConnectionName = true)
        {
            ConnectionStringSettings css;
            if (bWebConfig && bConnectionName)
                css = ConfigurationManager.ConnectionStrings[key];
            else if (bWebConfig && !bConnectionName)
                css = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().FirstOrDefault(o => o.ConnectionString == key);
            else
                css = new ConnectionStringSettings("name", DBConfigSettings.ConnectionStrings.GetConnectionString(key), DBConfigSettings.ConnectionStrings.GetProviderName(key));
            
            
            if (css == null)
                throw new ArgumentException("Invalid or missing connection string . Check if it exists in configuration file.");

            try
            {
                _provider = DbProviderFactories.GetFactory(css.ProviderName);

                _connection = _provider.CreateConnection();
                _connection.ConnectionString = css.ConnectionString;
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
                _connection.Close();
                _connection.Dispose();
                _provider = null;
            }
        }
        #endregion

        #region [Parameter]

        /// <summary>
        /// DynamicParameters 추가
        /// </summary>
        /// <param name="name">Parameter 명</param>
        /// <param name="value">Parameter 값</param>
        /// <param name="dbtype">Parameter 개체의 데이터 형식</param>
        /// <param name="direction">매개 변수의 형식</param>
        /// <param name="size">size</param>
        public void AddParameter(string name, object value, DbType? dbtype, ParameterDirection? direction, int? size)
        {
            if (_params == null) { _params = new DynamicParameters(); }

            _params.Add(name, value, dbType: dbtype, direction: direction, size: size);
        }

        /// <summary>
        /// DynamicParameters 추가
        /// </summary>
        /// <param name="name">Parameter 명</param>
        /// <param name="value">Parameter 값</param>
        /// <param name="dbtype">Parameter 개체의 데이터 형식</param>
        /// <param name="direction">매개 변수의 형식</param>
        /// <param name="size">size</param>
        /// <param name="precision">precision</param>
        /// <param name="scale">scale</param>
        public void AddParameter(string name, object value = null, DbType? dbtype = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null)
        {
            if (_params == null) { _params = new DynamicParameters(); }

            _params.Add(name, value: value, dbType: dbtype, direction: direction, size: size, precision: precision, scale: scale);
        }

        /// <summary>
        /// Output 파라메터 값 반환
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="name">Parameter 명</param>
        /// <returns></returns>
        public T GetParameter<T>(string name)
        {
            return _params.Get<T>(name);
        }

        /// <summary>
        /// Celaer DynamicParameters 
        /// </summary>
        public void ClearParameter()
        {
            if (_params == null) { return; }

            _params = new DynamicParameters();
        }
        #endregion

        #region [Execute]
        /// <summary>
        /// 연결에 대한 Transact-SQL 문(INSERT, UPDATE, DELETE)을 실행하고 영향을 받는 행의 수를 반환합니다.
        /// </summary>
        /// <param name="sql">Transact-SQL statement, stored procedure</param>
        /// <param name="param"></param>
        /// <param name="commandType">CommandType</param>
        /// <param name="IsTransRequired">트랜잭션 처리 유무</param>
        /// <param name="commandTimeout">명령 실행을 종료하고 오류를 생성하기 전 대기 시간</param>
        /// <returns>영향 받는 행의 수</returns>
        public int Execute(string sql, object param = null, CommandType? commandType = default(CommandType?), bool IsTransRequired = false, int? commandTimeout = default(int?))
        {
            int affectedRowsCnt = 0;
            IDbTransaction transaction = null;
            try
            {
                if (IsTransRequired) {
                    _connection.Open();
                    transaction = _connection.BeginTransaction();
                }
                affectedRowsCnt = _connection.Execute(sql, param, transaction, commandTimeout, commandType);

                if (IsTransRequired) transaction.Commit();
            }
            catch (Exception ex) {
                if(IsTransRequired) transaction.Rollback();

                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }
            return affectedRowsCnt;
        }

        public object ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            object obj = null;
            try
            {
                obj = _connection.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }
            return obj;
        }
        #endregion

        #region [Query]
        /// <summary>
        /// 데이터베이스에서 정보를 추출하고 비즈니스 객체 모델에 채워 반환
        /// </summary>
        /// <param name="sql">Transact-SQL statement, stored procedure</param>
        /// <param name="param">파라메터 (ex: new {param1 = 1, param2 = "문자"})</param>
        /// <param name="commandType">CommandType</param>
        /// <returns>dynamic 열거형</returns>
        public IEnumerable<dynamic> Query(string sql, object param = null, CommandType? commandType = default(CommandType?))
        {
            try
            {
                return _connection.Query(sql, param, commandType: commandType ?? CommandType.Text);
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }
            return null;
        }

        /// <summary>
        /// 데이터베이스에서 정보를 추출하고 비즈니스 객체 모델에 채워 반환
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="sql">쿼리문 또는 SP명</param>
        /// <param name="param">파라메터 (ex: new {param1 = 1, param2 = "문자"})</param>
        /// <param name="commandType">CommandType</param>
        /// <returns>Generic object 열거형</returns>
        public IEnumerable<T> Query<T>(string sql, object param = null, CommandType? commandType = default(CommandType?))
        {
            try
            {
                return _connection.Query<T>(sql, param, commandType: commandType ?? CommandType.Text);
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }
            return null;
        }
        #endregion

        #region [QuerySingle]
        /// <summary>
        /// 단일 행을 결과로 반환
        /// </summary>
        /// <param name="sql">쿼리문 또는 SP명</param>
        /// <param name="param">파라메터 (ex: new {param1 = 1, param2 = "문자"})</param>
        /// <param name="commandType">CommandType</param>
        /// <returns>dynamic</returns>
        public dynamic QuerySingle(string sql, object param = null, CommandType? commandType = default(CommandType?))
        {
            try
            {
                return _connection.Query(sql, param, commandType: commandType ?? CommandType.Text).FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }
            return null;
        }

        /// <summary>
        /// 단일 행을 결과로 반환
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="sql">쿼리문 또는 SP명</param>
        /// <param name="param">파라메터 (ex: new {param1 = 1, param2 = "문자"})</param>
        /// <param name="commandType">CommandType</param>
        /// <returns>Generic object</returns>
        public T QuerySingle<T>(string sql, object param = null, CommandType? commandType = default(CommandType?))
        {
            try
            {
                return _connection.Query<T>(sql, param, commandType: commandType ?? CommandType.Text).FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (_handleErrors) _lstError = ex.Message;
                else throw ex; // CustomException System, ex
            }
            return default(T);
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
                _connection.Close();
                _connection.Dispose();
                _connection = null;
                _provider = null;
            }

            // Free any unmanaged objects(all native resources) here.
            //
            disposed = true;
        }

        //// Implement a finalizer by using destructor style syntax
        //~DBHelper2()
        //{
        //    // Call the overridden Dispose method that contains common cleanup code
        //    // Pass false to indicate the it is not called from Dispose
        //    Dispose(false);
        //}
        #endregion

    }





    #region SqlMapper.ITypeMap

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DapperColumnAttribute : System.Attribute
    {
        public string DbName { get; set; }

        public DapperColumnAttribute(string pDbName)
        {
            DbName = pDbName;
        }
    }

    /// <summary>
    /// Uses the Name value of the <see cref="ColumnAttribute"/> specified to determine
    /// the association between the name of the column in the query results and the member to
    /// which it will be extracted. If no column mapping is present all members are mapped as
    /// usual.
    /// </summary>
    /// <typeparam name="T">The type of the object that this association between the mapper applies to.</typeparam>
    public class ColumnAttributeTypeMapper<T> : FallbackTypeMapper
    {
        public ColumnAttributeTypeMapper()
            : base(new SqlMapper.ITypeMap[]
                {
                    new CustomPropertyTypeMap(
                       typeof(T),
                       (type, columnName) =>
                           type.GetProperties().FirstOrDefault(prop =>
                               prop.GetCustomAttributes(false)
                                   .OfType<DapperColumnAttribute>()
                                   .Any(attr => attr.DbName == columnName)
                               )
                       ),
                    new DefaultTypeMap(typeof(T))
                })
        {
        }
    }

    public class FallbackTypeMapper : SqlMapper.ITypeMap
    {
        private readonly IEnumerable<SqlMapper.ITypeMap> _mappers;

        public FallbackTypeMapper(IEnumerable<SqlMapper.ITypeMap> mappers)
        {
            _mappers = mappers;
        }


        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    ConstructorInfo result = mapper.FindConstructor(names, types);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (NotImplementedException)
                {
                }
            }
            return null;
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    var result = mapper.GetConstructorParameter(constructor, columnName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (NotImplementedException)
                {
                }
            }
            return null;
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            foreach (var mapper in _mappers)
            {
                try
                {
                    var result = mapper.GetMember(columnName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (NotImplementedException)
                {
                }
            }
            return null;
        }


        public ConstructorInfo FindExplicitConstructor()
        {
            return _mappers
                .Select(mapper => mapper.FindExplicitConstructor())
                .FirstOrDefault(result => result != null);
        }
    }

    #endregion SqlMapper.ITypeMap
}
