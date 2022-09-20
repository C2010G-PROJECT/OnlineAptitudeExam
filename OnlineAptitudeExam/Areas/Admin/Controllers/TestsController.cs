using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{
    public class TestsController : Controller
    {
        private OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();

        // GET: Admin/Tests
        // [Authentication(true)]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Tests/GetData
        public ActionResult GetData(string sort, string order, string filter, string search, int? page)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentOrder = order;
            ViewBag.CurrentFilter = search = search ?? filter;

            var tests = from s in db.Tests select s;
            if (!string.IsNullOrEmpty(search))
            {
                tests = tests.Where(s => s.name.Contains(search));
            }

            if (!string.IsNullOrEmpty(order))
            {
                switch (sort)
                {
                    case SortHelper.NAME:
                        tests = SortHelper.isAsc(order) ? tests.OrderBy(s => s.name) : tests.OrderByDescending(s => s.name);
                        break;
                    case SortHelper.DATE:
                        tests = SortHelper.isAsc(order) ? tests.OrderBy(s => s.created_date) : tests.OrderByDescending(s => s.created_date);
                        break;
                    default:
                        tests = SortHelper.isAsc(order) ? tests.OrderBy(s => s.id) : tests.OrderByDescending(s => s.id);
                        break;
                }
            }
            else tests = tests.OrderByDescending(s => s.id);

            int pageSize = 3;
            int pageNumber = ((page == null || page < 1) ? 1 : (int)page);
            return View(tests.ToPagedList(pageNumber, pageSize));
        }
    }
}