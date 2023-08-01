using System;

namespace upos_device_simulation.Models
{
    public class ScannedEventArgs : EventArgs
    {
        public string BarcodeId { get; set; }
        public string DeviceId { get; set; }

    }
}
