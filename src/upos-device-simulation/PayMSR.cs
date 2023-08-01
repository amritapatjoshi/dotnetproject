using Microsoft.PointOfService;
using System;
using upos_device_simulation.Interfaces;
using upos_device_simulation.Models;

namespace upos_device_simulation
{
    public class PayMSR: IPayMSR
    {

        private PosExplorer posExplorer;
        private Msr msr;
        public event EventHandler<CardSwipeEventArgs> CardSwiped;
        ILogger logger;
        public PayMSR( ILogger logger)
        {
            this.logger = logger;
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        public void Start()
        {
            logger.Info("Getting MSR Device from posExplorer.");
            DeviceInfo device = posExplorer.GetDevices(DeviceType.Msr)[0];
            logger.Info("Got MSR Device.");
            if (msr == null)
            {
                msr = (Msr)posExplorer.CreateInstance(device);
                msr.DataEvent += new DataEventHandler(msr_DataEvent);
                msr.ErrorEvent += new DeviceErrorEventHandler(msr_ErrorEvent);
                msr.Open();
                msr.Claim(1000);
                msr.DeviceEnabled = true;
                msr.DataEventEnabled = true;
                msr.DecodeData = true;
                msr.ParseDecodeData = true;
                logger.Info("MSR Started.");
            }
        }
       
        private void msr_DataEvent(object sender, DataEventArgs e)
        {

            string name = msr.FirstName + " " + msr.MiddleInitial+" " + msr.Surname;
            CardSwipeEventArgs cardInfo = new CardSwipeEventArgs();
            cardInfo.AccountNumber = msr.AccountNumber;
            cardInfo.Name = name;
            cardInfo.ExpirationDate = msr.ExpirationDate;
            cardInfo.Suffix = msr.Suffix;
            cardInfo.ServiceCode = msr.ServiceCode;
            cardInfo.Title = msr.Title;
            logger.Info("Card Swiped. Card Details is as below  AccountNumber: " + cardInfo.AccountNumber + " Name : " + cardInfo.Name + " ExpirationDate: " + cardInfo.ExpirationDate + " Service Code" + cardInfo.ServiceCode);
            CardSwiped?.Invoke(this, cardInfo);
            msr.DeviceEnabled = false;
            msr.DataEventEnabled = false;
            msr.Release();
            msr.Close();
           
        }
       
        private void msr_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == Msr.ExtendedErrorStart)
                logger.Error("Indicates a start sentinel error.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == Msr.ExtendedErrorEnd)
                logger.Error("Indicates an end sentinel error.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == Msr.ExtendedErrorEnd)
                logger.Error("Indicates an end sentinel error.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == Msr.ExtendedErrorParity)
                logger.Error("Indicates a parity error");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == Msr.ExtendedErrorLrc)
                logger.Error("Indicates an LRC error.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == Msr.ExtendedErrorDeviceAuthenticationFailed)
                logger.Error("ndicates an extended error where the device authentication process failed.");
            else if (e.ErrorCode == ErrorCode.Extended || e.ErrorCodeExtended == Msr.ExtendedErrorDeviceDeauthenticationFailed)
                logger.Error("Indicates an extended error where the device deauthentication failed.");
            else
                logger.Error("Error while reading swiped card details.", e.ErrorCode);
        }

        private void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "msr")
            {
                msr.DeviceEnabled = false;
                msr.Release();
                msr.Close();
                logger.Info("MSR removed");
            }
        }

        private void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "msr")
            {
                msr = (Msr)posExplorer.CreateInstance(e.Device);
                msr.Open();
                msr.Claim(1000);
                msr.DeviceEnabled = true;
                logger.Info("MSR Attached");
            }
        }
    }
}
