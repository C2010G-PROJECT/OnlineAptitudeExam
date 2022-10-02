using System;
using System.Linq;
using System.Web.Mvc;
using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{
    public class ReportController : Controller
    {

        private readonly OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();
        // GET: Admin/Report
        [Authentication(true)]
        public ActionResult Index(bool isAjax = false)
        {
            ViewBag.isAjax = isAjax;
            return View();
        }

        // GET: Admin/Tests/GetData
        [Authentication(true)]
        [ValidateInput(false)]
        public ActionResult GetDataReport(string nameExam, string dateStart, string dateEnd, int? page)
        {

            /*SELECT Test.name, COUNT(Exam.test_id)[Number of uses]
            FROM Test JOIN Exam on Test.id = Exam.test_id
            GROUP BY Test.name;*/

            var data = from t in db.Tests
                       join e in db.Exams
                       on t.id equals e.test_id
                       select t.name;

            foreach (var v in data)
            {
                Console.WriteLine(v);
            }

            ViewBag.CurrentNameExam = nameExam;
            ViewBag.CurrentDateStart = dateStart;
            ViewBag.CurrentDateEnd = dateEnd;


            return View();
        }
    }
}