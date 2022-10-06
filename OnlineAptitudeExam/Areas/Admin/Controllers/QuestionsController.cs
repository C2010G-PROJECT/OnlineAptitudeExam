using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using static OnlineAptitudeExam.Models.Responses;
using Question = OnlineAptitudeExam.Models.Question;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly OnlineAptitudeExamEntities db = new OnlineAptitudeExamEntities();

        // GET: Admin/Questions
        [Authentication(true)]
        public ActionResult Index()
        {
            return HttpNotFound("404 Page not found");
        }

        // POST: Admin/GetData
        [HttpPost]
        [Authentication(true)]
        public ActionResult GetData(int testId, int type)
        {
            var test = db.Tests.Find(testId);
            if (test == null)
            {
                return HttpNotFound("404 Page not found");
            }
            ViewBag.testId = testId;
            ViewBag.type = type;
            ViewBag.questions = test.Questions.Where(q => q.type == type).ToList();
            return View();
        }

        // POST: Admin/GetQuestion
        [HttpPost]
        [Authentication(true)]
        public JsonResult GetQuestion(int id)
        {
            var q = db.Questions.Find(id);
            if (q == null) return Json(null);
            else return Json(new
            {
                q.id,
                q.test_id,
                q.question,
                q.answers,
                q.correct_answers,
                q.type,
                q.score
            });
        }

        [HttpPost, ValidateInput(false)]
        [Authentication(true)]
        public JsonResult Create(FormModelView.Question model)
        {

            var test = db.Tests.Find(model.testId);
            if (test == null)
            {
                return Json(Error("Can't find the question! Try again"));
            }
            JsonResult check = CheckTestValid(test);
            if (check != null)
            {
                return check;
            }
            if (model.type < 0 || model.type > 2)
            {
                return Json(Error("The type is invalid!"));
            }
            if (test.Questions.Where(q => q.type == model.type).Count() >= 5)
            {
                return Json(Error("This category has enough question!", MessageType.WARNING));
            }
            Question question = new Question
            {
                test_id = model.testId,
                question = model.question,
                answers = model.answers,
                correct_answers = model.correctAnswers,
                type = (byte?)model.type,
                score = model.score
            };
            test.Questions.Add(question);
            db.SaveChanges();

            return Json(Success(null, "Created question!!!"), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        [Authentication(true)]
        public JsonResult Update(int id, FormModelView.Question model)
        {
            var question = db.Questions.Find(id);
            if (question == null)
            {
                return Json(Error("Not found item!"), JsonRequestBehavior.AllowGet);
            }
            JsonResult check = CheckTestValid(question.Test);
            if (check != null)
            {
                return check;
            }
            //question.type = (byte?)model.type;
            question.question = model.question;
            question.answers = model.answers;
            question.correct_answers = model.correctAnswers;
            question.score = model.score;
            db.Entry(question).State = EntityState.Modified;
            db.SaveChanges();
            return Json(Success(new
            {
                question.id,
                question.test_id,
                question.question,
                question.answers,
                question.correct_answers,
                question.type,
                question.score
            }, "Update success!"), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authentication(true)]
        public JsonResult Delete(int id)
        {
            var question = db.Questions.Find(id);
            if (question == null)
            {
                return Json(Error("Not found item!"), JsonRequestBehavior.AllowGet);
            }
            JsonResult check = CheckTestValid(question.Test);
            if (check != null)
            {
                return check;
            }
            db.Questions.Remove(question);
            db.SaveChanges();
            return Json(Success(null, "Delete success!"), JsonRequestBehavior.AllowGet);
        }

        private JsonResult CheckTestValid(Test test)
        {
            if (test.Exams.Count() != 0)
            {
                return Json(Error("This test is already taken by users. You cannot change it!", MessageType.WARNING), JsonRequestBehavior.AllowGet);
            }
            return null;
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