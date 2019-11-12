using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WCMS.FrameWork.Data.Mapper.Dapper;
using Dapper;

namespace WCMS.DAC
{
    public abstract class DacBase : IDisposable
    {
        private SqlConnection _sqlConnection;
        private DapperHelper _dapperHelper;


        public SqlConnection _connection
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

        public DapperHelper _DapperHelper
        {
            get
            {
                return _dapperHelper;
            }
            set
            {
                _dapperHelper = value;
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
                    _sqlConnection.Dispose();
                    _sqlConnection = null;
                    _DapperHelper.Dispose();
                    _DapperHelper = null;
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
