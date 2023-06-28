using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Microsoft.PointOfService;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Http;
using System.Web.Http.SelfHost;


namespace POSServiceDriver
{
    public partial class Service1 : ServiceBase
    {
        // IDisposable SignalR { get; set; }
        // private IHubContext hubContext;
        PosExplorer posExplorer;
        // Scanner scanner;
        private Scanner scanner;
        private Microsoft.PointOfService.DeviceSimulators.MyScannerSimulator scannerDevice;
        public Service1()
        {
            InitializeComponent();
           
        }

        public void StartSignalR()
        {
            //Controller = new MPWorkflowQueueController(App.AdminConfiguration.ConnectionString);

            //var config = QueueMessageManagerConfiguration.Current;
            //Controller.QueueName = config.QueueName;
            //Controller.WaitInterval = config.WaitInterval;
            //Controller.ThreadCount = config.ControllerThreads;

            // SignalR = WebApp.Start<SignalRStartup>("http://localhost:8001");

            //// Spin up the queue
            //Controller.StartProcessingAsync();

            //LogManager.Current.LogInfo(String.Format("QueueManager Controller Started with {0} threads.",
            //                                Controller.ThreadCount));

            //// Allow access to a global instance of this controller and service
            //// So we can access it from the stateless SignalR hub
            //Globals.Controller = Controller;
            //Globals.WindowsService = this;
        }

        public new void StopSignalR()
        {
            //LogManager.Current.LogInfo("QueueManager Controller Stopped.");

            //Controller.StopProcessing();
            //Controller.Dispose();
            // SignalR.Dispose();

            // Thread.Sleep(1500);
        }

        public Type getClass()
        {
            Assembly myLibrary = System.Reflection.Assembly
    .LoadFile(@"C:\Program Files (x86)\Microsoft Point Of Service\SDK\Samples\Simulator Service Objects\Microsoft.PointOfService.DeviceSimulators.dll");
            // get type of class Calculator from just loaded assembly
            foreach (Type type in myLibrary.GetTypes())
            {
                if (type.FullName == "Microsoft.PointOfService.DeviceSimulators.MyScannerSimulator")
                    return type;
            }
            return myLibrary.GetType("MyScannerSimulator");
        }



