using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SyncOMatic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Hostname { get; private set; }
        public string IpAddress { get; private set; }
        public ObservableCollection<RemoteDevice> RemoteDevices { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Hostname = System.Net.Dns.GetHostName();
        }

        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            var rdWindow = new RemoteDeviceWindow();
            rdWindow.Owner = this;
            rdWindow.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
