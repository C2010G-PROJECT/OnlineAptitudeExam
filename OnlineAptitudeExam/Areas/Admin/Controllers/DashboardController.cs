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
                                  select c ).ToList();
            var homeViewModel = new HomeViewModel();
            homeViewModel.AccountsTested = selectAccounts;
            homeViewModel.ExamsTested = selectExams;
            return View(homeViewModel);
        }

        public ActionResult Indexxxx()
        {
            
            int down5 = 2;
            int up5 = 1;
            int up10 = 3;
            int up15 = 10;
            int up20 = 3;

            int pass = 0;
            int fail = 0;

            var accountsTested = (from acc in db.Accounts
                                  join exm in db.Exams on acc.id equals exm.user_id
                                  join exmdl in db.ExamDetails on exm.id equals exmdl.exam_id
                                  select new
                                  {
                                      exm.user_id,
                                      exmdl.id,          // exam (- test ?                            vd: exam (- test id = 1
                                      exm.test_id,            // exam tested ? to take correct answer      vd: exam => examDetail id =1 => take list answer

                                  }).ToList();

            var examsTested = (from exm in db.Exams
                               join tes in db.Tests on exm.test_id equals tes.id
                               join ques in db.Questions on tes.id equals ques.test_id
                               select new
                               {
                                   ques.id,         // exam tested?           vd: tes.id 
                                   ques.test_id,  // question (- test ?        vd: have list answer correct => question correct => score => total score
                               }).ToList();
            var accountsTested_examsTested = (from aT in accountsTested
                                              join eT in examsTested on aT.test_id equals eT.test_id
                                              select new
                                              {
                                                  aT.user_id,  //acc.id
                                                  eT.id,    // ques.id

                                                  
                                              }).ToList();
  

            //if (accountsTested == null)
            //{
            //    return RedirectToAction("PrepareTesting", "Home");
            //}

           // var questions = from e in db.Questions
           //                 where e.test_id == exam.test_id
            //                select e;

            double score = 0;
            int correct = 0;
            int incorrect = 0;
            foreach (var examDetail in db.ExamDetails)
            {
                foreach (var question in db.Questions)
                {
                    if (examDetail.selected_question.Equals(question.correct_answers) && examDetail.question_id == question.id)
                    {
                        correct++;
                        score += question.score.Value;
                        ViewBag.score = score;
                    }
                    else if (!examDetail.selected_question.Equals(question.correct_answers) && examDetail.question_id == question.id)
                    {
                        incorrect++;
                    }
                }
                if( score >= 20){
                    up20 += 1;
                } else if( score >= 15 ) {
                    up15 += 1;
                } else if ( score >= 10)
                {
                    up10 += 1;
                } else if (score >= 5)
                {
                    up5 += 1;
                } else
                {
                    down5 +=1;
                }

            }
            int[] distance = new int[] { down5, up5, up10, up15, up20 };
            ViewBag.d = distance;
            return View();
        }

    }
}