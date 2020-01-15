using System;
using System.Web;
using System.Web.Mvc;
using WCMS.Data;
using WCMS.Web.Helpers;
using WCMS.Bussiness;
using System.Collections.Generic;
using PagedList;
using System.Drawing;
using System.IO;

namespace WCMS.Web.Controllers
{
    public class ImageController : BaseController
    {
        private int _pageSize = 5;
        private string _defaultPath = "D:/images/default.png";

        private readonly BizImage _bizImage = new BizImage();
        
        [HttpPost]
        public JsonResult ImageUpdate(string jsonData)
        {
            string memberId = HttpContext.Session["USER_NAME"].ToString();

            try
            {
                var settings = Settings.newtonsoftSetting();
                ImageData imageData = new ImageData();

                if (!string.IsNullOrEmpty(jsonData))
                {
                    imageData = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageData>(jsonData, settings);
                }

                int result = _bizImage.UpdateImageData(imageData, memberId);

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { STATUS = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        
        public ActionResult GetImage(int idx)
        {
            string path = string.Empty;

            if (idx > 0)
            { 
                ImageData imageData = _bizImage.GetImageData(idx);
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageData.imagePath, imageData.imageName);
            }
            else
            {
                path = _defaultPath;
            }

            FileStream stream = new FileStream(path, FileMode.Open);

            return new FileStreamResult(stream, "image/png");
        }

        public ActionResult ImageSerachList(int? page, string keyword = "")
        {
            if (!LoginCheckBool()) return RedirectToAction("Login", "Account");

            int pageNo = (page ?? 1);

            List<ImageData> imageDatas = _bizImage.GetImageList(keyword);

            return View(imageDatas.ToPagedList(pageNo, _pageSize));
        }

        public ActionResult ImageAdd()
        {

            return LoginCheck();
        }

        // 이미지 등록 프로세스 페이지 
        [HttpPost]
        public ActionResult ImageAdd(HttpPostedFileBase file,string keyword)
        {
            if(!LoginCheckBool()) return RedirectToAction("Login", "Account");

            if (file != null)
            {
                
                string fileExtensionType = string.Empty;
                string fileName = string.Empty;
                string subPath = string.Empty;
                string saveFileName = string.Empty;
                string fullPath = string.Empty;
                string memberId = HttpContext.Session["USER_NAME"].ToString();

                FileUploadStatus uploadStatus = FileUploader.FileSave(file,ref fileExtensionType, ref fileName,ref subPath,ref saveFileName,ref fullPath);

                switch (uploadStatus)
                {
                    case FileUploadStatus.SYSTEM_EXCEPTION: ViewBag.message = "Could not get file information."; break;
                    case FileUploadStatus.ZERO_BYTE: ViewBag.message = "File Size is Zero."; break;
                    case FileUploadStatus.NOT_SUPPORT_EXTENSIONTYPE: ViewBag.message = "File format is not supported."; break;
                    case FileUploadStatus.OK: 
                    default: ViewBag.message = "Image upload complete"; break;
                }

                ImageData imageData = new ImageData();
                imageData.imageKeyword = string.IsNullOrEmpty(keyword) ? fileName : keyword;
                imageData.imagePath = subPath;
                imageData.imageName = saveFileName;

                string result = _bizImage.SetImageData(imageData, memberId);

                ViewBag.message = result.Equals("0") ? "The same image name exists." : ViewBag.message;
            }
            else
            {
                ViewBag.message = "Could not get file information.";
            }

            return View();
        }


        [HttpPost]
        public JsonResult GetImageAddPath(HttpPostedFileBase fileInfo)
        {

            try
            {
                string returnMessage = string.Empty;
                string returnStatus = "F";

                if (fileInfo != null)
                {

                    string fileExtensionType = string.Empty;
                    string fileName = string.Empty;
                    string subPath = string.Empty;
                    string saveFileName = string.Empty;
                    string fullPath = string.Empty;
                    string memberId = HttpContext.Session["USER_NAME"].ToString();

                    FileUploadStatus uploadStatus = FileUploader.FileSave(fileInfo, ref fileExtensionType, ref fileName, ref subPath, ref saveFileName, ref fullPath);

                    switch (uploadStatus)
                    {
                        case FileUploadStatus.SYSTEM_EXCEPTION: returnMessage = "Could not get file information."; break;
                        case FileUploadStatus.ZERO_BYTE: returnMessage = "File Size is Zero."; break;
                        case FileUploadStatus.NOT_SUPPORT_EXTENSIONTYPE: returnMessage = "File format is not supported."; break;
                        case FileUploadStatus.OK:
                        default: returnMessage = fullPath; returnStatus = "S";  break;
                    }

                    var settings = Settings.newtonsoftSetting();
                    ImageData imageData = new ImageData();
                    imageData.imageKeyword = fileName;
                    imageData.imagePath = subPath;
                    imageData.imageName = saveFileName;

                    string result = _bizImage.SetImageData(imageData, memberId);

                    
                    if (result.Equals("0"))
                    {
                        returnMessage = "The same image name exists.";
                        returnStatus = "F";
                    }
                }
                else
                {
                    returnStatus = "F";
                    returnMessage = "Could not get file information.";
                }

                return Json(new { Message = returnMessage , Status = returnStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "F" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}