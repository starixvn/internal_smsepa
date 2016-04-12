using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SMS.EPA.ServiceAPI.ClientSide.Startup))]

namespace SMS.EPA.ServiceAPI.ClientSide
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
