using SyncOMatic.Model;
using SyncOMatic.Networking;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace SyncOMatic.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AddEditDeleteWindow
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

        public MainWindow(ObservableCollection<RemoteDevice> remoteDevices) : this()
        {
            RemoteDevices = remoteDevices;
        }

        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            var rdWindow = new RemoteDeviceWindow();
            AddItem(rdWindow, RemoteDevices);
            if (rdWindow.RemoteDevice != null)
            {
                Task updateTask = rdWindow.RemoteDevice.UpdateHostnameAsync();
            }
        }

        private void EditDevice_Click(object sender, RoutedEventArgs e)
        {
            var rdWindow = new RemoteDeviceWindow();
            EditItem(rdWindow, remoteDevicesListView, RemoteDevices);
            if (rdWindow.RemoteDevice != null)
            {
                Task updateTask = rdWindow.RemoteDevice.UpdateHostnameAsync();
            }
        }

        private void DeleteDevice_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem(remoteDevicesListView, RemoteDevices);
        }

        private void UpdateHostnames()
        {
            foreach (RemoteDevice remoteDevice in RemoteDevices)
            {
                Task updateTask = remoteDevice.UpdateHostnameAsync();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Sync_Click(object sender, RoutedEventArgs e)
        {
            SyncCoordinator.GetInstance().SyncAll();
        }
    }
}
