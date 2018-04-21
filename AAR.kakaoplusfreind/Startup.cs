using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AAR.kakaoplusfreind.Startup))]
namespace AAR.kakaoplusfreind
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
