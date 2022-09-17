using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OnlineAptitudeExam.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            return RedirectToAction("Login", "Home");
        }

        //
        // GET: /Auth/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session["UserInfo"] is User)
            {
                User user = Session["UserInfo"] as User;
                if (user.type.Equals(Models.User.USER))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Auth/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string password = GetMD5(model.Password);
                var user = dbEntities.Users.Where(x => 
                x.username.Equals(model.UserName) &&
                x.password.Equals(password)).First();
 
                if (user != null)
                {
                    Session["UserInfo"] = user;
                    bool isAdmin = user.type == Models.User.ADMIN;
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
                    ViewBag.Error = "Login Fail .Please try again!";
                    return RedirectToAction("Login", "Auth");
                }
            }
            return View();
        }
         
        //
        // GET: /Auth/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Session["UserInfo"] is User)
            {
                User user = Session["UserInfo"] as User;
                if (user.type.Equals(Models.User.USER))
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

                var user = new User();
                user.username = model.UserName;
                user.fullname = model.FullName;
                user.password = GetMD5(model.ConfirmPassword);
                user.status = 0;
                user.type = 1;

                var checkAuthExist = dbEntities.Users.FirstOrDefault(x => x.username == user.username);

                if (checkAuthExist == null)
                {
                    dbEntities.Users.Add(user);
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
            return RedirectToAction("Index", "Auth");
        }

        //
        // GET: /Auth/ExternalLoginFailure
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