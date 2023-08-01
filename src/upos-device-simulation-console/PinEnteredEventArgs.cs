using System;

namespace upos_device_simulation
{
    internal class PinEnteredEventArgs: EventArgs
    {
        public string PinData { get; set; }
        public string AccountNumber { get; set; }
        public decimal Ammount { get; set; }
        public string DeviceId { get; set; }
        public bool PaymentStatus { get; set; }
    }
}
