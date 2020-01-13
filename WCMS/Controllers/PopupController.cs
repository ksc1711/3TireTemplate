using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WCMS.Web.Controllers
{
    public class PopupController : BaseController
    {
        // GET: Popup
        public ActionResult PopUpList()
        {
            return LoginCheck();
        }
    }
}