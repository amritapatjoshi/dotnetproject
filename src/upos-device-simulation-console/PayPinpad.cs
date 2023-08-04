using Microsoft.PointOfService;
using System;
using System.Net.Http;
using System.Text;

namespace upos_device_simulation
{
    internal class PayPinpad
    {
        private PosExplorer posExplorer;
        private PinPad pinPad;
        public event EventHandler<PinEnteredEventArgs> PinEntered;
        public PayPinpad()
        {
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        internal void Start()
        {
            //DeviceInfo device = posExplorer.GetDevice("PinPad", "myPinpad");
            DeviceInfo device = posExplorer.GetDevices(DeviceType.PinPad)[0];
            if (pinPad == null)
            {
                pinPad = (PinPad)posExplorer.CreateInstance(device);
                pinPad.DataEvent += new DataEventHandler(pinPad_DataEvent);
                pinPad.ErrorEvent += new DeviceErrorEventHandler(pinPad_ErrorEvent);
                pinPad.Open();
                pinPad.Claim(1000);
                pinPad.DeviceEnabled = true;
                pinPad.DataEventEnabled = true;
                pinPad.Track1Data = Encoding.ASCII.GetBytes("test data12");
                pinPad.Track2Data = Encoding.ASCII.GetBytes("test data2");
                pinPad.Amount = decimal.Parse("220", System.Globalization.CultureInfo.CurrentCulture);
                pinPad.TerminalId = "T1";
                pinPad.MerchantId = "M1";
                pinPad.AccountNumber = "1234567456";
                PinPadSystem pps = (PinPadSystem)Enum.Parse(typeof(PinPadSystem), PinPadSystem.Dukpt.ToString());
                int th = int.Parse("1", System.Globalization.CultureInfo.CurrentCulture);
                pinPad.BeginEftTransaction(pps, th);
                enterPin();

            }

        }

        internal void enterPin()
        {
            pinPad.EnablePinEntry();
        }

        internal void EndEftTransaction()
        {
            pinPad.EndEftTransaction(EftTransactionCompletion.Normal);
        }

        void pinPad_DataEvent(object sender, DataEventArgs e)
        {
            if ((PinEntryStatus)e.Status == PinEntryStatus.Success)
                DisplayMessage("EncryptedPIN = " + pinPad.EncryptedPin + "\r\nAdditionalSecurityInformation = " + pinPad.AdditionalSecurityInformation);
            else if ((PinEntryStatus)e.Status == PinEntryStatus.Cancel)
                DisplayMessage("Pin entry was cancelled.");
            else if ((PinEntryStatus)e.Status == PinEntryStatus.Timeout)
                DisplayMessage("A timeout condition occured in the pinpad.");
            else
                DisplayMessage("Pinpad returned status code: " + e.Status.ToString(System.Globalization.CultureInfo.CurrentCulture));

            postData(pinPad.EncryptedPin,pinPad.AccountNumber,pinPad.Amount,"1",e.Status ==1 ?true:false);

        }

        void postData(string pinpaddata,string accountNumber,decimal ammount,string deviceId,bool paymentstatus)
        {
            PinEntered?.Invoke(this, new PinEnteredEventArgs { PinData = pinpaddata,AccountNumber=accountNumber,Ammount=ammount,DeviceId=deviceId,PaymentStatus=paymentstatus });
            Console.WriteLine(pinpaddata);
           
            Console.WriteLine("Pinpad Transaction Ended");

            pinPad.DeviceEnabled = false;
            pinPad.DataEventEnabled = false;
            pinPad.Release();
            pinPad.Close();

        }
        void pinPad_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PinPad.ExtendedErrorBadKey)
            {
                DisplayMessage("An encryption key is corrupted or missing.");
            }
        }

        protected void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
        void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "PinPad")
            {

                pinPad.DeviceEnabled = false;
                pinPad.Release();
                pinPad.Close();
                Console.WriteLine("Printer removed");
            }
        }

        void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "PinPad")
            {
                pinPad = (PinPad)posExplorer.CreateInstance(e.Device);
                pinPad.Open();
                pinPad.Claim(1000);
                pinPad.DeviceEnabled = true;
                Console.WriteLine("Scanner Attached");
            }
        }



    }
}
