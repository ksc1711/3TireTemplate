using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WCMS.Bussiness;
using WCMS.Data;

namespace WCMS.Web.Controllers
{

    public class PopupController : BaseController
    {
        private int _pageSize = 10;

        private readonly BizMember _bizMember = new BizMember();
        private readonly BizPopup _bizPopup = new BizPopup();
        private readonly BizCommon _bizCommon = new BizCommon();

        // GET: Popup
        public ActionResult PopUpList(int? page)
        {
            if (!LoginCheckBool()) return RedirectToAction("Login", "Account");

            // 참여 인원 데이터 
            var members = _bizMember.GetLoginList("SDT");
            ViewBag.Members = members;

            // 팝업 타입 데이터 
            var commonCodes = _bizCommon.GetCommonCodeData("104");
            ViewBag.PopupTypes = commonCodes;

            // 팝업 리스트
            List<PopupData> popupDatas = _bizPopup.GetPopupList();

            int pageNo = (page ?? 1);

            return View(popupDatas.ToPagedList(pageNo, _pageSize));
        }

        [HttpPost]
        public ActionResult PopUpList(int? page, PopupData popupData)
        {
            if (!LoginCheckBool()) return RedirectToAction("Login", "Account");

            // 참여 인원 데이터 
            var members = _bizMember.GetLoginList("SDT");
            ViewBag.Members = members;

            // 팝업 타입 데이터 
            var commonCodes = _bizCommon.GetCommonCodeData("104");
            ViewBag.PopupTypes = commonCodes;

            int pageNo = (page ?? 1);

            // 팝업 리스트
            List<PopupData> popupDatas = _bizPopup.GetPopupList(popupData);

            return View(popupDatas.ToPagedList(pageNo, _pageSize));
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