using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineAptitudeExam.Models;

namespace OnlineAptitudeExam.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        private static String ADMIN_NAME = "Admin";
        private static String ADMIN_PASSWORD = "Admin123456";

        private static int ADMIN = 0;
        private static int USER = 1;

        private static int LOCK = 1;
        private static int UNLOCK = 0;


        private OnlineAptitudeExamEntities dbEntities = new OnlineAptitudeExamEntities();

        public AccountController()
        {
        }


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
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var passwordF = GetMD5(model.Password);

                var dataUser = dbEntities.Users.Where(x => x.username.Equals(model.UserName) &&
                x.password.Equals(passwordF) && x.type == USER).ToList();

                var dataAdmin = dbEntities.Users.Where(x => x.username.Equals(model.UserName) &&
               x.password.Equals(passwordF) && x.type == ADMIN).ToList();

                if (dataUser.Count() > 0 )
                {
                    Session["FullName"] = dataUser.FirstOrDefault().fullname;
                    Session["UserName"] = dataUser.FirstOrDefault().username;
                    Session["UserId"] = dataUser.FirstOrDefault().id;
                    return RedirectToAction("Index", "Home");

                }else if (dataAdmin.Count() > 0)
                {
                    Session["FullName"] = dataAdmin.FirstOrDefault().fullname;
                    Session["UserName"] = dataAdmin.FirstOrDefault().username;
                    Session["UserId"] = dataAdmin.FirstOrDefault().id;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Error = "Login Fail .Please try again!";
                    return RedirectToAction("Login","Account");
                }
            }
            return View();
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new User();
                user.username = model.UserName;
                user.fullname = model.FullName;
                user.password = GetMD5(model.ConfirmPassword);
                user.status = 0;
                user.type = 1;

                var checkAccountExist = dbEntities.Users.FirstOrDefault(x => x.username == user.username);

                if (checkAccountExist == null)
                {
                    dbEntities.Users.Add(user);
                    dbEntities.SaveChanges();
                    return RedirectToAction("Login", "Account");
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

     
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

      
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
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