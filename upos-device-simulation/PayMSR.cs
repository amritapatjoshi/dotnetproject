using Microsoft.PointOfService;
using System;
using System.Text;

namespace upos_device_simulation
{
    public class PayMSR
    {

        private PosExplorer posExplorer;
        private Msr msr;
        public event EventHandler<CardSwipeEventArgs> CardSwiped;
        public PayMSR()
        {
            posExplorer = new PosExplorer();
            posExplorer.DeviceAddedEvent += new DeviceChangedEventHandler(posExplorer_DeviceAddedEvent);
            posExplorer.DeviceRemovedEvent += new DeviceChangedEventHandler(posExplorer_DeviceRemovedEvent);
        }

        public void Start()
        {
            DeviceInfo device = posExplorer.GetDevices(DeviceType.Msr)[0];
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
            }

        }
       
        void msr_DataEvent(object sender, DataEventArgs e)
        {
            string name = msr.FirstName + " " + msr.MiddleInitial+" " + msr.Surname;
            CardSwipeEventArgs cardInfo = new CardSwipeEventArgs();
            cardInfo.AccountNumber = msr.AccountNumber;
            cardInfo.Name = name;
            cardInfo.ExpirationDate = msr.ExpirationDate;
            cardInfo.Suffix = msr.Suffix;
            cardInfo.ServiceCode = msr.ServiceCode;
            cardInfo.Title = msr.Title;
            Console.WriteLine(name + " " + msr.AccountNumber);
            CardSwiped?.Invoke(this, cardInfo);
            msr.DeviceEnabled = false;
            msr.DataEventEnabled = false;
            msr.Release();
            msr.Close();


        }
       
        void msr_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            Console.WriteLine(e.ErrorCode);
        }

        void posExplorer_DeviceRemovedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "msr")
            {

                msr.DeviceEnabled = false;
                msr.Release();
                msr.Close();
                Console.WriteLine("MSR removed");
            }

        }

        void posExplorer_DeviceAddedEvent(object sender, DeviceChangedEventArgs e)
        {
            if (e.Device.Type == "msr")
            {
                msr = (Msr)posExplorer.CreateInstance(e.Device);
                msr.Open();
                msr.Claim(1000);
                msr.DeviceEnabled = true;
                Console.WriteLine("MSR Attached");
            }
        }
    }
}
