using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleClient
{
    class ClientSocketProgram
    {
        static Socket client = new Socket(AddressFamily.InterNetwork,
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
                        return (ip.ToString());

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
            client.Connect(localEndPoint);
            if (client.Connected)
            {
                Console.WriteLine("Client is accepted");
                try
                {
                    while (true)
                    {
                        try
                        {
                            string response;
                            string request;
                            request = "GET ";
                            bufferOut = Encoding.ASCII.GetBytes(request);
                            client.Send(bufferOut);
                            client.Receive(bufferIn);
                            response = Encoding.ASCII.GetString(bufferIn);
                            Console.WriteLine(response);
                            Thread.Sleep(3000);
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                client.Close();
                Console.ReadKey();
            }
        }
    }
}
