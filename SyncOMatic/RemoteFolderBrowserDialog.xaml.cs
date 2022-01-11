using SyncOMatic.Requests;
using SyncOMatic.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace SyncOMatic
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
