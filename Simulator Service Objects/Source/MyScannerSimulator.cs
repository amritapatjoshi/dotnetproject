using Microsoft.PointOfService.BaseServiceObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PointOfService.DeviceSimulators
{
    [ServiceObjectAttribute(DeviceType.Scanner,
        "Microsoft Scanner Simulator1",
       "Simulated service object for scanner 1", 1, 14)]
    /// <summary>
    /// Simulate a scanner by enabling a UI to manually trigger scan events. Additionally provide automated interface
    /// through an XML scan file.
    /// </summary>
    public class MyScannerSimulator : ScannerBase
    {
        // private ScannerSimulatorWindow Window;

        public MyScannerSimulator()
        {
            // This is a non-Pnp device so we must set its device path here
            DevicePath = "Microsoft Scanner Simulator1";
            Properties.DeviceDescription = "Microsoft Scanner Simulator";
        }

        ~MyScannerSimulator()
        {
            Dispose(false);
        }


        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Logger.Info("ScannerSimulator", "Disposing class: " + this.ToString());
                    // Release the managed resources you added in
                    // this derived class here.
                }
                // Release the native unmanaged resources
                // WindowThread needs to be treated as an unmanaged resource because
                // it is a class that contain threads and windows and won't get collected
                // by the GC
                //if (Window != null)
                //    Window.Close();
                //Window = null;


            }
            finally
            {
                // Call Dispose on your base class.
                base.Dispose(disposing);
            }
        }



        private string DataToHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 4);
            // decode the byte array containing scan data back to a string.
            foreach (byte b in data)
            {
                sb.Append(b.ToString("X", CultureInfo.InvariantCulture));
                sb.Append(" ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// A good scan event occurred: queue it and update the GoodScanCount statistic
        /// </summary>
        /// <param name="d">Scan event arguments</param>
        public void InputDataEvent(string data)
        {
            InputDataEvent(data, BarCodeSymbology.Unknown);
        }


        /// <summary>
        /// A good scan event occurred: queue it and update the GoodScanCount statistic
        /// </summary>
        /// <param name="d">Scan event arguments</param>
        public void InputDataEvent(string data, BarCodeSymbology type)
        {
            if (data == null)
                return;

            byte[] b = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                b[i] = (byte)data[i];

            Logger.Info("Scanner", "Scan event: " + DataToHex(b) + ", type=" + type);

            GoodScan(b, type, b);
        }

        /// <summary>
        /// A bad scan event occurred: queue it
        /// </summary>
        /// <param name="d">Error event arguments</param>
        public void InputErrorEvent()
        {
            Logger.Info("Scanner", "Bad Scan event. ");
            FailedScan();
        }

        #region Overrides
        override public void Open()
        {
            // Device State checking done in base class
            base.Open();

            // Set values for common statistics
            SetStatisticValue(StatisticManufacturerName, "Microsoft Corporation");
            SetStatisticValue(StatisticManufactureDate, "2004-05-23");
            SetStatisticValue(StatisticModelName, "Scanner Simulator");
            SetStatisticValue(StatisticMechanicalRevision, "1.0");
            SetStatisticValue(StatisticInterface, "Other");

            // Initialize the CheckHealthText property to an empty string
            checkhealthtext = "";

            // show simulation window
            // Window = new ScannerSimulatorWindow(this);
        }

        private string checkhealthtext;
        public override string CheckHealthText
        {
            get
            {
                // Verify that device is open
                VerifyState(false, false);

                return checkhealthtext;
            }
        }


        public override string CheckHealth(HealthCheckLevel level)
        {
            // Verify that device is open, claimed and enabled
            VerifyState(true, true);

            // TODO: check the health of the device and return a descriptive string 

            // Cache result in the CheckHealthText property
            checkhealthtext = "Ok";
            return checkhealthtext;
        }

        public override DirectIOData DirectIO(int command, int data, object obj)
        {
            // Verify that device is open
            VerifyState(false, false);

            return new DirectIOData(data, obj);
        }

        #endregion Overrides

    }
}
