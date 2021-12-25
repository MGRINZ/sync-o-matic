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
    public partial class RemoteDeviceWindow : Window
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
            sfWindow.Owner = this;
            sfWindow.ShowDialog();
            if (sfWindow.SharedFolder != null)
                RemoteDevice.SharedFolders.Add(sfWindow.SharedFolder);
        }

        private void EditSharedFolder_Click(object sender, RoutedEventArgs e)
        {
            var sfWindow = new SharedFolderWindow();
            sfWindow.Owner = this;

            if (sharedFoldersListView.SelectedItem == null)
                return;

            sfWindow.SharedFolder = sharedFoldersListView.SelectedItem as SharedFolder;
            sfWindow.ShowDialog();

            if (sfWindow.SharedFolder != null)
                RemoteDevice.SharedFolders[sharedFoldersListView.SelectedIndex] = sfWindow.SharedFolder;
        }

        private void DeleteSharedFolder_Click(object sender, RoutedEventArgs e)
        {
            if (sharedFoldersListView.SelectedItem != null)
                RemoteDevice.SharedFolders.Remove(sharedFoldersListView.SelectedItem as SharedFolder);
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
