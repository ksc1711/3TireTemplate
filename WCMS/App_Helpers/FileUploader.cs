using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Web;
using ImageMagick;

namespace WCMS.Web.Helpers
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

                    subPath = GetFileSaveLocalPath(DateTime.Now.ToString("yyyyMMdd"));
                    string serverPath = Path.Combine(subPath, saveFileName);
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

        public static void ImageResize(StringBuilder _sb, FileInfo _fi, Image _image)
        {
            int ResizeNum = Convert.ToInt32(ConfigurationManager.AppSettings["ResizeNum"]);

            bool FolderDesignate = Convert.ToBoolean(ConfigurationManager.AppSettings["ResizeFolder_Designate"]);
            string ResizeFolder = ConfigurationManager.AppSettings["ResizeFolder"].ToString();

            // target folder가 없을 시 폴더 생성
            if (!Directory.Exists(ResizeFolder))
                Directory.CreateDirectory(ResizeFolder);

            for (int i = 1; i <= ResizeNum; i++)
            {
                _sb.Clear();

                string ImageWidth = "ImageWidth" + i;
                string ImageHeight = "ImageHeight" + i;

                int rImageWidth = Convert.ToInt32(ConfigurationManager.AppSettings[ImageWidth]);
                int rImageHeight = Convert.ToInt32(ConfigurationManager.AppSettings[ImageHeight]);

                using (Bitmap imgBitmap = new Bitmap(rImageWidth, rImageHeight))
                {
                    // * resize
                    Graphics graphics = Graphics.FromImage(imgBitmap);
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    graphics.DrawImage(_image, 0, 0, rImageWidth, rImageHeight);
                    //filename.
                    string file = _fi.Name.Clone().ToString();
                    file = file.Substring(0, file.LastIndexOf('.'));

                    //resize folder 지정
                    string paths = string.Empty;
                    if (FolderDesignate)
                        paths = _sb.AppendFormat("{0}\\{1}({2}){3}", ResizeFolder, file, i, _fi.Extension).ToString();
                    else
                        paths = _sb.AppendFormat("{0}\\{1}({2}){3}", _fi.DirectoryName, file, i, _fi.Extension).ToString();

                    switch (_fi.Extension)
                    {
                        case ".gif":
                            {
                                using (MagickImageCollection collection = new MagickImageCollection(_fi))
                                {
                                    // This will remove the optimization and change the image to how it looks at that point
                                    // during the animation. More info here: http://www.imagemagick.org/Usage/anim_basics/#coalesce
                                    collection.Coalesce();

                                    // Resize each image in the collection to a width of 200. When zero is specified for the height
                                    // the height will be calculated with the aspect ratio.
                                    foreach (MagickImage image in collection)
                                    {
                                        image.Resize(rImageWidth, rImageHeight);
                                    }

                                    // Save the result
                                    collection.Write(paths);
                                }
                            }
                            break;
                        default:
                            imgBitmap.Save(paths);
                            break;
                    }
                    //imgBitmap.Save(paths);
                    //, System.Drawing.Imaging.ImageFormat.Jpeg
                    graphics.Dispose();
                }
            }
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