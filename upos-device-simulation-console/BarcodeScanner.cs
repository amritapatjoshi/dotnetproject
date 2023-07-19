using System;
using System.Text;
using Microsoft.PointOfService;
namespace upos_device_simulation
{

    internal class ScannedEventArgs : EventArgs
    {
        public string BarcodeId { get; set; }
        public string DeviceId { get; set; }

    }
    internal static class BarcodeScanner
    {
        private static PosExplorer posExplorer;
        private static Scanner scanner;
        public static event EventHandler<ScannedEventArgs> Scanned;
         static BarcodeScanner()
        {
            posExplorer = new PosExplorer();
            //posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            //posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        internal static void Start()
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

        static void scanner_DataEvent(object sender, DataEventArgs e)
        {
            
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            var scandata = asciiEncoding.GetString(scanner.ScanDataLabel);
            Console.WriteLine(scandata);
            Scanned?.Invoke(null, new ScannedEventArgs { BarcodeId = scandata,DeviceId="1" });
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
        }

        static void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
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

        static void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
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
