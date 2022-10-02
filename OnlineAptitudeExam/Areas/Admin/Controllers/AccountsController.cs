using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using PagedList;
using static OnlineAptitudeExam.Models.Responses;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{
    public class AccountsController : Controller
    {
        private OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();

        // GET: Admin/Accounts
        [Authentication(true)]
        public ActionResult Index(bool isAjax = false)
        {
            ViewBag.isAjax = isAjax;
            return View();
        }


        [Authentication(true)]
        //[ValidateInput(false)]
        public ActionResult GetData(string sort, string order, string filter, int? page)
        {
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentOrder = order;
            ViewBag.CurrentFilter = filter;

            var accounts = from s in db.Accounts
                           select s;
            accounts = accounts.Where(a => a.type == (int)Enums.Type.USER);
            if (!string.IsNullOrEmpty(filter))
            {
                accounts = accounts.Where(s => s.fullname.Contains(filter));
            }
            if (!string.IsNullOrEmpty(order))
            {
                switch (sort)
                {
                    case SortHelper.NAME:
                        accounts = SortHelper.IsAsc(order) ? accounts.OrderBy(s => s.fullname) : accounts.OrderByDescending(s => s.fullname);
                        break;
                    case SortHelper.DATE:
                        accounts = SortHelper.IsAsc(order) ? accounts.OrderBy(s => s.created_at) : accounts.OrderByDescending(s => s.created_at);
                        break;
                    default:
                        accounts = SortHelper.IsAsc(order) ? accounts.OrderBy(s => s.id) : accounts.OrderByDescending(s => s.id);
                        break;
                }
            }
            else accounts = accounts.OrderByDescending(s => s.id);

            int pageSize = 5;
            int pageNumber = (page == null || page < 1) ? 1 : (int)page;
            return View(accounts.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [Authentication(true)]
        public JsonResult ToggleStatus(int id)
        {
            if (ModelState.IsValid)
            {
                var account = db.Accounts.Find(id);
                account.status = Enums.GetOpposite(account.status.Value);
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return Json(Success(new { account.status }, "Update success!"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Error("Something was wrong"), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateInput(false)]
        [Authentication(true)]
        public JsonResult Create(FormModelView.Account model)
        {
            string fullname = model.fullname;
            string email = model.email;
            string username = model.username;
            string password = model.password;
            if (db.Accounts.Where(t => username.Equals(t.username)).Any())
            {
                return Json(Error("Username is exists!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
            }
            if(db.Accounts.Where(t => email.Equals(t.email)).Any())
            {
                return Json(Error("Email is exists!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
            }
            Account account = new Account();
            account.email = email;
            account.fullname = fullname;
            account.username = username;            
            account.password = Helper.GetMD5(password);
            account.type = (byte?)Enums.Type.USER;
            account.status = (byte?)Enums.Status.UNLOCK;
            account.created_at = DateTime.Now.Ticks;

            db.Accounts.Add(account);
            db.SaveChanges();
            return Json(Success(account, "Created Account!!!"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        [Authentication(true)]
        public JsonResult Update(int id, FormModelView.Account model)
        {
            //string fullname = model.fullname;
            //string username = model.username;
            string password = model.password;

            var account = db.Accounts.Find(id);
            if (password == null)
            {
                return Json(Error("Can not be blank!"), JsonRequestBehavior.AllowGet);
            }
            //if (db.Accounts.Where(t => t.username.Equals(username)).Any())
            //{
            //    return Json(Error("Username is exists!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
            //}
            if (db.Accounts.Where(t => t.password.Equals(password)).Any())
            {
                return Json(Error("Password not duplicate old password!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
            }

            //account.fullname = fullname;
            //account.username = username;
            account.password = Helper.GetMD5(password);
            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();
            return Json(Success(password, "Update success!!!"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authentication(true)]
        public JsonResult Delete(int id)
        {
            var account = db.Accounts.Find(id);
            if (account == null)
            {
                return Json(Error("Not found item!"), JsonRequestBehavior.AllowGet);
            }
            if (account.Exams.Count() != 0)
            {
                return Json(Error("This account is already tested. You cannot remove it!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
            }
            db.Accounts.Remove(account);
            db.SaveChanges();
            return Json(Success(null, "Delete success!"), JsonRequestBehavior.AllowGet);
        }

        [Authentication(true)]
        public ActionResult Detail(int id = -1, bool isAjax = false)
        {
            var account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound("404 Page not found");
            }
            ViewBag.IsAjax = isAjax;
            return View(account);
        }

        // GET: Admin/Accounts/Create
        public ActionResult Create()
        {
            return View();
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
