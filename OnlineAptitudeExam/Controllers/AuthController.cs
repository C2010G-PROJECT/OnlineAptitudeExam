﻿using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineAptitudeExam.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {

        private OnlineAptitudeExamEntities dbEntities = new OnlineAptitudeExamEntities();

        public AuthController()
        {
        }


        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        //
        // GET: /Auth/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Session["UserInfo"] is Account)
            {
                Account user = Session["UserInfo"] as Account;
                if (user.type.Equals((int)Enums.Type.USER))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }

        //
        // POST: /Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string password = Helper.GetMD5(model.Password);
                var user = dbEntities.Accounts.Where(x =>
                x.username.Equals(model.UserName) &&
                x.password.Equals(password)).FirstOrDefault();

                if (user != null)
                {
                    Session["UserInfo"] = user;
                    bool isAdmin = user.type == ((int)Enums.Type.ADMIN);
                    if (isAdmin)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = "Username or password is incorrect!";
                }
            }
            return View();
        }

        //
        // GET: /Auth/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Session["UserInfo"] is Account)
            {
                Account user = Session["UserInfo"] as Account;
                if (user.type.Equals((int)Enums.Type.USER))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }

        //
        // POST: /Auth/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new Account();
                user.username = model.UserName;
                user.fullname = model.FullName;
                user.password = Helper.GetMD5(model.ConfirmPassword);
                user.status = 0;
                user.type = 1;

                var checkAuthExist = dbEntities.Accounts.FirstOrDefault(x => x.username == user.username);

                if (checkAuthExist == null)
                {
                    dbEntities.Accounts.Add(user);
                    dbEntities.SaveChanges();
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    ViewBag.error = "Username already exists.";
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        // POST: /Auth/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/LogOut
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        //
        // GET: /Auth/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
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

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}