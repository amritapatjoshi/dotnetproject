using System;
using System.Threading;
using Microsoft.PointOfService;
using upos_device_simulation.Interfaces;

namespace upos_device_simulation
{

    public class ReceiptPrinter: IReceiptPrinter
    {
        private ILogger logger;
        private PosExplorer posExplorer;
        private PosPrinter printer;
        public ReceiptPrinter(ILogger logger)
        {
            this.logger = logger;
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        public void Start(object printData)
        {
            logger.Info("Getting Printer");
            DeviceInfo device = posExplorer.GetDevices(DeviceType.PosPrinter)[0];
            logger.Info("Got Printer");
            printer = (PosPrinter)posExplorer.CreateInstance(device);
            printer.Open();
            printer.ErrorEvent += new DeviceErrorEventHandler(printer_ErrorEvent);
            Print((string)printData);
            logger.Info("Printer started");
        }

        private void printer_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorCoverOpen)
                logger.Error("Indicates that the printer cover is open.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorJournalEmpty)
                logger.Error("Indicates the journal station is out of paper.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorReceiptEmpty)
                logger.Error("Indicates the receipt station is out of paper.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorSlipEmpty)
                logger.Error("Indicates a form has not been inserted into the slip station.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorSlipForm)
                logger.Error("Indicates a form is present while the printer is being taken out of from removal");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorTooBig)
                logger.Error("Indicates the bitmap is either too wide to print without transformation, or too big to tranform.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorBadFormat)
                logger.Error("Indicates an unsupported format.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorJournalCartridgeRemoved)
                logger.Error(" Indicates the journal cartridge has been removed..");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorJournalCartridgeEmpty)
                logger.Error("Indicates the journal cartridge is empty.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorJournalHeadCleaning)
                logger.Error("Indicates the journal cartridge head is being cleaned.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorReceiptCartridgeRemoved)
                logger.Error("Indicates the receipt cartridge has been removed.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorReceiptCartridgeEmpty)
                logger.Error("Indicates the receipt cartridge is empty.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorReceiptHeadCleaning)
                logger.Error("Indicates the receipt cartridge head is being cleaned.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorSlipCartridgeRemoved)
                logger.Error(" Indicates the slip cartridge has been removed.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorSlipCartridgeEmpty)
                logger.Error("Indicates the slip cartridge is empty.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PosPrinter.ExtendedErrorSlipHeadCleaning)
                logger.Error("Indicates the slip cartridge head is being cleaned.");
            else
                logger.Error("Error occured while Printing receipt." + e.ErrorCode);
        }

        void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "printer")
            {
                printer.DeviceEnabled = false;
                printer.Release();
                printer.Close();
                logger.Info("Printer removed");
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
                logger.Info("Printed receipt " + data);
                Thread.Sleep(50000);
                printer.DeviceEnabled = false;
                printer.ClearOutput();
                printer.Release();
                printer.Close();

            }
            else
            {
                logger.Info("Close Printer Cover");
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
                logger.Info("printer Attached");
            }
        }
    }
}
