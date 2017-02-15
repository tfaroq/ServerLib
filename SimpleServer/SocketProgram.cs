using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleServer
{
    class SocketProgram
    {
        static Socket listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        public static string GetIpAddress()
        {
            string str = string.Empty;
            string hostName = Dns.GetHostName();
            string strIP = string.Empty;
            IPHostEntry HostEntry = Dns.GetHostEntry(hostName);
            if (HostEntry.AddressList.Length > 0)
            {
                foreach (IPAddress ip in HostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return(ip.ToString());
                        
                    }
                }
            }
            return str;
        }
        static void Main(string[] args)
        {
            byte[] bufferIn = new byte[1024];
            byte[] bufferOut = new byte[1024];
            IPAddress ipAddress = IPAddress.Parse(GetIpAddress());
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 23000);
            listener.Bind(localEndPoint);
            listener.Listen(10);
            Console.WriteLine("Server is listening");
            Socket socket = listener.Accept();
            Console.WriteLine("Client is accepted");
            try
            {
                while (true)
                {
                    try
                    {
                        socket.Receive(bufferIn);
                        string request = Encoding.ASCII.GetString(bufferIn);
                        string response;
                     
                        if (request.StartsWith("GET "))
                        {
                            response = "HTTP/1.1 200 OK\n\n";
                            response += "<html><head></head><body><h1>Hello!</h1></body></html>\n\n";
                        }
                        else
                        {
                            response = "HTTP/1.1 500 Server could not interpret request.\n\n";
                        }
                        bufferOut = Encoding.ASCII.GetBytes(response);
                        socket.Send(bufferOut);
                        Console.WriteLine(response);
                        Thread.Sleep(3000);
                       
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            socket.Close();
            Console.ReadKey();
        }
    }
}
