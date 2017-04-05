using Microsoft.Owin;

using Owin;

[assembly: OwinStartupAttribute(typeof(SurveySystem.Web.Startup))]

namespace SurveySystem.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}
