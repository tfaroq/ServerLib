using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComLib;
using System.Windows.Threading;
using System.Threading;

namespace ClientUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClientEx client = new TcpClientEx();
        public static bool running = true;
        public MainWindow()
        {
            
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tbServerIP.Text) || string.IsNullOrEmpty(tbServerPort.Text))
                {
                    return;
                }
                client.Connect(tbServerIP.Text, tbServerPort.Text);
                Task t = new Task(GetDataAndUpdateConsolOutputBox);
                t.Start();
                lbStatus.Content = "Connected";
                btnConnect.IsEnabled = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
            
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrEmpty(tbPayLoad.Text)) return;
            try
            {
                tbConsole.Text = DateTime.Now.ToString("h:mm:ss tt") + " Client: " + tbPayLoad.Text + "\n" + tbConsole.Text;
                client.Send(tbPayLoad.Text);
                tbPayLoad.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetDataAndUpdateConsolOutputBox()
        {
            while (running)
            {
                client.GetAutoResetEvent().WaitOne();
                string str = client.GetData();
                int size = str.Length;
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                    tbConsole.Text = DateTime.Now.ToString("h:mm:ss tt") + " Server: " + str + tbConsole.Text;
                }));

            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            client.GetAutoResetEvent().Set();
            client.CloseOutputFile();
            running = false;
        }
    }
}
