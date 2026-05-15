using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QLXDK.Startup))]
namespace QLXDK
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
