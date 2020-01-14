﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using WCMS.Bussiness;
using WCMS.Web.Models;

namespace WCMS.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly BizMember _bizMember = new BizMember();

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public string Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var member = _bizMember.GetLoginData(model.memberId, model.memberPw);
                if (member != null)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.Id = member.memberNo.ToString();
                    user.UserName = model.memberId;
                    HttpContext.Session.Add("USER_LOGIN_KEY", member.memberNo);
                    HttpContext.Session.Add("USER_NAME", member.memberId);
                    return "S";
                    
                }
                else return "F";
            }
            return "P";
            // 이 경우 오류가 발생한 것이므로 폼을 다시 표시하십시오.
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public string Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                string result = _bizMember.SetSignUp(model.memberId, model.memberPw, model.memberName, model.memberPhone);

                return result.Equals("0") ? "F" : "S";
                
            }

            // 이 경우 오류가 발생한 것이므로 폼을 다시 표시하십시오.
            return "P";
        }


        // /Account/LogOff
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            HttpContext.Session.Abandon();
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Remove("ASP.NET_SessionId");
            return RedirectToAction("Login", "Account");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region 도우미

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

        }
        #endregion
    }
}