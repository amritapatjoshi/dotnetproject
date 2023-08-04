using System;
using System.Threading;
using Microsoft.PointOfService;
namespace upos_device_simulation
{

    internal class ReceiptPrinter
    {
        private PosExplorer posExplorer;
        private PosPrinter printer;
        public Thread printerThread { get; set; }

        public ReceiptPrinter()
        {
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        internal void Start(object printData)
        {
            DeviceInfo device = posExplorer.GetDevices(DeviceType.PosPrinter)[0];
            printer = (PosPrinter)posExplorer.CreateInstance(device);
            printer.Open();
            Print((string)printData);
        }


        void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "printer")
            {
                printer.DeviceEnabled = false;
                printer.Release();
                printer.Close();
                Console.WriteLine("Printer removed");
            }
        }

         void Print(string data)
        {
            if (!printer.Claimed)
            {
                printer.Claim(10000);
                printer.DeviceEnabled = true;
            }
            if (!printer.CoverOpen)
            {
                printer.PrintNormal(PrinterStation.Receipt, data);
                Thread.Sleep(50000);
                printer.DeviceEnabled = false;
                printer.ClearOutput();
                printer.Release();
                printer.Close();

            }
            else
            {
                Console.WriteLine("Close Printer Cover");
            }
        }
        void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "printer")
            {
                printer = (PosPrinter)posExplorer.CreateInstance(e.Device);
                printer.Open();
                printer.Claim(1000);
                printer.DeviceEnabled = true;
                Console.WriteLine("printer Attached");
            }
        }
    }
}