        protected override void OnStart(string[] args)
        {
            //var config = new HttpSelfHostConfiguration("http://localhost:8000");
            //config.Routes.MapHttpRoute(
            //    name: "API",
            //    routeTemplate: "{controller}/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //    );
            //HttpSelfHostServer server = new HttpSelfHostServer(config);
            //server.OpenAsync().Wait();


            //var url = "http://localhost:8044"; // Replace with your desired URL
            //FileHelper.write("creating signalr with url"+url);
            //SignalR = WebApp.Start(url, app =>
            //{
            //    app.MapSignalR();
            //});

            //hubContext = GlobalHost.ConnectionManager.GetHubContext<BarcodeHub>();
            //StartSignalR();
            //FileHelper.write("Calling ProcessBarcode");
            //// StartSignalR();
            //ProcessBarcode("123422");

            string url = "http://localhost:6118";
            try
            {
                WebApp.Start<Startup>(url);
                //using (WebApp.Start<Startup>(url))
                //{
                //FileHelper.write("Server started at" + url);
                //FileHelper.write("without using");
                // Console.WriteLine("The Server URL is: {0}", url);
                //Timer timer1 = new Timer
                //{
                //    Interval = 10000
                //};
                //timer1.Enabled = true;
                //timer1.Start();
                //timer1.Elapsed += Timer1_Elapsed;
                // Console.ReadLine();
                //}
                //posExplorer = new PosExplorer();
                //DeviceCollection deviceInfos = posExplorer.GetDevices((DeviceCompatibilities)Enum.Parse(typeof(DeviceCompatibilities), "OposAndCompatibilityLevel1", false));

               
                // scannerDevice.Open();
               

                PosExplorer posExplorer = new PosExplorer(); 
                var scannerDeviceInfo = posExplorer.GetDevices(DeviceType.Scanner)[0];
                //foreach (DeviceInfo deviceInfo in scannerDeviceInfo)
                //{
                //    FileHelper.write(CombineNames(deviceInfo.LogicalNames));
                //    FileHelper.write(deviceInfo.ServiceObjectName);
                //    FileHelper.write(deviceInfo.Description);

                //scanner = (Scanner)posExplorer.CreateInstance(deviceInfo);
                //}
                FileHelper.write(CombineNames(scannerDeviceInfo.LogicalNames));
                FileHelper.write(scannerDeviceInfo.ServiceObjectName);
                FileHelper.write(scannerDeviceInfo.Description);
                scanner = (Scanner)posExplorer.CreateInstance(scannerDeviceInfo);
                scanner.Open();
                scanner.Claim(1000);
                scanner.DeviceEnabled = true;
                scanner.DataEventEnabled = true;
                scanner.DecodeData = true;
                scanner.DataEvent += new DataEventHandler(Scanner_DataEvent);
                FileHelper.write("Before object");
                //Type t = getClass()

                //              Assembly myLibrary = System.Reflection.Assembly
                //.LoadFile(@"C:\Program Files (x86)\Microsoft Point Of Service\SDK\Samples\Simulator Service Objects\Microsoft.PointOfService.DeviceSimulators.dll");
                //              // get type of class Calculator from just loaded assembly
                //              foreach (Type type in myLibrary.GetTypes())
                //              {
                //                  if (scanner != null && type != null && type.FullName == "Microsoft.PointOfService.DeviceSimulators.MyScannerSimulator")
                //                  {
                //                      var scannerDevice1 = Cast(scanner , (type.GetType()));
                //                      FileHelper.write("After object");
                //                      getClass().InvokeMember("InputDataEvent",
                //           BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public,
                //           null, scannerDevice1, new object[] { "123566", BarCodeSymbology.Unknown });
                //                      FileHelper.write("After method call");
                //                  }
                //              }

                // var scannerDevice1 = scanner as getClass(); //Activator.CreateInstance(getClass());

                //var scannerDevice = (t)scanner;






                scanner.InputDataEvent("123566",BarCodeSymbology.Unknown);
                //scannerDevice.InputDataEvent("1235663", BarCodeSymbology.Unknown);
                //scannerDevice.InputDataEvent("12356637", BarCodeSymbology.Unknown);

                //DeviceInfo ObjDevicesInfo = posExplorer.GetDevice(DeviceType.Scanner, "Microsoft Scanner Simulator");
                //try
                //{
                //    scanner = (Scanner)getPosCommon(ObjDevicesInfo);
                //    FileHelper.write(ObjDevicesInfo.Type);
                //    // Create device instance and open it
                //    //DeviceBase device = CreateDeviceInstance(deviceInfo);
                //    //device.Open();

                //    scanner.Open();
                //}
                //catch (Exception e)
                //{
                //    FileHelper.write("New - " + e.Message);
                //}

                //foreach (DeviceInfo deviceInfo in deviceInfos)
                //{

                //    // Handle supported device types
                //    if (deviceInfo.Type == DeviceType.Scanner)
                //    {
                //        string name = GetDeviceDisplayName(deviceInfo);
                //        FileHelper.write("Printing Device Info Start");
                //        FileHelper.write(name);
                //        FileHelper.write(CombineNames(deviceInfo.LogicalNames));
                //        FileHelper.write(deviceInfo.ServiceObjectName);
                //        FileHelper.write("Printing Device Info End");
                //        if (name == "MyScanner")
                //        {
                //            // scanner = (Scanner)getPosCommon(deviceInfo);
                //            //FileHelper.write(deviceInfo.Type);

                //            // Create device instance and open it

                //            scannerDevice = CreateDeviceInstance(deviceInfo);
                //            scanner = (Scanner)scannerDevice;
                //            scannerDevice.Open();

                //            scanner.Open();
                //            scanner.Claim(1000);
                //            scanner.DeviceEnabled = true;
                //            scanner.DataEventEnabled = true;
                //            scanner.DecodeData = true;

                //            // scanner.ErrorEvent += Scanner_ErrorEvent;
                //            scanner.DataEvent += new DataEventHandler(Scanner_DataEvent);


                //            //scannerDevice.Claim(1000);
                //            //scannerDevice.DeviceEnabled = true;
                //            //scannerDevice.DataEventEnabled = true;
                //            //scannerDevice.DecodeData = true;

                //            //scannerDevice.ErrorEvent += Scanner_ErrorEvent;
                //            //scannerDevice.DataEvent += Scanner_DataEvent;

                //            //Timer timer1 = new Timer
                //            //{
                //            //    Interval = 10000
                //            //};
                //            //timer1.Enabled = true;
                //            //timer1.Start();
                //            //timer1.Elapsed += Timer1_Elapsed;



                //            FileHelper.write("scanner claimed - " + scanner.Claimed);
                //            FileHelper.write("scanner health - " + scanner.CheckHealthText);
                //            FileHelper.write("scanner data count - " + scanner.DataCount);
                //            FileHelper.write("scanner state - " + scanner.State);
                //            //FileHelper.write("scannerdevice data count - " + scannerDevice.DataCount);
                //            //FileHelper.write("scannerdevice claimed - " + scannerDevice.Claimed);
                //            scannerDevice.InputDataEvent("123457897", BarCodeSymbology.Unknown);
                //        }
                //        //scanner.Claim(1000);
                //        //scanner.DeviceEnabled = true;
                //        //scanner.DataEventEnabled = true;
                //        //scanner.DataEvent += Scanner_DataEvent;


                //        //// Add the device to the dictionary of devices
                //        //devices.Add(deviceInfo.ServiceObjectName, device);
                //    }
                //}
            }
            catch (Exception e)
            {
                FileHelper.write(e.Message);
            }


        }

