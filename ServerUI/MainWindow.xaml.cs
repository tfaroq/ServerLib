using System;
using System.Threading.Tasks;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using ComLib;
using System.Windows.Threading;
using System.Threading;

namespace ServerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpListnerEx2 server = new TcpListnerEx2();
        public static bool running = true;
        public MainWindow()
        {
            InitializeComponent();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    tbIPAddress.Text = ip.ToString();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            string result = string.Empty;
           
            if (server.StartServer(tbIPAddress.Text, tbPort.Text, out result))
            {
                Task t = new Task(GetDataAndUpdateConsolOutputBox);
                t.Start();
                lStatus.Content = "Listening";
                btnStartServer.IsEnabled = false;
            }
            else
            {
                tbConsoleOutPut.Text = result;
            }
        }

        private void GetDataAndUpdateConsolOutputBox()
        {
            while(running)
            {
                server.GetAutoResetEvent().WaitOne();
                string str = server.GetData();
                int size = str.Length;
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
             
                    tbConsoleOutPut.Text = DateTime.Now.ToString("h:mm:ss tt") + " Client: " + str + tbConsoleOutPut.Text;
                }));

            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            server.GetAutoResetEvent().Set();
            server.CloseOutputFile();
            running = false;
            Application.Current.Shutdown();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbPayLoad.Text)) return;
            try
            {
                tbConsoleOutPut.Text = DateTime.Now.ToString("h:mm:ss tt") + " Server: " + tbPayLoad.Text +"\n" + tbConsoleOutPut.Text;
                server.Send(tbPayLoad.Text);
                tbPayLoad.Text = "";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
