using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{

    public class AuthController : Controller
    {
        private OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();

        private const string COOKIE_NAME = "auth_cookie";
        // GET: Admin/Auth
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: Admin/Auth/Login
        public ActionResult Login()
        {
            HttpCookie authCookie = Request.Cookies.Get(COOKIE_NAME);

            if (authCookie != null)
            {
                long exp = Convert.ToInt64(authCookie.Values["exp"]);
                if (exp > DateTime.Now.Ticks)
                {
                    Debug.WriteLine(authCookie.Values["username"]);
                    ViewBag.username = authCookie.Values["username"];
                    ViewBag.password = authCookie.Values["password"];
                    ViewBag.checkbox = authCookie.Values["checkbox"];
                }
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string password = Helper.GetMD5(model.Password);
                var account = db.Accounts.Where(x =>
                    x.username.Equals(model.UserName) &&
                    x.password.Equals(password) &&
                    x.type == (int)Enums.Type.ADMIN
                ).FirstOrDefault();

                if (account != null)
                {
                    Session["UserInfo"] = account;
                    Debug.WriteLine("" + model.RememberMe);
                    if (model.RememberMe)
                    {
                        HttpCookie cookie = new HttpCookie(COOKIE_NAME);
                        cookie["username"] = model.UserName;
                        cookie["password"] = model.Password;
                        cookie["checkbox"] = "checked";
                        cookie["exp"] = DateTime.Now.AddDays(7).Ticks.ToString();
                        Response.Cookies.Set(cookie);
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie(COOKIE_NAME);
                        cookie["exp"] = "-1";
                        Response.Cookies.Set(cookie);
                    }
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.Error = "Username or password is incorrect!";
                }
            }
            return View();
        }

        // POST: Admin/Auth/LogOut
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

    }
}