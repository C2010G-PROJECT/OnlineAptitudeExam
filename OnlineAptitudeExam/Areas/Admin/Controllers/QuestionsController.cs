using System.Web.Mvc;

namespace OnlineAptitudeExam.Areas.Admin.Controllers
{
    public class QuestionsController : Controller
    {
        // GET: Admin/Questions
        public ActionResult Index()
        {
            return HttpNotFound("404 Page not found");
        }


    }
}