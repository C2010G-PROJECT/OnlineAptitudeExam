using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using static OnlineAptitudeExam.Models.Responses;
using static OnlineAptitudeExam.Models.FormModelView;
using Account = OnlineAptitudeExam.Models.FormModelView.Account;
using Microsoft.Ajax.Utilities;
using System.Security.Cryptography;

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
            //string sort = "";
            var exams = from s in db.Exams select s;
            if (exams.Count() == 0)
            {
                return View(new HomeViewModel());
            }
            exams = exams.OrderByDescending(t => t.time_end).Take(4);
            //sort = SortHelper.DATE;
            //exams = exams.OrderByDescending(s => s.time_end);

            var selectExams = (from p in exams
                               join c in db.Accounts on p.user_id equals c.id
                               select p).ToList();

            var selectAccounts = (from p in selectExams
                                  join c in db.Accounts on p.user_id equals c.id
                                  select c).ToList();
            var homeViewModel = new HomeViewModel();
            homeViewModel.AccountsTested = selectAccounts;
            homeViewModel.ExamsTested = selectExams;
            return View(homeViewModel);
        }
       
    }
}