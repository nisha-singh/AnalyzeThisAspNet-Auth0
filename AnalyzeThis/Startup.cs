using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AnalyzeThis.Startup))]
namespace AnalyzeThis
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