        public object Cast(object obj, Type t)
        {
            try
            {
                var param = Expression.Parameter(obj.GetType());
                return Expression.Lambda(Expression.Convert(param, t), param)
                     .Compile().DynamicInvoke(obj);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }


        private void Scanner_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            FileHelper.write("error data event - " + e.ErrorCode);
        }

        private void Scanner_DataEvent(object sender, DataEventArgs e)
        {
            FileHelper.write("In data event");
            byte[] b = scanner.ScanData;


            string str = "Raw Data: ";
            for (int i = 0; i < b.Length; i++)
                str += (b[i].ToString(System.Globalization.CultureInfo.CurrentCulture) + " ");
            str += "\r\n";

            str += "Formatted Data: ";
            b = scanner.ScanDataLabel;
            for (int i = 0; i < b.Length; i++)
                str += (char)b[i];
            str += "\r\n";

            str += "Symbology: " + scanner.ScanDataType + "\r\n";
            str += "\r\n";


            FileHelper.write(str);
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //context.Clients.All.broadcastNotification(str, "stop the chat");
            //string barcodeData = scanner.ScanData;
            //ProcessBarcodeData(scanner.ServiceObjectName, barcodeData);
        }
        //void Scanner_DataEvent(object sender, DataEventArgs e)
        //{

        //    byte[] b = scanner.ScanData;


        //    string str = "Raw Data: ";
        //    for (int i = 0; i < b.Length; i++)
        //        str += (b[i].ToString(System.Globalization.CultureInfo.CurrentCulture) + " ");
        //    str += "\r\n";

        //    str += "Formatted Data: ";
        //    b = scanner.ScanDataLabel;
        //    for (int i = 0; i < b.Length; i++)
        //        str += (char)b[i];
        //    str += "\r\n";

        //    str += "Symbology: " + scanner.ScanDataType + "\r\n";
        //    str += "\r\n";

