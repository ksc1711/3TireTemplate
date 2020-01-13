using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WCMS.Web.Controllers
{
    public class BaseController : Controller
    {

        public ActionResult  LoginCheck()
        {
            if (HttpContext.Session["USER_LOGIN_KEY"] == null)
            {
                //로그인이 안된 상태
               return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public bool LoginCheckBool()
        {
            if (HttpContext.Session["USER_LOGIN_KEY"] == null)
            {
                //로그인이 안된 상태
                return false;
            }

            return true;

        }
    }
}