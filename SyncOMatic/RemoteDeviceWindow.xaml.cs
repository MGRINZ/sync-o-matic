using System;
using System.Collections.Generic;
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
    public partial class RemoteDeviceWindow : AddEditDeleteWindow
    {
        public RemoteDevice RemoteDevice { get; set; }

        public RemoteDeviceWindow()
        {
            InitializeComponent();

            if (RemoteDevice == null)
                RemoteDevice = new RemoteDevice();

            this.DataContext = RemoteDevice;
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
            var sdWindow = new SyncDetailsWindow();
            sdWindow.Owner = this;
            sdWindow.ShowDialog();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
