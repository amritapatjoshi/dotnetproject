using System;
using System.Management;
using System.Text;
using Microsoft.PointOfService;
using upos_device_simulation.Interfaces;
using upos_device_simulation.Models;
using OposScanner_CCO;

namespace upos_device_simulation.Services
{

    public sealed class OposScanner : IBarcodeScanner
    {
        private ILogger logger;
        private PosExplorer posExplorer;
        private OPOSScanner scanner;
        public event EventHandler<ScannedEventArgs> Scanned;
        string deviceName = "RS232_SCANNER_ANY";
        public OposScanner(ILogger logger, OPOSScanner scanner)
        {
            this.logger = logger;
            this.scanner = scanner;
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        public void Start()
        {
            logger.Info("Getting Scanner form posExplorer.");
            //var devices = posExplorer.GetDevices(DeviceType.Scanner)[4];
            logger.Info("Got Scanner");
            //scanner = new OPOSScannerClass();
            scanner.Open(deviceName);
            scanner.ClaimDevice(1000);
            scanner.DataEvent += scanner_DataEvent;
            scanner.ErrorEvent += scanner_ErrorEvent;
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
            logger.Info("Scanner Started");
        }

        private void scanner_ErrorEvent(int ResultCode, int ResultCodeExtended, int ErrorLocus, ref int pErrorResponse)//(object sender, DeviceErrorEventArgs e)
        {
            logger.Error("Error occured while scanning product." + ResultCode);
        }
        public string CheckDeviceHealth()
        {
            try
            {
                string res = scanner.CheckHealthText;
                return "CheckHealth(Internal) returned: " + res;

            }
            catch (Exception ex)
            {
                return logger.GetPosException(ex);
            }
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
                logger.Error("Error occured while getting Device ID." + ex.Message + ex.StackTrace);
                return "1";
            }
        }
        private void scanner_DataEvent(int value)//(object sender, DataEventArgs e)
        {
            logger.Info(CheckDeviceHealth());
            string deviceId = GetDeviceId();
            var scandata = scanner.ScanDataLabel;
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
                scanner.ReleaseDevice();
                scanner.Close();
                logger.Info("Scanner removed");
            }
        }

        private void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "Scanner")
            {
                scanner = new OPOSScannerClass();

                string profileName = "RS232_SCANNER_ANY";
                scanner.Open(profileName);
                scanner.ClaimDevice(1000);
                scanner.DataEvent += scanner_DataEvent;
                scanner.DeviceEnabled = true;
                scanner.DataEventEnabled = true;
                scanner.DecodeData = true;
                logger.Info("Scanner Attached");
            }
        }
    }
}
