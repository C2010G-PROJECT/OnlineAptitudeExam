using System.Web.Mvc;
using OnlineAptitudeExam.Models;


namespace OnlineAptitudeExam.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [Authentication(true)]
        [AdminLayout]
        public ActionResult Index()
        {

            return View();
        }
    }
}