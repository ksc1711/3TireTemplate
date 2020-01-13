using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WCMS.Bussiness;
using WCMS.Data;
using WCMS.Web;


namespace WCMS.Web.Controllers
{
    public class ImageController : BaseController
    {
        private readonly BizCotent _content = new BizCotent();
        //
        // GET: /ImageUpload/
        public ActionResult ImageUpload()
        {
            return LoginCheck();
        }

        public ActionResult ImageSerach()
        {
            return LoginCheck();
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
        public ActionResult ImageAdd(HttpPostedFileBase file)
        {
            ViewBag.message = "Image upload complete";

            if (file != null)
            {
                string fileExtensionType = string.Empty;
                string fileName = string.Empty;
                string subPath = string.Empty;
                string saveFileName = string.Empty;
                string fullPath = string.Empty;

                FileUploadStatus uploadStatus = FileUploader.FileSave(file,ref fileExtensionType, ref fileName,ref subPath,ref saveFileName,ref fullPath);

                switch (uploadStatus)
                {
                    case FileUploadStatus.SYSTEM_EXCEPTION: ViewBag.message = "Could not get file information."; break;
                    case FileUploadStatus.ZERO_BYTE: ViewBag.message = "File Size is Zero."; break;
                    case FileUploadStatus.NOT_SUPPORT_EXTENSIONTYPE: ViewBag.message = "File format is not supported."; break;
                    case FileUploadStatus.OK: 
                    default: ViewBag.message = "Image upload complete"; break;

                }
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

                var List = _content.GetContentList(content);

                return Json(new { draw = draw, data = List }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { STATUS = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




	}
}