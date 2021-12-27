using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SyncOMatic
{
    /// <summary>
    /// Logika interakcji dla klasy RemoteDeviceWindow.xaml
    /// </summary>
    public partial class RemoteDeviceWindow : AddEditDeleteWindow, IAddEditDelete
    {
        private RemoteDevice remoteDevice;
        private bool save = false;

        public RemoteDevice RemoteDevice
        {
            get => remoteDevice;
            set
            {
                remoteDevice = (RemoteDevice)value.Clone();
                this.DataContext = remoteDevice;
            }
        }

        public RemoteDeviceWindow()
        {
            InitializeComponent();
            RemoteDevice = new RemoteDevice();
        }

        private void AddSharedFolder_Click(object sender, RoutedEventArgs e)
        {
            var sfWindow = new SharedFolderWindow();
            AddItem(sfWindow, RemoteDevice.SharedFolders);
        }

        private void EditSharedFolder_Click(object sender, RoutedEventArgs e)
        {
            var sfWindow = new SharedFolderWindow();
            sfWindow.Title = "Edytuj udostępniony folder";
            EditItem(sfWindow, sharedFoldersListView, RemoteDevice.SharedFolders);
        }

        private void DeleteSharedFolder_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem(sharedFoldersListView, RemoteDevice.SharedFolders);
        }

        private void AddSyncRule_Click(object sender, RoutedEventArgs e)
        {
            var sdWindow = new SyncDetailsWindow(remoteDevice.IpAddress, remoteDevice.Port);
            sdWindow.Owner = this;
            sdWindow.ShowDialog();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            save = true;

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!save)
                remoteDevice = null;
        }

        public object GetItem()
        {
            return RemoteDevice;
        }

        public void SetItem(object item)
        {
            RemoteDevice = item as RemoteDevice;
        }
    }
}
