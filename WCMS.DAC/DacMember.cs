using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WCMS.Data;

namespace WCMS.DAC
{
    class DacMember : WCMS.DAC.DacBase
    {
        public DacMember(string connectionString)
        {
            this.GetDbConnection = new SqlConnection(connectionString);
        }

        public List<MemberData> GetLoginData(MemberData pContent)
        {
            try
            {
                //this.GetDbConnection
                /*
                _DapperHelper.Connection.Open();
                _DapperHelper.ClearParameter();

                _DapperHelper.AddParameter("@FileName", pContent.FileName, DbType.String, ParameterDirection.Input);
                _DapperHelper.AddParameter("@FolderName", pContent.FolderName, DbType.String, ParameterDirection.Input);

                return _DapperHelper.Query<Content>(
                    "AdventureWorks2008.dbo.usp_CMS_Contents_Info_List",
                    _DapperHelper.Params,
                    CommandType.StoredProcedure
                ).ToList();
                */
                return null;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                //_DapperHelper.Connection.Close();
            }
        }

    }
}
