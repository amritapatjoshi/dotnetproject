using Microsoft.Owin.Cors;
using Owin;

namespace UposDeviceSimulationConsole
{
    class SignalRStartup
    {
        public void Configuration(IAppBuilder MyApp)
        {
            MyApp.UseCors(CorsOptions.AllowAll);
            MyApp.MapSignalR();
        }

    }
}
