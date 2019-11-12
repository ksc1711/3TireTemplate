using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WCMS.Web.Startup))]
namespace WCMS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
