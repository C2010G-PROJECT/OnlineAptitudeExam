using OnlineAptitudeExam.Models;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace OnlineAptitudeExam.Controllers
{
    public class TestsController : Controller
    {

        private OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();

        // GET: admin/Tests
       // [Authentication(true)]
        [AdminLayout]
        public ActionResult Index(string sortType, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentType = sortType;
            ViewBag.CurrentOrder = sortOrder;
            if (string.IsNullOrEmpty(sortOrder))
            {
                ViewBag.SortType = sortOrder = "desc";
            }
            else 
            {
                ViewBag.SortType = sortOrder = sortOrder.Equals("desc") ? "asc" : "desc";
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
           
            ViewBag.CurrentFilter = searchString;

            var tests = from s in db.Tests select s;
            if (!string.IsNullOrEmpty(searchString))
            {
                tests = tests.Where(s => s.name.Contains(searchString));
            }

            switch (sortType)
            {
                case "name":
                    tests = sortOrder.Equals("asc") ? tests.OrderBy(s => s.name) : tests.OrderByDescending(s => s.name);
                    break;
                case "date":
                    tests = sortOrder.Equals("asc") ? tests.OrderBy(s => s.created_date) : tests.OrderByDescending(s => s.created_date);
                    break;
                default:  // id ascending 
                    tests = sortOrder.Equals("asc") ? tests.OrderBy(s => s.id) : tests.OrderByDescending(s => s.id);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(tests.ToPagedList(pageNumber, pageSize));
        }
    }
}