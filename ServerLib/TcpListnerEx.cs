using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace ComLib
{
    public class TcpListnerEx
    {
        TcpListener tcpListner;
        string str = string.Empty;
        StreamWriter outFile = new StreamWriter(@"serverlog.txt", true);
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        public string GetData() {
            string tmp = str;
            str = "";
            return tmp;
        }
        public AutoResetEvent GetAutoResetEvent() { return autoResetEvent; }
        public AutoResetEvent GetClientConnectedDisconnectedEvent() { return clientConnectedDisconnectedEvent; }
        static List<TcpClient> clients = new List<TcpClient>();
        public List<TcpClient> GetTcpClients()
        {
            return clients;
        }
        byte[] rx;
        private AutoResetEvent clientConnectedDisconnectedEvent = new AutoResetEvent(false);

        /// <summary>
        /// Start server
        /// </summary>
        /// <param name="ipAddr"></param>
        /// <param name="port"></param>
        public void StartServer(IPAddress ipAddr, int port)
        {
            try
            {
                tcpListner = new TcpListener(ipAddr, port);
                tcpListner.Start();
                outFile.WriteLine("Server Started at: " + DateTime.Now.ToString("h:mm:ss tt"));
                outFile.Flush();
                tcpListner.BeginAcceptTcpClient(onCompleteAcceptTcpClientCallBack, tcpListner);
            }
            catch(System.Net.Sockets.SocketException ex)
            {
                outFile.WriteLine(ex.Message);
                outFile.Flush();
                throw new Exception(ex.Message);
            }
            catch(System.ArgumentOutOfRangeException ex)
            {
                outFile.WriteLine(ex.Message);
                outFile.Flush();
                throw new Exception(ex.Message);
            }
            catch (System.InvalidOperationException ex)
            {
                outFile.WriteLine(ex.Message);
                outFile.Flush();
                throw new Exception(ex.Message);
            }
            

        }
        public static List<string> GetAllIpAddresses()
        {
            List<string> str = new List<string>();
            string hostName = Dns.GetHostName();
            string strIP = string.Empty;
            IPHostEntry HostEntry = Dns.GetHostEntry(hostName);
            if (HostEntry.AddressList.Length > 0)
            {
                foreach (IPAddress ip in HostEntry.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        strIP = ip.ToString();
                        str.Add(strIP);
                    }
                }
            }
            return str;
        }

      

        //
        /// <summary>
        /// Callback method is called when a client has been connected
        /// </summary>
        /// <param name="iar"></param>
        void onCompleteAcceptTcpClientCallBack(IAsyncResult iar)
        {
            TcpListener tcpl = (TcpListener)iar.AsyncState;
            try
            {
                TcpClient tcpClient = tcpl.EndAcceptTcpClient(iar);
                clients.Add(tcpClient);
                clientConnectedDisconnectedEvent.Set();
                // 
                //outFile.WriteLine(tcpClient.Client.RemoteEndPoint.ToString());
                //outFile.Flush();
                rx = new byte[1];
                tcpClient.GetStream().BeginRead(rx, 0, rx.Length, OnComplateReadFromTCPClientStreamCallBack, tcpClient);
            }
            catch(Exception exc)
            {
               
                outFile.WriteLine(exc.Message);
                outFile.Flush();
            }
        }
        /// <summary>
        /// Called when data is recieved
        /// </summary>
        /// <param name="iar"></param>
        void OnComplateReadFromTCPClientStreamCallBack(IAsyncResult iar)
        {
            TcpClient tcpc;
            int countReadBytes = 0;
            string strRecv;
            tcpc = (TcpClient)iar.AsyncState;
            try
            {
                countReadBytes = tcpc.GetStream().EndRead(iar);
                if(countReadBytes == 0)
                {
                    //outFile.WriteLine("Client disconnected");
                    //outFile.Flush();
                    Debug.WriteLine("Client disconnected");
                    clients.Remove(tcpc);
                    autoResetEvent.Set();
                    return;
                }

                strRecv = Encoding.ASCII.GetString(rx, 0, rx.Length);
                str = strRecv + Environment.NewLine;
                autoResetEvent.Set();
                rx = new byte[1];
                tcpc.GetStream().BeginRead(rx, 0, rx.Length, OnComplateReadFromTCPClientStreamCallBack, tcpc);
            }
            catch (Exception exc)
            {
                outFile.WriteLine(exc.Message);
                outFile.Flush();
                rx = new byte[1];
                tcpc.GetStream().BeginRead(rx, 0, rx.Length, OnComplateReadFromTCPClientStreamCallBack, tcpc);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            try
            {
                foreach (TcpClient client in clients)
                {
                    if (client != null)
                    {
                        if (client.Client.Connected)
                        {
                            client.GetStream().BeginWrite(data, 0, data.Length, onCompleteWriteToClientStream, client);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("No client connected");
                    }
                }
            }
            catch (Exception exc)
            {
                outFile.WriteLine(exc.Message);
                outFile.Flush();
                throw new Exception(exc.Message);
            }
        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iar"></param>
        void onCompleteWriteToClientStream(IAsyncResult iar)
        {

            try
            {
                TcpClient tcpc = (TcpClient)iar.AsyncState;
                tcpc.GetStream().EndWrite(iar);

            }
            catch (Exception exc)
            {
                outFile.WriteLine(exc.Message);
                outFile.Flush();
                Debug.WriteLine(exc.Message);
            }
        
        }

        public void CloseOutputFile()
        {
            outFile.Close();
        }
    }
}
