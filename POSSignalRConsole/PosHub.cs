using Microsoft.AspNet.SignalR;
using System;
using System.Threading;

namespace POSSignalRConsole
{
   
    public class PosHub : Hub
    {
        public void StartPayment(string simulatorType)
        {
            Console.WriteLine("Pinpad started");
            Console.WriteLine(simulatorType);
            Thread newThread = new Thread(new ParameterizedThreadStart(Program.InvokeSimulator));
            newThread.Start(simulatorType);
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
