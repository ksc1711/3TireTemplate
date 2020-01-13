using WCMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace WCMS.Web
{
    public class FileUploader
    {
        /// <summary>
        /// 파일 저장
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="fileExtensionType"></param>
        /// <param name="fileName"></param>
        /// <param name="fileStoreLocation"></param>
        /// <returns>
        /// </returns>
        public static FileUploadStatus FileSave(HttpPostedFileBase targetFile, ref string fileExtensionType, ref string fileName, ref string subPath, ref string saveFileName, ref string fullPath)
        {
            if (targetFile.ContentLength > 0)
            {
                try
                {
                    fileExtensionType = GetFileExtensionTypeCode(Path.GetExtension(targetFile.FileName).ToLower());

                    if (string.IsNullOrEmpty(fileExtensionType))
                    {
                        //지원하지 않는 확장자
                        return FileUploadStatus.NOT_SUPPORT_EXTENSIONTYPE;
                    }

                    fileName = Path.GetFileName(targetFile.FileName);

                    // 유니크한 파일명 생성
                    string guid = Guid.NewGuid().ToString();
                    saveFileName = string.Format("{0}.{1}", guid, fileExtensionType);

                    subPath = DateTime.Now.ToString("yyyyMMdd");
                    string serverPath = Path.Combine(GetFileSaveLocalPath(subPath), saveFileName);
                    fullPath = serverPath;
                    // File Save
                    targetFile.SaveAs(serverPath);

                }
                catch
                {
                    return FileUploadStatus.SYSTEM_EXCEPTION;
                }

                return FileUploadStatus.OK;
            }
            else
            {
                return FileUploadStatus.ZERO_BYTE;
            }
        }

        /// <summary>
        /// 파일 확장자에 해당하는 FileExtensionTypeCode를 가져온다.
        /// </summary>
        /// <param name="fileExtension">파일 확장자</param>
        /// <returns>
        /// fileExtentionTypeCode
        /// string.Empty인 경우 지원하지 않는 파일 확장자
        /// </returns>
        public static string GetFileExtensionTypeCode(string fileExtension)
        {
            string extentionTypeCode = string.Empty;

            switch (Path.GetExtension(fileExtension))
            {
                case ".gif":
                    extentionTypeCode = "gif";
                    break;
                case ".bmp":
                    extentionTypeCode = "bmp";
                    break;
                case ".jpg":
                    extentionTypeCode = "jpg";
                    break;
                case ".png":
                    extentionTypeCode = "png";
                    break;
                default:
                    extentionTypeCode = string.Empty;
                    break;
            }

            return extentionTypeCode;
        }


        /// <summary>
        /// 첨부파일을 저장할 서버의 로컬경로를 가져온다.
        /// </summary>
        /// <returns></returns>
        public static string GetFileSaveLocalPath(string subPath)
        {
            string saveFilePath = ConfigurationManager.AppSettings["FileUpload"].ToString() + subPath + "\\";

            if (Directory.Exists(saveFilePath) == false)
            {
                Directory.CreateDirectory(saveFilePath);
            }
            return saveFilePath;
        }
    }

    public enum FileUploadStatus
    {
        OK = 0,

        ZERO_BYTE = 1,

        NOT_SUPPORT_EXTENSIONTYPE = 2,

        SYSTEM_EXCEPTION = 4,
    }
}