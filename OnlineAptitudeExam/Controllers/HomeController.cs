using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System.Data.Entity;
using System;
using System.Web.Mvc;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json; 

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


        [Authentication]
        [HttpPost]
        public ActionResult EditProfile(FormCollection key, HttpPostedFileBase avatar)
        {
            Account acc = Session["UserInfo"] as Account;
            string avatarName = acc.avatar;
            if (avatar != null && avatar.ContentLength > 0)
            {
                string ext = avatar.FileName.Substring(avatar.FileName.LastIndexOf(".")).ToLower();
                if(ext != ".png" && ext != ".jpg")
                {
                    ViewBag.avatar_err = "File not allow!";
                    return View(acc);
                }
                

                string rootPathFolder = Server.MapPath("/Content/images/avatars/");
                string currAvatarPath = rootPathFolder + acc.avatar;
                if (System.IO.File.Exists(currAvatarPath) && acc.avatar != "user_placeholder.png")
                {
                    System.IO.File.Delete(currAvatarPath);
                }
                // Save file
                avatarName = Helper.GenerateAlphaNumericPwd() + ext;
                string pathImage = rootPathFolder + avatarName;
                avatar.SaveAs(pathImage);
            }

            if (acc != null)
            {
                string sAvatar = avatarName;
                string sFullName = key["fullname"];
                string sAge = key["age"];
                string sAddress = key["address"];
                string sDescriptions = key["descriptions"];

                var account = dbEntities.Accounts.Find(acc.id);

                account.avatar = sAvatar;
                account.fullname = sFullName;
                if (!sAge.IsNullOrWhiteSpace())
                {
                    account.age = int.Parse(sAge);
                }
                else
                {
                    account.age = null;
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

            int testId;

            DateTime endTime;

            var exams = from e in dbEntities.Exams
                        where userId == e.user_id
                        select e;
            Exam exam = exams.FirstOrDefault();


            if (exam != null)
            {

                long timeStart = exam.time_start.Value;
                if (TimeSpan.FromTicks(DateTime.Now.Ticks - timeStart).TotalMinutes > 30)
                {
                    return RedirectToAction("ScoreTable", "Home");// return ra xem keets qua
                }
                else
                {
                    testId = exam.test_id.Value;
                    endTime = new DateTime(timeStart).AddSeconds(1801);
                }
            }
            else
            {
                int randomId = radomTestId();
                if (randomId != 0)
                {
                    testId = randomId;
                    endTime = DateTime.Now.AddSeconds(1801);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            if (exam != null)
            {
                int totalExam = exam.ExamDetails.Count();
                if (totalExam == 0)
                {
                    type = 0;
                }
                else if (totalExam == 5)
                {
                    type = 1;
                }
                else if (totalExam == 10)
                {
                    type = 2;
                }
                else
                {
                    return RedirectToAction("ScoreTable", "Home");
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
                var correctAnswers = JsonConvert.DeserializeObject(q.correct_answers) as JArray;
                totalCorrectAnswers.Add(correctAnswers.Count);
            }

            ViewBag.testId = testId;
            ViewBag.questions = questions;
            ViewBag.totalCorrectAnswers = totalCorrectAnswers;
            ViewBag.type = type;
            ViewBag.timeRemaining = timeRemaining;
            ViewBag.exam = exam;


            return View();
        }

        private int radomTestId()
        {
            Random random = new Random();
            var arrTestId = dbEntities.Tests.Where(t => t.status == 1).Select(t => t.id).ToArray();

            if (arrTestId.Length > 0)
            {
                return arrTestId[random.Next(arrTestId.Length)];
            }

            return -1;
        }

        [Authentication]
        [HttpPost]
        public JsonResult SaveExamResult(FormModelView.Exam examModel)
        {
            if (examModel == null)
            {
                return Json(Responses.Error("Invalid argument !"));
            }
            Account currentAccount = Session["UserInfo"] as Account;

            if (currentAccount == null)
            {
                return Json(Responses.Error("Please login !"));
            }

            var exams = from e in dbEntities.Exams
                        where currentAccount.id == e.user_id
                        select e;

            var exam = exams.FirstOrDefault();

            if (exam == null)
            {
                return Json(Responses.Error("Invalid Exam !"));
            }


            if (examModel.examDetails != null)
            {
                foreach (FormModelView.Exam.ExamDetail ed in examModel.examDetails)
                {
                    var examDetail = new ExamDetail();
                    examDetail.exam_id = exam.id;
                    examDetail.question_id = ed.questionId;
                    examDetail.selected_question = ed.selectedQuestion;

                    exam.ExamDetails.Add(examDetail);
                    dbEntities.SaveChanges();

                }
            }

            exam.time_end = DateTime.Now.Ticks;
            dbEntities.SaveChanges();


            return Json(Responses.Success(null, "Save Success !"));
        }


        [Authentication]
        public ActionResult ScoreTable()
        {

            Account currentAccount = Session["UserInfo"] as Account;

            if (currentAccount == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = currentAccount.id;

            var exams = from e in dbEntities.Exams
                        where e.user_id == userId
                        select e;

            var exam = exams.FirstOrDefault();

            var examDetails = from e in dbEntities.ExamDetails
                              where e.exam_id == exam.id
                              select e;

            if (examDetails == null)
            {
                return RedirectToAction("PrepareTesting", "Home");
            }

            var questions = from e in dbEntities.Questions
                            where e.test_id == exam.test_id
                            select e;

            double score = 0;
            int correct = 0;
            int incorrect = 0;
            double totalScore = 0;


            foreach (ExamDetail examDetail in examDetails)
            {
                foreach (Question question in questions)
                {
                    if (examDetail.question_id == question.id)
                    {
                        totalScore += question.score.Value;
                    }

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

            }

            double percentScore = (correct / totalScore) * 100;

            ViewBag.exam = exam;
            ViewBag.examDetails = examDetails;

            ViewBag.totalScore = totalScore;
            ViewBag.percentScore = percentScore;

            ViewBag.correct = correct;
            ViewBag.incorrect = incorrect;
            return View();

        }
    }
}