        //    FileHelper.write(str);
        //}
        private string CombineNames(string[] names)
        {
            string s = "";
            foreach (string name in names)
            {
                if (s.Length > 0)
                    s += ';';
                s += name;
            }

            return s;
        }
        private string GetDeviceDisplayName(DeviceInfo device)
        {
            // FileHelper.write("GetDeviceDisplayName");
            string name = CombineNames(device.LogicalNames);
            if (name.Length == 0)
            {
                name = device.ServiceObjectName;
                if (name.Length == 0)
                    name = device.Description;
            }
            return name;
        }
        private PosCommon getPosCommon(DeviceInfo deviceInfo)
        {
            switch (deviceInfo.Type)
            {
                case DeviceType.Scanner:
                    return (Scanner)posExplorer.CreateInstance(deviceInfo);

                //case DeviceType.PosPrinter:
                //    return new PrinterDevice((PosPrinter)posExplorer.CreateInstance(deviceInfo));

                //case DeviceType.Scale:
                //    return new WeightScaleDevice((Scale)posExplorer.CreateInstance(deviceInfo));

                default:
                    throw new NotSupportedException($"Device type {deviceInfo.Type} is not supported.");
            }
        }
        private MyScannerSimulator CreateDeviceInstance(DeviceInfo deviceInfo)
        {
            switch (deviceInfo.Type)
            {
                case DeviceType.Scanner:
                    
                    //scanner.Open();
                    //FileHelper.write("scanner name - "+scanner.DeviceName);
                    /// var device = new ScannerDevice();
                    // scanner = (Scanner)posExplorer.CreateInstance(deviceInfo);
                    //try
                    //{
                    //    var _device = posExplorer.GetDevice(DeviceType.Scanner, "MyScanner");
                    //    FileHelper.write("_device - " + _device.ServiceObjectName);
                    //    FileHelper.write("_device - " + _device.LogicalNames);
                    //    var s= (Scanner)posExplorer.CreateInstance(_device);
                    //    var t = (ScannerSimulator)s;
                    //    if (t != null)
                    //    {
                    //        FileHelper.write("cast successful - ");
                    //    }
                    //}catch(Exception e)
                    //{
                    //    FileHelper.write("cast exception - " + e.Message);
                    //}
                    return new MyScannerSimulator();

                //case DeviceType.PosPrinter:
                //    return new PrinterDevice((PosPrinter)posExplorer.CreateInstance(deviceInfo));

                //case DeviceType.Scale:
                //    return new WeightScaleDevice((Scale)posExplorer.CreateInstance(deviceInfo));

                default:
                    throw new NotSupportedException($"Device type {deviceInfo.Type} is not supported.");
            }
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            scannerDevice.InputDataEvent("Sending data - " + DateTime.Now.ToString(), BarCodeSymbology.Unknown);
            FileHelper.write("Timer Elapsed" + DateTime.Now.ToString());
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //context.Clients.All.broadcastNotification("this is my message", "stop the chat");
        }

        protected override void OnStop()
        {
            //StopSignalR();
            // SignalR.Dispose();
        }

        //public void ProcessBarcode(string barcode)
        //{
        //    // Process the barcode data
        //    //FileHelper.write("Inside ProcessBarcode" + barcode);

        //    //// Push the barcode data to connected clients
        //    //hubContext.Clients.All.ReceiveBarcode(barcode);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);

        //    if (SignalR != null)
        //    {
        //        SignalR.Dispose();
        //        SignalR = null;
        //    }
        //}
    }

    public class ChatHub : Hub
    {
        public void LetsChat(string Cl_Name, string Cl_Message)
        {
            Clients.All.NewMessage(Cl_Name, Cl_Message);
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder MyApp)
        {
            MyApp.Map("/signalr", map =>
            {
                MyApp.UseCors(CorsOptions.AllowAll);
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                //map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    EnableJSONP = true,
                    EnableJavaScriptProxies = true,
                    EnableDetailedErrors = true,
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
           
            MyApp.MapSignalR();
        }
    }
}
