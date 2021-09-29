using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mis4200team2.Startup))]
namespace mis4200team2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
