using System;
using System.Web.Mvc;
using WCMS.Bussiness;
using WCMS.Data;

namespace WCMS.Web.Controllers
{

    public class PopupController : BaseController
    {
        private readonly BizMember _bizMember = new BizMember();
        private readonly BizPopup _bizPopup = new BizPopup();

        // GET: Popup
        public ActionResult PopUpList()
        {
            if (!LoginCheckBool()) return RedirectToAction("Login", "Account");

            var members = _bizMember.GetLoginList("SDT");

            ViewBag.Members = members;

            return View();
        }

        [HttpGet]
        public ActionResult PopUpAdd()
        {
            if (!LoginCheckBool()) return RedirectToAction("Login", "Account");

            var members = _bizMember.GetLoginList("SDT");

            ViewBag.ImageLinks = "";

            return View(members);
        }


        [HttpPost]
        public ActionResult PopUpAdd(string ImageLinks)
        {
            if (!LoginCheckBool()) return RedirectToAction("Login", "Account");

            string[] imageLinkArray = { };
            if (!string.IsNullOrEmpty(ImageLinks))
            {
                imageLinkArray = ImageLinks.Split('|');
            }

            ViewBag.ImageLinks = imageLinkArray;

            var members = _bizMember.GetLoginList("SDT");

            return View(members);
        }

        [HttpPost]
        public JsonResult PopUpAddProc(string jsonData)
        {
            string memberId = HttpContext.Session["USER_NAME"].ToString();
            int result = 0;

            try
            {
                if (!string.IsNullOrEmpty(jsonData))
                {
                    var settings = Settings.newtonsoftSetting();

                    PopupData popupData = new PopupData();
                    popupData = Newtonsoft.Json.JsonConvert.DeserializeObject<PopupData>(jsonData, settings);

                    result = _bizPopup.SetPopupData(popupData, memberId);
                }

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}