using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WCMS.Data;
using Dapper;
using System.Linq;

namespace WCMS.DAC
{
    public class DacMember : WCMS.DAC.DacBase
    {
        public DacMember(string connectionString)
        {
            this.Connection = new SqlConnection(connectionString);
        }

        public MemberData GetLoginData(string memberId, string memberPw)
        {
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@memberId", memberId, DbType.String);
            queryParam.Add("@memberPw", memberPw, DbType.String);

            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    return dbConnection.Query<MemberData>("", queryParam, commandType: CommandType.StoredProcedure).ToList()[0];

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
