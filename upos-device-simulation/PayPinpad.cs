using Microsoft.PointOfService;
using System;
using System.Text;

namespace upos_device_simulation
{

    public class Paypinpad
    {
        private readonly PosExplorer posExplorer;
        private PinPad pinpad;
        public event EventHandler<PinEnteredEventArgs> PinEntered;
        public CardSwipeEventArgs CardSwipeInfo { get; set; }
        public Paypinpad()
        {
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        public void Start(CardSwipeEventArgs cardInfo=null)
        {
            //DeviceInfo device = posExplorer.GetDevice("pinpad", "mypinpad");
            DeviceInfo device = posExplorer.GetDevices(DeviceType.PinPad)[0];
            if (pinpad == null)
            {
                if (cardInfo!=null)
                    CardSwipeInfo = cardInfo;
                pinpad = (PinPad)posExplorer.CreateInstance(device);
                pinpad.DataEvent += new DataEventHandler(pinpad_DataEvent);
                pinpad.ErrorEvent += new DeviceErrorEventHandler(pinpad_ErrorEvent);
                pinpad.Open();
                pinpad.Claim(1000);
                pinpad.DeviceEnabled = true;
                pinpad.DataEventEnabled = true;
                pinpad.Track1Data = Encoding.ASCII.GetBytes("test data12");
                pinpad.Track2Data = Encoding.ASCII.GetBytes("test data2");
                pinpad.Amount = decimal.Parse("220", System.Globalization.CultureInfo.CurrentCulture);
                pinpad.TerminalId = "T1";
                pinpad.MerchantId = "M1";
                pinpad.AccountNumber = cardInfo.AccountNumber;
                PinPadSystem pps = (PinPadSystem)Enum.Parse(typeof(PinPadSystem), PinPadSystem.Dukpt.ToString());
                int th = int.Parse("1", System.Globalization.CultureInfo.CurrentCulture);
                pinpad.BeginEftTransaction(pps, th);
                enterPin();

            }

        }

        internal void enterPin()
        {
            pinpad.EnablePinEntry();
        }

        internal void EndEftTransaction()
        {
            pinpad.EndEftTransaction(EftTransactionCompletion.Normal);
            //Console.WriteLine("Transaction Ended");
        }

        void pinpad_DataEvent(object sender, DataEventArgs e)
        {
            if ((PinEntryStatus)e.Status == PinEntryStatus.Success)
                DisplayMessage("EncryptedPIN = " + pinpad.EncryptedPin + "\r\nAdditionalSecurityInformation = " + pinpad.AdditionalSecurityInformation);
            else if ((PinEntryStatus)e.Status == PinEntryStatus.Cancel)
                DisplayMessage("Pin entry was cancelled.");
            else if ((PinEntryStatus)e.Status == PinEntryStatus.Timeout)
                DisplayMessage("A timeout condition occured in the pinpad.");
            else
                DisplayMessage("pinpad returned status code: " + e.Status.ToString(System.Globalization.CultureInfo.CurrentCulture));

            postData(pinpad.EncryptedPin,pinpad.AccountNumber,pinpad.Amount,"1",e.Status ==1 ?true:false);

        }

        void postData(string pinpaddata,string accountNumber,decimal ammount,string deviceId,bool paymentstatus)
        {
            PinEntered?.Invoke(this, new PinEnteredEventArgs 
            { 
                PinData = pinpaddata,
                AccountNumber=accountNumber,
                Ammount=ammount,
                DeviceId=deviceId,
                PaymentStatus=paymentstatus ,
                Name=CardSwipeInfo.Name,
                ExpirationDate=CardSwipeInfo.ExpirationDate,
                ServiceCode=CardSwipeInfo.ServiceCode

            });
            Console.WriteLine(pinpaddata);
           
            Console.WriteLine("Pinpad Transaction Ended");

            pinpad.DeviceEnabled = false;
            pinpad.DataEventEnabled = false;
            pinpad.Release();
            pinpad.Close();

        }
        void pinpad_ErrorEvent(object sender, DeviceErrorEventArgs e)
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
            if (e.Device.Type == "pinpad")
            {

                pinpad.DeviceEnabled = false;
                pinpad.Release();
                pinpad.Close();
                Console.WriteLine("Pinpad removed");
            }
        }

        void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "pinpad")
            {
                pinpad = (PinPad)posExplorer.CreateInstance(e.Device);
                pinpad.Open();
                pinpad.Claim(1000);
                pinpad.DeviceEnabled = true;
                Console.WriteLine("Pinpad Attached");
            }
        }



    }
}
