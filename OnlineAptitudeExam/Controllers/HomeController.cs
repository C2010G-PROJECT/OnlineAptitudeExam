using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using OnlineAptitudeExam.Models;

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

        public ActionResult Profile()
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

        public ActionResult EditProfile()
        {
            Account user = Session["UserInfo"] as Account;


            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                ViewBag.accountLogin = user;
            }

            ViewBag.accountEdit = user;

            return View(user);
        }

        [HttpGet]
        public ActionResult EditProfile(FormCollection key)
        {
            Account modelEdit = Session["UserInfo"] as Account;

            if (modelEdit != null) {
                string sAvatar = key["NameAvatar"];
                string sFullName = key["fullname"];
                string sUseName = key["username"];
                string sAge = key["age"];
                string sAddress = key["address"];
                string sDescriptions = key["descriptions"];

                modelEdit.avatar = sAvatar;
                modelEdit.fullname = sFullName;
                modelEdit.username = sUseName;
                if (sAge != null) {
                    modelEdit.age = Int32.Parse(sAge);
                }
                modelEdit.address = sAddress;
                modelEdit.descriptions = sDescriptions;
                
                UpdateModel(modelEdit);
                dbEntities.SaveChanges();

            }

            return View();
        }
    }
}

