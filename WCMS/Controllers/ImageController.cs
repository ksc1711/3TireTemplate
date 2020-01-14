using System;
using System.Web;
using System.Web.Mvc;
using WCMS.Data;
using WCMS.Web.Helpers;
using WCMS.Bussiness;
using System.Collections.Generic;
using PagedList;

namespace WCMS.Web.Controllers
{
    public class ImageController : BaseController
    {
        private int _pageSize = 5;

        private readonly BizImage _bizImage = new BizImage();
        //
        // GET: /ImageUpload/
        public ActionResult ImageUpload()
        {
            return LoginCheck();
        }

        public ActionResult ImageSerachList(int? page, string imageKeyword = "")
        {
            

            if (!LoginCheckBool()) return RedirectToAction("Login", "Account");

            int pageNo = (page ?? 1);

            int totalCnt = 0;
            int pageCount = 0;

            List<ImageData> imageDatas = _bizImage.GetImageList(imageKeyword);

            // 페이징 갯수 
            int pagingCount = (totalCnt==0 ? 1: totalCnt) / _pageSize;
            ViewBag.pageCount = pagingCount;
            ViewBag.pageDataCount = pageCount;

            return View(imageDatas.ToPagedList(pageNo, _pageSize));
        }

        public ActionResult ImageDetail()
        {
            return LoginCheck();
        }
        public ActionResult ImageAdd()
        {

            return LoginCheck();
        }

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

                ViewBag.message = result.Equals("0") ? "The same image name exists." : "Image upload complete";
            }
            else
            {
                ViewBag.message = "Could not get file information.";
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetContentList(int draw, string pJsonString)
        {
            try
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
                settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                settings.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime;
                //settings.Culture = new System.Globalization.CultureInfo("utf-8");
                ContentData content = new ContentData();

                if (!string.IsNullOrEmpty(pJsonString))
                {
                    content = Newtonsoft.Json.JsonConvert.DeserializeObject<ContentData>(pJsonString, settings);
                }

               var List = new { };//_content.GetContentList(content);

                return Json(new { draw = draw, data = List }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { STATUS = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




	}
}