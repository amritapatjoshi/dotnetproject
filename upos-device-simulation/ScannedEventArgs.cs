using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upos_device_simulation
{
    public class ScannedEventArgs : EventArgs
    {
        public string BarcodeId { get; set; }
        public string DeviceId { get; set; }

    }
}
