using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WCMS.Data;

namespace WCMS.DAC
{
    public class DacImage : WCMS.DAC.DacBase
    {
        public DacImage(string connectionString)
        {
            this.Connection = new SqlConnection(connectionString);
        }

        public string SetImageData(ImageData imageData, string memberId)
        {
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@ImageName", imageData.imageName, DbType.String);
            queryParam.Add("@ImagePath", imageData.imagePath, DbType.String);
            queryParam.Add("@ImageKeyword", imageData.imageKeyword, DbType.String);
            queryParam.Add("@memberId", memberId, DbType.String);

            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    return dbConnection.Query<string>("uspSet_Image_Insert", queryParam, commandType: CommandType.StoredProcedure).First();
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

        public List<ImageData> GetImageList( string imageKeyword)
        {
            
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@ImageKeyword", imageKeyword, DbType.String);

            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    var imageList = dbConnection.Query<ImageData>("uspGet_Image_List", queryParam, commandType: CommandType.StoredProcedure).ToList();
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


        public ImageData GetImageData(int idx)
        {
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@idx", idx, DbType.String);

            try
            {
                using (IDbConnection dbConnection = this.Connection)
                {
                    return dbConnection.Query<ImageData>("uspGet_Image_Select", queryParam, commandType: CommandType.StoredProcedure).First();
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
