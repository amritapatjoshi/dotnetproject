using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.Configuration;
using upos_device_simulation.Models;

using upos_device_simulation.Interfaces;

namespace UposDeviceSimulationConsole
{
    public class PosExecutor
    {
        ILogger logger;
        static IHubContext context = GlobalHost.ConnectionManager.GetHubContext<PosHub>();
        private static readonly HttpClient client = new HttpClient();
        public static string defaultSimulatorType = "scan";
        IBarcodeScanner barcodeScanner;
        IReceiptPrinter receiptPrinter;
        IPayMSR payMSR;
        IPaypinpad paypinpad;
       
        public PosExecutor(ILogger logger, IBarcodeScanner barcodeScanner, IPaypinpad paypinpad, IPayMSR payMSR, IReceiptPrinter receiptPrinter)
        {
            this.logger = logger;   
            this.barcodeScanner=barcodeScanner;
            this.paypinpad=paypinpad;
            this.payMSR = payMSR;
            this.receiptPrinter=receiptPrinter;
        }
        public void Execute()
        {
            try
            {
                logger.Info("upos Service started.");
                string url = ConfigurationManager.AppSettings["SignalrUrl"];
                using (WebApp.Start<SignalRStartup>(url))
                {
                    logger.Info("The Server URL is: " + url);
                    InvokeSimulator(defaultSimulatorType);
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured in PosExecutor" + ex.Message );
            }
           
        }
        public void InvokeSimulator(object obj)
        {
            string data = (string)obj;
            if (data == "scan")
            {
                logger.Info("Starting scanner simmulator");
                barcodeScanner.Scanned += BarcodeScanner_Scanned;
                barcodeScanner.Start();
               barcodeScanner.CheckDeviceHealth();
            }
            else if (data == "print")
            {
                logger.Info("Starting Printer simmulator");
                receiptPrinter.Start("Print data will go here");

            }
            else if (data == "pinpad")
            {
                logger.Info("Starting Pinpad simmulator");
                paypinpad.PinEntered += PayPinpad_PinEntered;
                paypinpad.Start();
                
            }
            else if (data == "msr")
            {
                logger.Info("Starting MSR simmulator");
                payMSR.CardSwiped += PayMsr_CardSwiped;
                payMSR.Start();
                
            }
            else
            {
                Console.WriteLine("Nothing Invoked");
            }
        }
        private  void PayMsr_CardSwiped(object sender, CardSwipeEventArgs e)
        {
            try
            {
                logger.Info("Card Swiped and read user card Details ");
                logger.Info("starting Pinpad simmulator");
                paypinpad.PinEntered += PayPinpad_PinEntered;
                paypinpad.Start(e);
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured in PayMsr_CardSwiped." + ex.Message + ex.StackTrace);
            }
           
        }

        private async  void BarcodeScanner_Scanned(object sender, ScannedEventArgs e)
        {
            try 
            { 
                logger.Info("Barcode  : " + e.BarcodeId + " scanned and sending to UI");
                await context.Clients.All.sendScannedData(e.BarcodeId, e.DeviceId);
                logger.Info("Barcode send to UI");
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured in BarcodeScanner_Scanned." + ex.Message + ex.StackTrace);
            }
        }

        private async  void PayPinpad_PinEntered(object sender, PinEnteredEventArgs e)
        {
            try
            { 
                logger.Info("Pin Enterd by " + e.Name + " and sending Payment details to UI");
                await context.Clients.All.paymentComplete(e);
                logger.Info("Payment details send to UI");
            }
            catch (Exception ex)
            {
                logger.Error("Exception occured in PayPinpad_PinEntered." + ex.Message + ex.StackTrace);
            }

        }
    }
}
