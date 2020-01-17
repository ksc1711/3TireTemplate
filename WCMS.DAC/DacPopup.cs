using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WCMS.Data;

namespace WCMS.DAC
{
    public class DacPopup : WCMS.DAC.DacBase
    {
        public DacPopup(string connectionString)
        {
            this.Connection = new SqlConnection(connectionString);
        }

        public int SetPopupData(PopupData popupData, string memberId)
        {
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@title", popupData.title, DbType.String);
            queryParam.Add("@startDate", popupData.startDate, DbType.String);
            queryParam.Add("@endDate", popupData.endDate, DbType.String);
            queryParam.Add("@joinMember", popupData.joinMember, DbType.String);
            queryParam.Add("@popupType", popupData.popupType, DbType.String);
            queryParam.Add("@descript", popupData.descript, DbType.String);
            queryParam.Add("@popupHtml", popupData.popupHtml, DbType.String);
            queryParam.Add("@popupScript", popupData.popupScript, DbType.String);
            queryParam.Add("@memberId", memberId, DbType.String);

            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    return dbConnection.Query<int>("uspSet_Popup_Insert", queryParam, commandType: CommandType.StoredProcedure).First();
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

        // 전체 팝업 검색 
        public List<PopupData> GetPopupList()
        {
            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    var popupDatas = dbConnection.Query<PopupData>("uspGet_Popup_List", commandType: CommandType.StoredProcedure).ToList();
                    return popupDatas;
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

        // 파라메터별 팝업 검색 
        public List<PopupData> GetPopupList(PopupData popupData)
        {
            
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@keyword", popupData.title, DbType.String);
            queryParam.Add("@startDate", popupData.startDate, DbType.String);
            queryParam.Add("@endDate", popupData.endDate, DbType.String);
            queryParam.Add("@joinMember", popupData.joinMember, DbType.String);
            queryParam.Add("@popupType", popupData.popupType, DbType.String);
            queryParam.Add("@regUser", popupData.regUser, DbType.String);
            queryParam.Add("@idx", popupData.idx, DbType.Int32);

            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    var popupDatas = dbConnection.Query<PopupData>("uspGet_Popup_List", queryParam, commandType: CommandType.StoredProcedure).ToList();
                    return popupDatas;
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
