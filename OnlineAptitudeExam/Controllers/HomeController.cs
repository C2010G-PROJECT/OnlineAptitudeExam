using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System.Data.Entity;
using System;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

namespace OnlineAptitudeExam.Controllers
{
    public class HomeController : Controller
    {
        private OnlineAptitudeExamEntities dbEntities = new OnlineAptitudeExamEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authentication]
        public ActionResult Profiles()
        {
            ViewBag.Message = "Your contact page.";
            Account user = Session["UserInfo"] as Account;
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                ViewBag.accountLogin = user;
                if (user.avatar == null)
                {
                    user.avatar = "user_placeholder.png";
                }
            }

            ViewBag.accountLogin = user;

            return View();
        }

        [Authentication]
        public ActionResult EditProfile()
        {
            Account user = Session["UserInfo"] as Account;
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                ViewBag.accountEdit = user;
            }
            return View(user);
        }

        string avatarCurrentFile;

        [Authentication]
        [HttpPost]
        public ActionResult EditProfile(FormCollection key, HttpPostedFileBase avatar)
        {
            Account currentAccountSession = Session["UserInfo"] as Account;

            if (avatar.ContentLength > 0)
            {

                // Save file
                string rootPathFolder = Server.MapPath("/Content/images/avatars/");
                string pathImage = rootPathFolder + avatar.FileName;
                avatar.SaveAs(pathImage);
                avatarCurrentFile = rootPathFolder + currentAccountSession.avatar;
                if (System.IO.File.Exists(avatarCurrentFile) && avatar.FileName != currentAccountSession.avatar && currentAccountSession.avatar != "user_placeholder.png")
                {
                    System.IO.File.Delete(avatarCurrentFile);
                }
            }

            if (currentAccountSession != null)
            {
                string sAvatar = avatar.FileName;
                string sFullName = key["fullname"];
                string sUseName = key["username"];
                string sAge = key["age"];
                string sAddress = key["address"];
                string sDescriptions = key["descriptions"];

                var account = dbEntities.Accounts.Find(currentAccountSession.id);

                account.avatar = sAvatar;
                account.fullname = sFullName;
                account.username = sUseName;
                if (sAge != null)
                {
                    account.age = Int32.Parse(sAge);
                }
                account.address = sAddress;
                account.descriptions = sDescriptions;

                dbEntities.Entry(account).State = EntityState.Modified;
                dbEntities.SaveChanges();
                Session["UserInfo"] = account;
            }

            return RedirectToAction("Profiles", "Home");
        }

        [Authentication]
        public ActionResult PrepareTesting()
        {

            return View();
        }

        [Authentication]
        public ActionResult QuestionExam(int type = 0)
        {


            Account currentAccount = Session["UserInfo"] as Account;


            int userId = currentAccount.id;

            int testId ;

            Exam exam = null;
            DateTime endTime;

            if (currentAccount.Exams.Count() == 1)
            {
                exam = currentAccount.Exams.ToArray()[0];
                long timeStart = exam.time_start.Value;

                if (TimeSpan.FromTicks(DateTime.Now.Ticks - timeStart).TotalMinutes > 30)
                {
                    return RedirectToAction("Index", "Home");// return ra xem keets qua
                }
                else
                {
                    testId = exam.test_id.Value;
                    endTime = new DateTime(timeStart).AddMinutes(30);
                }
            }
            else
            {
                testId = 7;
                endTime = DateTime.Now.AddMinutes(30);

            }

            if (exam != null)
            {
                int totalExam = exam.ExamDetails.Count();
                if (totalExam < 5)
                {
                    type = 0;
                }
                else if (totalExam < 10)
                {
                    type = 1;
                }
                else if (totalExam < 15)
                {
                    type = 2;
                }
                else
                {
                    return HttpNotFound("404 Page Not Found");// return ra xem keets qua
                }
            }
            else
            {
                exam = new Exam();
                exam.user_id = userId;
                exam.test_id = testId;
                exam.time_start = DateTime.Now.Ticks;
                dbEntities.Exams.Add(exam);
                dbEntities.SaveChanges();
            }

            var timeRemaining = TimeSpan.FromTicks(endTime.Ticks - DateTime.Now.Ticks).TotalSeconds.ToString("#");



            var tests = from s in dbEntities.Tests
                        where s.id == testId
                        select s;
            Test test = tests.FirstOrDefault();

            if (test == null)
            {
                return HttpNotFound("404 Page Not Found");
            }


            List<Question> questions = test.Questions.Where(q => q.type == type).ToList();

            List<int> totalCorrectAnswers = new List<int>();

            foreach (Question q in questions)
            {
                int total = q.correct_answers.Trim('[', ']').Split(new[] { ',' }).Select(x => x.Trim('"')).ToArray().Count();
                totalCorrectAnswers.Add(total);
            }

            ViewBag.testId = testId;
            ViewBag.questions = questions;
            ViewBag.totalCorrectAnswers = totalCorrectAnswers;
            ViewBag.type = type;
            ViewBag.timeRemaining = timeRemaining;
            ViewBag.exam = exam;


            return View();
        }

        //[HttpPost]
        //public JsonResult QuestionExam()
        //{

        //}
    }
}
