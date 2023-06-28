using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Cors;
using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.PointOfService;
using System.Net.Http;
using Newtonsoft.Json;
using Owin;
namespace POSSignalRConsole
{
    class Program
    {
        //static var _hubContext;
        static Scanner scanner;
        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {

            string url = "http://localhost:6118";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("The Server URL is: {0}", url);
                StartScannerSimulator();
                Console.ReadLine();
            }
        }

        private static void StartScannerSimulator()
        {

            PosExplorer posExplorer = new PosExplorer();
            var scannerDeviceInfo = posExplorer.GetDevices(DeviceType.Scanner)[0];

            scanner = (Scanner)posExplorer.CreateInstance(scannerDeviceInfo);
            scanner.Open();
            scanner.Claim(1000);
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
            scanner.DataEvent += new DataEventHandler(Scanner_DataEvent);
        }
        private static string GetDeviceId()
        {
            string deviceId = Environment.MachineName;
            return "1";
        }
        private static void Scanner_DataEvent(object sender, DataEventArgs e)
        {
            byte[] b = scanner.ScanData;
            string deviceId = GetDeviceId();
            string barcodeId = "";
            b = scanner.ScanDataLabel;
            for (int i = 0; i < b.Length; i++)
                barcodeId += (char)b[i];

            Console.WriteLine(barcodeId);
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.broadcastNotification(barcodeId, deviceId);
            //postData(barcodeId);
            scanner.DeviceEnabled = true;
            scanner.DataEventEnabled = true;
            scanner.DecodeData = true;
        }
        private async static void postData(string str)
        {
            var data = new
            {
                scandata = str
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("http://localhost:3001/broadcast/t1", content);


            //response.EnsureSuccessStatusCode();
            //string responseBody = await response.Content.ReadAsStringAsync();

            //Console.WriteLine(responseBody);

            //scanner.Release();
            //scanner.Close();
            //scanner = null;
        }

        private static void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Message sent");
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.broadcastNotification("this is my message", "stop the chat");
        }

    }

    class Startup
    {
     
        public void Configuration(IAppBuilder MyApp)
        {
            MyApp.UseCors(CorsOptions.AllowAll);
            MyApp.MapSignalR();
        }
    }

    public class ChatHub : Hub
    {
        public void LetsChat(string Cl_Name, string Cl_Message)
        {
            Clients.All.NewMessage(Cl_Name, Cl_Message);
        }
    }
}
