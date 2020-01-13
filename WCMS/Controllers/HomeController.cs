using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WCMS.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (HttpContext.Session["USER_LOGIN_KEY"] == null)
            {
                //로그인이 안된 상태
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public ActionResult test()
        {

            return View();
        }
    }
}