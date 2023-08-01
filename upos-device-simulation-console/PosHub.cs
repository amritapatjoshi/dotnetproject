using Microsoft.AspNet.SignalR;
using System;
using System.Threading;
using upos_device_simulation.Interfaces;

namespace UposDeviceSimulationConsole
{

    public class PosHub : Hub
    {
        ILogger logger;
        Executor executor;
        IReceiptPrinter receiptPrinter;
        public PosHub ()
        {
            this.logger = (ILogger)Startup._serviceProvider.GetService(typeof(ILogger));
            this.executor = (Executor)Startup._serviceProvider.GetService(typeof(Executor));
            this.receiptPrinter = (IReceiptPrinter)Startup._serviceProvider.GetService(typeof(IReceiptPrinter)); 
        }
       public PosHub(Executor executor, ILogger logger,IReceiptPrinter receiptPrinter )
        {
            this.logger = logger;
            this.executor = executor;
            this.receiptPrinter = receiptPrinter;
        }
        public void StartPayment(string simulatorType)
        {
            logger.Info("Payment started");
            Thread newThread = new Thread(new ParameterizedThreadStart(executor.InvokeSimulator));
            newThread.Start("msr");
            
        }
        public void StartPrinter(string simulatorType,string printData)
        {
            logger.Info("printing started");
            Thread newThread = new Thread(new ParameterizedThreadStart(receiptPrinter.Start));
            newThread.Start(printData);
        }
    }
}
