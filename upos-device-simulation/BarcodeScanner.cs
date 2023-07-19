using System;
using System.Text;
using Microsoft.PointOfService;
using Microsoft.PointOfService.DeviceSimulators;

namespace upos_device_simulation
{

  
    public sealed class BarcodeScanner
    {
        private PosExplorer posExplorer;
        private Scanner scanner;
        public  event EventHandler<ScannedEventArgs> Scanned;
        private static readonly BarcodeScanner barcodeScanner = new BarcodeScanner();
        ScannerSimulator scannerSimulator=new ScannerSimulator();
        private  BarcodeScanner()
        {
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }
        public static BarcodeScanner Instance
        {
            get
            {
                return barcodeScanner;
            }
        }
        public void Start()
        {
            DeviceInfo device = posExplorer.GetDevices(DeviceType.Scanner)[0];
            scanner = (Scanner)posExplorer.CreateInstance(device);
            scanner.Open();
            scanner.Claim(1000);
            scanner.DataEvent += new DataEventHandler(scanner_DataEvent);
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
            
            
        }

         void scanner_DataEvent(object sender, DataEventArgs e)
        {
            
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            var scandata = asciiEncoding.GetString(scanner.ScanDataLabel);
            Console.WriteLine(scandata);
            Scanned?.Invoke(null, new ScannedEventArgs { BarcodeId = scandata,DeviceId="1" });
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
        }

         void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "Scanner")
            {
                scanner.DataEventEnabled = false;
                scanner.DeviceEnabled = false;
                scanner.Release();
                scanner.Close();
                Console.WriteLine("Scanner removed");
            }
        }
        public void Scanner_InputDataEvent(string barcode)
        {
            scannerSimulator.InputDataEvent(barcode, BarCodeSymbology.Unknown);
        }
         void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "Scanner")
            {
                scanner = (Scanner)posExplorer.CreateInstance(e.Device);
                scanner.Open();
                scanner.Claim(1000);
                scanner.DataEvent += new DataEventHandler(scanner_DataEvent);
                scanner.DeviceEnabled = true;
                scanner.DataEventEnabled = true;
                scanner.DecodeData = true;
                Console.WriteLine("Scanner Attached");
            }
        }
    }
}
