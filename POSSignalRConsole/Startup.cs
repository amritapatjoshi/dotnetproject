using Microsoft.Owin.Cors;
using Owin;

namespace POSSignalRConsole
{
    class Startup
    {
        public void Configuration(IAppBuilder MyApp)
        {
            MyApp.UseCors(CorsOptions.AllowAll);
            MyApp.MapSignalR();
        }
    }
}
