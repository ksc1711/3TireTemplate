using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using WCMS.FrameWork.Data.Mapper.Dapper;

using WCMS.Data;
using Dapper;

namespace WCMS.DAC
{
    public class DacContent : WCMS.DAC.DacBase
    {
        public DacContent(string connectionString)
        {
            this._connection = new SqlConnection(connectionString);
            this._DapperHelper = new DapperHelper(connectionString, true, false);
        }
        public DacContent(SqlConnection connection)
        {
            this._connection = connection;
            this._DapperHelper = new DapperHelper(connection.ConnectionString, true, false);
        }

        #region Content List
        public List<Content> GetContentList(Content pContent)
        {

    

            try
            {
                _DapperHelper.Connection.Open();
                _DapperHelper.ClearParameter();

                _DapperHelper.AddParameter("@FileName", pContent.FileName, DbType.String, ParameterDirection.Input);
                _DapperHelper.AddParameter("@FolderName", pContent.FolderName, DbType.String, ParameterDirection.Input);

                return _DapperHelper.Query<Content>(
                    "AdventureWorks2008.dbo.usp_CMS_Contents_Info_List",
                    _DapperHelper.Params,
                    CommandType.StoredProcedure
                ).ToList();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _DapperHelper.Connection.Close();
            }
        }
        #endregion

    }
}
