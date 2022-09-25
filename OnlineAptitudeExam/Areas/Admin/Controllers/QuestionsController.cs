using Newtonsoft.Json;
using OnlineAptitudeExam.Models;
using OnlineAptitudeExam.Utils;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

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

        [HttpPost, ValidateInput(false)]
        [Authentication(true)]
        public JsonResult Create(FormModelView.Question model)
        {

            var test = db.Tests.Find(model.testId);
            if (test == null)
            {
                return Json(Responses.Error("Can't find the test! Try again"));
            }
            if (model.type < 0 || model.type > 2)
            {
                return Json(Responses.Error("The type is invalid!"));
            }
            if (test.Questions.Where(q => q.type == model.type).Count() >= 5)
            {
                return Json(Responses.Error("This category has enough question!", Responses.MessageType.WARNING));
            }
            Question question = new Question();
            question.test_id = model.testId;
            question.question = model.question;
            question.answers = model.answers;
            question.correct_answers = model.correctAnswers;
            question.type = (byte?)model.type;
            question.score = model.score;
            test.Questions.Add(question);
            db.SaveChanges();

            return Json(Responses.Success(null, "Created question!!!"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test()
        {

            dynamic array = JsonConvert.DeserializeObject("[\"2\", \"1\", \"2\", \"3\", \"4\"]");
            foreach (int item in array)
            {
                Debug.WriteLine(item);
            }
            return View();
        }

    }
}