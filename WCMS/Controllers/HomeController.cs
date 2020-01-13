using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WCMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return LoginCheck();
        }

        public ActionResult test()
        {

            return View();
        }
    }
}