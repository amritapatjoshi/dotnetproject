using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.Configuration;

namespace POSSignalRConsole
{

    class Program
    {
        static IHubContext context = GlobalHost.ConnectionManager.GetHubContext<PosHub>();
        private static readonly HttpClient client = new HttpClient();
        public static string defaultSimulatorType="scan";
        static void Main(string[] args)
        {

            string url = ConfigurationManager.AppSettings["SignalrUrl"]; ;
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("The Server URL is: {0}", url);
                InvokeSimulator(defaultSimulatorType);
                Console.ReadLine();
            }
        }
      
       public static void InvokeSimulator(Object obj)
        {
            string data = (string)obj;
            if (data == "scan")
            {
               // BarcodeScanner barcodeScanner = new BarcodeScanner();
                BarcodeScanner.Start();
                BarcodeScanner.Scanned += BarcodeScanner_Scanned;
            }
            else if (data == "print")
            {
                ReceiptPrinter receiptPrinter = new ReceiptPrinter();
                receiptPrinter.Start("Print data will go here");

            }
            else if (data == "pinpad")
            {
                PayPinpad payPinpad = new PayPinpad();
                payPinpad.Start();
                payPinpad.PinEntered += PayPinpad_PinEntered;
            }
            else
            {
                Console.WriteLine("Nothing Invoked");
            }


        }

        private async static void BarcodeScanner_Scanned(object sender, ScannedEventArgs e)
        {
           await context.Clients.All.sendScannedData(e.BarcodeId, e.DeviceId);
        }

        private async static void PayPinpad_PinEntered(object sender, PinEnteredEventArgs e)
        {
            await context.Clients.All.paymentComplete(e.DeviceId, e.AccountNumber,e.Ammount,e.PaymentStatus);
        }
    }

    

    
}
