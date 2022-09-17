using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace OnlineAptitudeExam.Models
{
    public class AdminLayout: ActionFilterAttribute
    {
        private static bool pendingAdmin;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            pendingAdmin = true;
        }

        public static bool IsPending()
        {
            Debug.WriteLine("=========== > "+pendingAdmin);
            bool _isAdmin = pendingAdmin;
            pendingAdmin = false;
            return _isAdmin;
        }
    }
 
}