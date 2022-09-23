using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{
    public class TestsController : Controller
    {
        private readonly OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();

        // GET: Admin/Tests
        [Authentication(true)]
        public ActionResult Index(bool isAjax = false)
        {
            ViewBag.isAjax = isAjax;
            return View();
        }

        // GET: Admin/Tests/GetData
        [Authentication(true)]
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
                        tests = SortHelper.IsAsc(order) ? tests.OrderBy(s => s.name) : tests.OrderByDescending(s => s.name);
                        break;
                    case SortHelper.DATE:
                        tests = SortHelper.IsAsc(order) ? tests.OrderBy(s => s.created_date) : tests.OrderByDescending(s => s.created_date);
                        break;
                    default:
                        tests = SortHelper.IsAsc(order) ? tests.OrderBy(s => s.id) : tests.OrderByDescending(s => s.id);
                        break;
                }
            }
            else tests = tests.OrderByDescending(s => s.id);

            int pageSize = 5;
            int pageNumber = (page == null || page < 1) ? 1 : (int)page;
            return View(tests.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost, ValidateInput(false)]
        [Authentication(true)]
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
        [Authentication(true)]
        public ActionResult Update(int id, string name)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return Json(Responses.Error("Name must be not null!"), JsonRequestBehavior.AllowGet);
                }
                var test = db.Tests.Find(id);
                if (test == null)
                {
                    return Json(Responses.Error("Not found item!"), JsonRequestBehavior.AllowGet);
                }
                if (test.Exams.Count() != 0)
                {
                    return Json(Responses.Error("This test is already taken by users. You cannot change it!"), JsonRequestBehavior.AllowGet);
                }
                if (db.Tests.Where(t => t.id != id && name.Equals(t.name)).Any())
                {
                    return Json(Responses.Error("Name is exists!"), JsonRequestBehavior.AllowGet);
                }
                test.name = name;
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Responses.Success(name, "Update success!!!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Responses.Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authentication(true)]
        public ActionResult ToggleStatus(int id)
        {
            if (ModelState.IsValid)
            {
                var test = db.Tests.Find(id);
                if (test == null)
                {
                    return Json(Responses.Error("Not found item!"), JsonRequestBehavior.AllowGet);
                }
                if (test.Exams.Count() < Constants.TOTAL_QUESTION_IN_TEST)
                {
                    if (test.status != (byte?)Enums.Status.PRIVATE)
                    {
                        test.status = (byte?)Enums.Status.PRIVATE;
                        db.Entry(test).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Json(Responses.Error("This test does not have enough questions.Please create more questions.", new {status = test.status}), JsonRequestBehavior.AllowGet);
                }
                test.status = Enums.GetOpposite(test.status.Value);
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Responses.Success(new { status = test.status }, "Update success!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Responses.Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authentication(true)]
        public ActionResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var test = db.Tests.Find(id);
                if(test == null)
                {
                    return Json(Responses.Error("Not found item!"), JsonRequestBehavior.AllowGet);
                }
                if (test.Exams.Count() != 0)
                {
                    return Json(Responses.Error("This test is already taken by users. You cannot change it!"), JsonRequestBehavior.AllowGet);
                }
                if (test.Questions.Count() != 0)
                {
                    db.Questions.RemoveRange(test.Questions);
                }
                db.Tests.Remove(test);
                db.SaveChanges();
                return Json(Responses.Success(null, "Delete success!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Responses.Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }

        [Authentication(true)]
        public ActionResult Detail(int id, bool isAjax = false)
        {
            var test = db.Tests.Find(id);
            ViewBag.IsAjax = isAjax;
            return View(test);
        }
    }


}