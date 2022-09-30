using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System.Data.Entity;
using System;
using System.Web.Mvc;
using System.Web;

namespace OnlineAptitudeExam.Controllers
{
    public class HomeController : Controller
    {
        private OnlineAptitudeExamEntities dbEntities = new OnlineAptitudeExamEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authentication]
        public ActionResult Profiles()
        {
            ViewBag.Message = "Your contact page.";
            Account user = Session["UserInfo"] as Account;
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                ViewBag.accountLogin = user;
            }

            ViewBag.accountLogin = user;

            return View();
        }

        [Authentication]
        public ActionResult EditProfile()
        {
            Account user = Session["UserInfo"] as Account;
            ViewBag.accountEdit = user;
            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(FormCollection key)
        {
            Account currentAccountSession = Session["UserInfo"] as Account;

            if (currentAccountSession != null)
            {
                string sAvatar = key["NameAvatar"];
                string sFullName = key["fullname"];
                string sUseName = key["username"];
                string sAge = key["age"];
                string sAddress = key["address"];
                string sDescriptions = key["descriptions"];

                var account = dbEntities.Accounts.Find(currentAccountSession.id);

                account.avatar = sAvatar;
                account.fullname = sFullName;
                account.username = sUseName;
                if (sAge != null)
                {
                    account.age = Int32.Parse(sAge);
                }
                account.address = sAddress;
                account.descriptions = sDescriptions;

                dbEntities.Entry(account).State = EntityState.Modified;
                dbEntities.SaveChanges();
                Session["UserInfo"] = account;
            }

            return RedirectToAction("Profiles");
        }

        public ActionResult PrepareTesting()
        {

            return View();
        }

        public ActionResult QuestionExam()
        {

            return View();
        }

    }
}