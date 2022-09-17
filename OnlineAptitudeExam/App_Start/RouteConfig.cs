using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineAptitudeExam
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // ---------------- general ---------------


            // ---------------- client ---------------- 



            // ----------------  admin  --------------- 


            routes.MapRoute(
                 name: "admin_tests",
                 url: "admin/tests/{action}/{id}",
                 defaults: new { controller = "tests", action = "Index", id = UrlParameter.Optional }
            );

            // ---------------- default ---------------

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
