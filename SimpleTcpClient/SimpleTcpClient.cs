using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTcpClient
{
    class SimpleTcpClient
    {
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
        public static string EnterIpAddress()
        {
            Console.WriteLine("Enter server ip address");
            return Console.ReadLine();
        }
        static void SendMessage(TcpClient client, NetworkStream stream, string message)
        {
            // Translate the passed message into ASCII and store it as a Byte array.
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", message);

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new byte[256];

            // String to store the response ASCII representation.
            string responseData = string.Empty;

            // Read the first batch of the TcpServer response bytes.
            int bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);

         
        }
        static void Main(string[] args)
        {
            try
            {
                // Create a TcpClient.
                int port = 23000;
                TcpClient client = new TcpClient(EnterIpAddress(), port);
                if (client.Connected)
                    Console.WriteLine("Client is connected");
                NetworkStream stream = client.GetStream();
                try
                {
                    while (true)
                    {
                        Console.WriteLine("Write something to the server");
                        string message = Console.ReadLine();
                        SendMessage(client, stream, message);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                // Close all
                client.Close();
                stream.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
    }
}
