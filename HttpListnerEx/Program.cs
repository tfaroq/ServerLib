using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpListnerEx
{
    class Program
    {
        static void Main()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            listener.Start();
            Console.WriteLine("Listening...");
            for (;;)
            {
                HttpListenerContext ctx = listener.GetContext();
                new Thread(new Worker(ctx).ProcessRequest).Start();
            }
        }
    }
}
//static void Main(string[] args)
//        {
//            WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/");
//            ws.Run();
//            Console.WriteLine("A simple webserver. Press a key to quit.");
//            Console.ReadKey();
//            ws.Stop();
//        }
//        public static string SendResponse(HttpListenerRequest request)
//        {
//            return string.Format("<HTML><BODY>Welcome to my page.<br>{0}</BODY></HTML>", DateTime.Now);
//        }
//    }
    

