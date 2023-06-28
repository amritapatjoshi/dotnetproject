using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
namespace POSServiceDriver
{
    public class POSController: ApiController
    {
        public string GetPOSString()
        {
            return "test string";
        }
    }
}
