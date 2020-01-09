using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace WCMS.DAC
{
    public abstract class DacBase : IDisposable
    {
        private IDbConnection _sqlConnection;
        /* Dapper ORM을 활용한 DB접근 Base 클래스
          Base를 생성한것은 DB마다 접근 계정 및 DB가 다를경우를 
          감안하여 생성. 
             */

        public IDbConnection Connection
        {
            get
            {
                return _sqlConnection;
            }
            set
            {
                _sqlConnection = value;
            }
        }
    
    bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                if (_sqlConnection != null)
                {
                    //_sqlConnection.Dispose();
                    //_sqlConnection = null;
                    _sqlConnection.Dispose();
                    _sqlConnection = null;
                }
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        ~DacBase()
        {
            Dispose(false);
        }
    }
}
