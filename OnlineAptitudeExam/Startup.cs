using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineAptitudeExam.Startup))]
namespace OnlineAptitudeExam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
