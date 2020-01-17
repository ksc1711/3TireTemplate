using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WCMS.Data;

namespace WCMS.DAC
{
    public class DacCommon : WCMS.DAC.DacBase
    {
        public DacCommon(string connectionString)
        {
            this.Connection = new SqlConnection(connectionString);
        }

        // 코드 타입별 공통 코드 리스트 
        public List<CommonCodeData> GetCommonCodeData(string commonCode)
        {

            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@commonCode", commonCode, DbType.String);

            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    var imageList = dbConnection.Query<CommonCodeData>("uspGet_Common_Code_List", queryParam, commandType: CommandType.StoredProcedure).ToList();
                    return imageList;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                this.Connection.Close();
            }
        }
    }
}
