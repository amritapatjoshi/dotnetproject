using Microsoft.PointOfService;
using System;
using System.Text;
using upos_device_simulation.Interfaces;
using upos_device_simulation.Models;

namespace upos_device_simulation
{

    public class Paypinpad : IPaypinpad
    {
        private readonly PosExplorer posExplorer;
        private PinPad pinpad;
        public event EventHandler<PinEnteredEventArgs> PinEntered;
        ILogger logger;
        public CardSwipeEventArgs CardSwipeInfo { get; set; }
        public Paypinpad( ILogger logger)
        {
            this.logger = logger;
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        public void Start(CardSwipeEventArgs cardInfo=null)
        {
            logger.Info("Getting PinPad Device from posExplorer.");
            DeviceInfo device = posExplorer.GetDevices(DeviceType.PinPad)[0];
            logger.Info("Got Pinpad Device");
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
                int transactionHost = int.Parse("1", System.Globalization.CultureInfo.CurrentCulture);
                pinpad.BeginEftTransaction(pps, transactionHost);
                enterPin();
                logger.Info("Pinpad Started");
            }

        }

        internal void enterPin()
        {
            pinpad.EnablePinEntry();
        }

        internal void EndEftTransaction()
        {
            pinpad.EndEftTransaction(EftTransactionCompletion.Normal);
        }

        void pinpad_DataEvent(object sender, DataEventArgs e)
        {
            if ((PinEntryStatus)e.Status == PinEntryStatus.Success)
                logger.Info("EncryptedPIN = " + pinpad.EncryptedPin + "\r\nAdditionalSecurityInformation = " + pinpad.AdditionalSecurityInformation);
            else if ((PinEntryStatus)e.Status == PinEntryStatus.Cancel)
                logger.Info("Pin entry was cancelled.");
            else if ((PinEntryStatus)e.Status == PinEntryStatus.Timeout)
                logger.Info("A timeout condition occured in the pinpad.");
            else
                logger.Info("pinpad returned status code: " + e.Status.ToString(System.Globalization.CultureInfo.CurrentCulture));

            postData(pinpad.EncryptedPin,pinpad.AccountNumber,pinpad.Amount,"1",e.Status ==1 ?true:false);

        }

        void postData(string pinpaddata,string accountNumber,decimal ammount,string deviceId,bool paymentstatus)
        {
            PinEntered?.Invoke(this, new PinEnteredEventArgs 
            { 
                PinData = pinpaddata,
                AccountNumber=accountNumber,
                Amount=ammount,
                DeviceId=deviceId,
                PaymentStatus=paymentstatus ,
                Name=CardSwipeInfo.Name,
                ExpirationDate=CardSwipeInfo.ExpirationDate,
                ServiceCode=CardSwipeInfo.ServiceCode

            });
            logger.Info("User enter Pin: "+pinpaddata);

            logger.Info("Pinpad Transaction Ended.");

            pinpad.DeviceEnabled = false;
            pinpad.DataEventEnabled = false;
            pinpad.Release();
            pinpad.Close();

        }
        void pinpad_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == PinPad.ExtendedErrorBadKey)
            
                logger.Error("An encryption key is corrupted or missing.");
            
            else
                logger.Error("Error while reading user entered pin.", e.ErrorCode);
        
        }

        void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "pinpad")
            {
                pinpad.DeviceEnabled = false;
                pinpad.Release();
                pinpad.Close();
                logger.Info("Pinpad removed.");
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
                logger.Info("Pinpad Attached.");
            }
        }



    }
}
