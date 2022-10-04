using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using PagedList;
using static OnlineAptitudeExam.Models.CustomModel;

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
        public ActionResult GetData(string sort, string order, string filter, long? d_start, long? d_end, int? page)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentOrder = order;
            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentDateStart = d_start;
            ViewBag.CurrentDateEnd = d_end;

            /*SELECT Test.id, Test.name, COUNT(Test.id)[total_user]
            FROM Test JOIN Exam on Test.id = Exam.test_id
            GROUP BY Test.id, Test.name;*/

            long start = 0, end = long.MaxValue;
            if (d_start.HasValue)
                start = DateUtils.FromUnixTime(d_start.Value).Ticks;
            if (d_end.HasValue)
                end = DateUtils.FromUnixTime(d_end.Value).Ticks;

            var testsReport = (from t in db.Tests
                               join e in db.Exams on t.id equals e.test_id
                               where e.time_start > start && e.time_start < end
                               group e by e.test_id into g
                               select g)
                        .ToList()
                        .Select(val => new TestReport(val.First().test_id, val.First().Test.name, val.ToList()));
            if (!string.IsNullOrEmpty(filter))
            {
                testsReport = testsReport.Where(s => s.Name.Contains(filter));
            }
            if (!string.IsNullOrEmpty(order))
            {
                switch (sort)
                {
                    case SortHelper.NAME:
                        testsReport = SortHelper.IsAsc(order) ? testsReport.OrderBy(s => s.Name) : testsReport.OrderByDescending(s => s.Name);
                        break;
                    default:
                        testsReport = SortHelper.IsAsc(order) ? testsReport.OrderBy(s => s.Id) : testsReport.OrderByDescending(s => s.Id);
                        break;
                }
            }
            else testsReport = testsReport.OrderByDescending(s => s.Id);

            int pageSize = 5;
            int pageNumber = (page == null || page < 1) ? 1 : (int)page;
            return View(testsReport.ToPagedList(pageNumber, pageSize));
        }

        [Authentication(true)]
        public ActionResult Detail(int id = -1, bool isAjax = false)
        {
            var exam = db.Exams.Find(id);
            if (exam == null)
            {
                return HttpNotFound("404 Page not found");
            }
            ViewBag.IsAjax = isAjax;
            return View(exam);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

   
}