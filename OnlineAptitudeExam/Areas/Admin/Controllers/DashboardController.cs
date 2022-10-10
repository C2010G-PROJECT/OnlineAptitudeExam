using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Linq;
using static OnlineAptitudeExam.Models.CustomModel;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();
        // GET: Admin/Dashboard
        [Authentication(true)]
        public ActionResult Index(bool isAjax = false)
        {
            ViewBag.isAjax = isAjax;
            return View();
        }

        [Authentication(true)]
        [ValidateInput(false)]
        public ActionResult GetNewAccountTesting()
        {
            ViewBag.exams = (from s in db.Exams select s).Take(5).ToList();
            return View();
        }
       
    }
}