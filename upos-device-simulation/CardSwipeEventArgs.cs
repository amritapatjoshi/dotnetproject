using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace upos_device_simulation
{
    public class CardSwipeEventArgs
    {
        public string Title { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string ExpirationDate { get; set; }
        public string ServiceCode { get; set; }
        public string Suffix { get; set; }
    }
}
