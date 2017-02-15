using ComLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace ListningToAllPortsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string strIP = null;

            IPHostEntry HosyEntry = Dns.GetHostEntry((Dns.GetHostName()));
            if (HosyEntry.AddressList.Length > 0)
            {
                foreach (IPAddress ip in HosyEntry.AddressList)
                {
                    strIP = ip.ToString();
                    cbIPAddresses.Items.Add(strIP);
                }
            }
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            string strIPAddress = (string)cbIPAddresses.SelectedItem;
            if (strIPAddress.Equals(""))
            {
                MessageBox.Show("Select an Interface to capture the packets.", "Listning to all ports",
                    MessageBoxButton.YesNo, MessageBoxImage.Error);
                return;
            }
            else
            {
                Task.Run(() => ListningToAllPorts.Start(strIPAddress));
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
