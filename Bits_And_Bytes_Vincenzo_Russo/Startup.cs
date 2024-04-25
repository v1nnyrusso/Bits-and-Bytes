using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bits_And_Bytes_Vincenzo_Russo.Startup))]
namespace Bits_And_Bytes_Vincenzo_Russo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}
