using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using static OnlineAptitudeExam.Models.Responses;

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
        [ValidateInput(false)]
        public ActionResult GetData(string sort, string order, string filter, int? page)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentOrder = order;
            ViewBag.CurrentFilter = filter;

            var tests = from s in db.Tests select s;
            if (!string.IsNullOrEmpty(filter))
            {
                tests = tests.Where(s => s.name.Contains(filter));
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
        public JsonResult Create(FormModelView.Test model)
        {
            if (ModelState.IsValid)
            {
                string name = model.Name;
                if (db.Tests.Where(t => name.Equals(t.name)).Any())
                {
                    return Json(Error("Name is exists!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
                }
                Test test = new Test();
                test.name = name;
                test.created_date = DateTime.Now.Ticks;
                test.status = (byte?)Enums.Status.PRIVATE;
                db.Tests.Add(test);
                db.SaveChanges();
                return Json(Success(test, "Created test!!!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Error("Something was wrong"));
            }
        }

        [HttpPost, ValidateInput(false)]
        [Authentication(true)]
        public JsonResult Update(int id, FormModelView.Test model)
        {
            if (ModelState.IsValid)
            {
                string name = model.Name;
                var test = db.Tests.Find(id);
                if (test == null)
                {
                    return Json(Error("Not found item!"), JsonRequestBehavior.AllowGet);
                }
                if (test.Exams.Count() != 0)
                {
                    return Json(Error("This test is already taken by users. You cannot change it!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
                }
                if (db.Tests.Where(t => t.id != id && name.Equals(t.name)).Any())
                {
                    return Json(Error("Name is exists!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
                }
                test.name = name;
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Success(name, "Update success!!!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authentication(true)]
        public JsonResult ToggleStatus(int id)
        {
            if (ModelState.IsValid)
            {
                var test = db.Tests.Find(id);
                if (test == null)
                {
                    return Json(Error("Not found item!"), JsonRequestBehavior.AllowGet);
                }
                if (test.Exams.Count() < Constants.TOTAL_QUESTION_IN_TEST)
                {
                    if (test.status != (byte?)Enums.Status.PRIVATE)
                    {
                        test.status = (byte?)Enums.Status.PRIVATE;
                        db.Entry(test).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return Json(Error("This test does not have enough questions.Please create more questions.",MessageType.WARNING, new {status = test.status}), JsonRequestBehavior.AllowGet);
                }
                test.status = Enums.GetOpposite(test.status.Value);
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Success(new { test.status }, "Update success!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authentication(true)]
        public JsonResult Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var test = db.Tests.Find(id);
                if(test == null)
                {
                    return Json(Error("Not found item!"), JsonRequestBehavior.AllowGet);
                }
                if (test.Exams.Count() != 0)
                {
                    return Json(Error("This test is already taken by users. You cannot change it!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
                }
                if (test.Questions.Count() != 0)
                {
                    db.Questions.RemoveRange(test.Questions);
                }
                db.Tests.Remove(test);
                db.SaveChanges();
                return Json(Success(null, "Delete success!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }

        [Authentication(true)]
        public ActionResult Detail(int id = -1, bool isAjax = false)
        {
            var test = db.Tests.Find(id);
            if(test == null)
            {
                return HttpNotFound("404 Page not found");
            }
            ViewBag.IsAjax = isAjax;
            return View(test);
        }
    }


}