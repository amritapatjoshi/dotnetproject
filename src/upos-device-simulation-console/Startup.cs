using Microsoft.Extensions.DependencyInjection;
using System;
using upos_device_simulation.Services;
using upos_device_simulation.Helpers;
using upos_device_simulation.Interfaces;
using OposScanner_CCO;

namespace UposDeviceSimulationConsole
{
    public static class Startup
    {
        public static IServiceProvider _serviceProvider;
        public static void ConfigureServices(this IServiceCollection services)
        {
           
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IBarcodeScanner, BarcodeScanner>();
            services.AddTransient<IPayMSR, PayMSR>();
            services.AddTransient<IPaypinpad, Paypinpad>();
            services.AddTransient<IReceiptPrinter, ReceiptPrinter>();
            services.AddTransient<OPOSScanner, OPOSScannerClass>();
            services.AddSingleton<PosHub, PosHub>();
            services.AddSingleton<PosExecutor, PosExecutor>();
            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
