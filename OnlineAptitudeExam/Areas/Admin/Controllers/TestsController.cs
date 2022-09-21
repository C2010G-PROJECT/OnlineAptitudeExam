using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using PagedList;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{
    public class TestsController : Controller
    {
        private readonly OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();

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

            int pageSize = 5;
            int pageNumber = ((page == null || page < 1) ? 1 : (int)page);
            return View(tests.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(string name)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Json(Responses.Error("Name must be not null!"), JsonRequestBehavior.AllowGet);
                }
                if (db.Tests.Where(t => name.Equals(t.name)).Any())
                {
                    return Json(Responses.Error("Name is exists!"), JsonRequestBehavior.AllowGet);
                }
                Test test = new Test();
                test.name = name;
                test.created_date = DateTime.Now.Ticks;
                test.status = (byte?)Enums.Status.PRIVATE;
                db.Tests.Add(test);
                db.SaveChanges();
                return Json(Responses.Success(test, "Created test!!!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Responses.Error("Something was wrong"));
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Update(int id, string name)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Json(Responses.Error("Name must be not null!"), JsonRequestBehavior.AllowGet);
                }
                if (db.Tests.Where(t => t.id != id && name.Equals(t.name)).Any())
                {
                    return Json(Responses.Error("Name is exists!"), JsonRequestBehavior.AllowGet);
                }
                var test = db.Tests.Find(id);
                test.name = name;
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Responses.Success(test, "Update success!!!"), JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(Responses.Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ToggleStatus(int id)
        {
            if (ModelState.IsValid)
            {
                var test = db.Tests.Find(id);
                test.status = Enums.GetOpposite(test.status.Value);
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Responses.Success(test, "Update success!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Responses.Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }
    }
}