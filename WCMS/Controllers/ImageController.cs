using System;
using System.Web.Mvc;
using WCMS.Bussiness;
using WCMS.Data;

namespace WCMS.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly BizCommon _common = new BizCommon();
        private readonly BizCotent _content = new BizCotent();
        //
        // GET: /ImageUpload/
        public ActionResult ImageUpload()
        {
            return View();
        }

        public ActionResult ImageSerach()
        {
            return View();
        }
        public ActionResult ImageDetail()
        {
            return View();
        }
        public ActionResult CategoryAdd()
        {
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