using Microsoft.AspNet.SignalR;
using System;
using System.Threading;
using upos_device_simulation;

namespace UposDeviceSimulationConsole
{
   
    public class PosHub : Hub
    {
        public void StartPayment(string simulatorType)
        {
            Console.WriteLine("Payment started");
            Console.WriteLine(simulatorType);
            Thread newThread = new Thread(new ParameterizedThreadStart(Program.InvokeSimulator));
            newThread.Start("msr");
            
        }
        public void StartPrinter(string simulatorType,string printData)
        {
            Console.WriteLine("printing started");
            ReceiptPrinter receiptPrinter = new ReceiptPrinter();
            Thread newThread = new Thread(new ParameterizedThreadStart(receiptPrinter.Start));
            newThread.Start(printData);
        }
    }
}
