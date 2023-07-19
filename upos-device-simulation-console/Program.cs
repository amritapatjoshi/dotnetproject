using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.Configuration;
using upos_device_simulation;

namespace UposDeviceSimulationConsole
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
                BarcodeScanner barcodeScanner = BarcodeScanner.Instance;
                barcodeScanner.Start();
                barcodeScanner.Scanned += BarcodeScanner_Scanned;
            }
            else if (data == "print")
            {
                ReceiptPrinter receiptPrinter = new ReceiptPrinter();
                receiptPrinter.Start("Print data will go here");

            }
            else if (data == "pinpad")
            {
                Paypinpad paypinpad = new Paypinpad();
                paypinpad.Start();
                paypinpad.PinEntered += PayPinpad_PinEntered;
            }
            else if (data == "msr")
            {
                PayMSR paymsr = new PayMSR();
                paymsr.Start();
                paymsr.CardSwiped += PayMsr_CardSwiped;
            }
            else
            {
                Console.WriteLine("Nothing Invoked");
            }
        }

      

        private  static void PayMsr_CardSwiped(object sender, CardSwipeEventArgs e)
        {
            Paypinpad paypinpad = new Paypinpad();
            paypinpad.Start(e);
            paypinpad.PinEntered += PayPinpad_PinEntered;
        }

        private async static void BarcodeScanner_Scanned(object sender, ScannedEventArgs e)
        {
           await context.Clients.All.sendScannedData(e.BarcodeId, e.DeviceId);
        }

        private async static void PayPinpad_PinEntered(object sender, PinEnteredEventArgs e)
        {
            await context.Clients.All.paymentComplete(e);
        }
    }

    

    
}
