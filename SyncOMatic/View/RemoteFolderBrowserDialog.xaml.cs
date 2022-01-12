using SyncOMatic.Model.FileSystem;
using SyncOMatic.Networking;
using SyncOMatic.Networking.Requests;
using SyncOMatic.Networking.Responses;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SyncOMatic.View
{
    /// <summary>
    /// Logika interakcji dla klasy RemoteFolderBrowserDialog.xaml
    /// </summary>
    public partial class RemoteFolderBrowserDialog : Window
    {
        public ObservableCollection<SharedFolder> SharedFolders { get; private set; }

        public RemoteFolderBrowserDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public RemoteFolderBrowserDialog(IPAddress ipAddress, short port) : this()
        {
            GetRemoteFolders(ipAddress, port);
        }

        private async void GetRemoteFolders(IPAddress ipAddress, short port)
        {
            var syncClient = new SyncClient(ipAddress, port);
            SharedFoldersResponse response = (SharedFoldersResponse)await syncClient.SendRequestAsync(new SharedFoldersRequest());
            SharedFolders = new ObservableCollection<SharedFolder>(response.SharedFolders);
            foreach(var folder in SharedFolders)
            {
                folder.IpAddress = ipAddress;
                folder.Port = port;
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var remoteFolder = (SharedSubfolder)remoteFoldersTree.SelectedItem;

            if (remoteFolder == null)
                return;

            (Owner as SyncDetailsWindow).SyncRule.RemoteDir = remoteFolder.Path;

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            ((e.OriginalSource as TreeViewItem).DataContext as SharedSubfolder).LoadRemoteSubfolders();
        }
    }
}
