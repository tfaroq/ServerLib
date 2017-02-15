using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ComLib
{
    public class ListningToAllPorts
    {
        private static Socket mainSocket;                          //The socket which captures all incoming packets
        private static byte[] byteData = new byte[4096];
        private static bool bContinueCapturing = false;            //A flag to check if packets are to be captured or not
        //public static string message = string.Empty;
        public static void Start(string ipAddress)
        {

            try
            {
                if (!bContinueCapturing)
                {
                    //Start capturing the packets...



                    bContinueCapturing = true;

                    //For sniffing the socket to capture the packets has to be a raw socket, with the
                    //address family being of type internetwork, and protocol being IP
                    mainSocket = new Socket(AddressFamily.InterNetwork,
                        SocketType.Raw, ProtocolType.IP);
                    IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 0);
                    //Bind the socket to the selected IP address
                    mainSocket.Bind(localEndPoint);

                    //Set the socket  options
                    mainSocket.SetSocketOption(SocketOptionLevel.IP,            //Applies only to IP packets
                                               SocketOptionName.HeaderIncluded, //Set the include the header
                                               true);                           //option to true

                    byte[] byTrue = new byte[4] { 1, 0, 0, 0 };
                    byte[] byOut = new byte[4] { 1, 0, 0, 0 }; //Capture outgoing packets

                    //Socket.IOControl is analogous to the WSAIoctl method of Winsock 2
                    mainSocket.IOControl(IOControlCode.ReceiveAll,              //Equivalent to SIO_RCVALL constant
                                                                                //of Winsock 2
                                         byTrue,
                                         byOut);

                    //Start receiving the packets asynchronously
                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceiveCallBack), null);
                }
                else
                {
                    bContinueCapturing = false;
                    //To stop capturing the packets close the socket
                    mainSocket.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private static void OnReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int nReceived = mainSocket.EndReceive(ar);

                //Analyze the bytes received...

                ParseData(byteData, nReceived);

                if (bContinueCapturing)
                {
                    byteData = new byte[4096];

                    //Another call to BeginReceive so that we continue to receive the incoming
                    //packets
                    mainSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                        new AsyncCallback(OnReceiveCallBack), null);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void ParseData(byte[] byteData, int nReceived)
        {

            //Since all protocol packets are encapsulated in the IP datagram
            //so we start by parsing the IP header and see what protocol data
            //is being carried by it
            //IPHeader ipHeader = new IPHeader(byteData, nReceived);
            //TCPHeader tcpHeader = new TCPHeader(byteData,              //IPHeader.Data stores the data being 
            //                                                           //carried by the IP datagram
            //                                      nReceived); //Length of the data field                    
            MemoryStream memoryStream = new MemoryStream(byteData, 0, nReceived);
            BinaryReader binaryReader = new BinaryReader(memoryStream);

            //The first sixteen bits contain the source port
            ushort usSourcePort = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            Debug.WriteLine(usSourcePort.ToString());
        }

        public void CloseSocket()
        {
            if (bContinueCapturing)
            {
                mainSocket.Close();
            }
        }
    }
}
