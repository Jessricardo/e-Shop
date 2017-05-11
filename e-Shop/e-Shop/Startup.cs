using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(e_Shop.Startup))]
namespace e_Shop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
