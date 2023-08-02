using System;
using System.Management;
using System.Text;
using Microsoft.PointOfService;
using upos_device_simulation.Interfaces;
using upos_device_simulation.Models;

namespace upos_device_simulation.Services
{

    public sealed class BarcodeScanner : IBarcodeScanner
    {
        private ILogger logger;
        private PosExplorer posExplorer;
        private Scanner scanner;
        public event EventHandler<ScannedEventArgs> Scanned;
        public BarcodeScanner(ILogger logger)
        {
            this.logger = logger;
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        public void Start()
        {
            logger.Info("Getting Scanner form posExplorer.");
            DeviceInfo device = posExplorer.GetDevices(DeviceType.Scanner)[0];
            logger.Info("Got Scanner");
            scanner = (Scanner)posExplorer.CreateInstance(device);
            scanner.Open();
            scanner.Claim(1000);
            scanner.DataEvent += new DataEventHandler(scanner_DataEvent);
            scanner.ErrorEvent += new DeviceErrorEventHandler(scanner_ErrorEvent);
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
            logger.Info("Scanner Started");
        }

        private void scanner_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            logger.Error("Error occured while scanning product.", e.ErrorCode);
        }
        private string GetDeviceId()
        {
            try
            {
                string cpuInfo = string.Empty;
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (cpuInfo == String.Empty)
                    {   
                        cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    }
                }
                return cpuInfo;
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while getting Device ID."+ex.Message+ex.StackTrace);
                return "1";
            }
        }
        private void scanner_DataEvent(object sender, DataEventArgs e)
        {
            string deviceId = GetDeviceId();
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            var scandata = asciiEncoding.GetString(scanner.ScanDataLabel);
            logger.Info("Barcode Scanned " + scandata);
            Scanned?.Invoke(null, new ScannedEventArgs { BarcodeId = scandata, DeviceId = deviceId });
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
        }
        private void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "Scanner")
            {
                scanner.DataEventEnabled = false;
                scanner.DeviceEnabled = false;
                scanner.Release();
                scanner.Close();
                logger.Info("Scanner removed");
            }
        }

        private void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
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
                logger.Info("Scanner Attached");
            }
        }
    }
}
