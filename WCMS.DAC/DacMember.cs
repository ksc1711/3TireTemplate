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
                    return dbConnection.Query<MemberData>("uspGet_Member_Select", queryParam, commandType: CommandType.StoredProcedure).ToList()[0];

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

        public string GetSignUp(string memberId, string memberPw, string memberName, string memberPhone)
        {
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@memberId", memberId, DbType.String);
            queryParam.Add("@memberPw", memberPw, DbType.String);
            queryParam.Add("@memberName", memberName, DbType.String);
            queryParam.Add("@memberPhone", memberPhone, DbType.String);

            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    return dbConnection.Query<string>("uspSet_Member_Insert", queryParam, commandType: CommandType.StoredProcedure).First();

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
