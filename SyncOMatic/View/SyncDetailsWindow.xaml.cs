using SyncOMatic.Model;
using SyncOMatic.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace SyncOMatic.View
{
    /// <summary>
    /// Logika interakcji dla klasy SyncDetailsWindow.xaml
    /// </summary>
    public partial class SyncDetailsWindow : AddEditDeleteWindow, IAddEditDelete
    {
        private IPAddress ipAddress;
        private short port;

        private bool save = false;

        public int SelectedIndex { get; set; }

        private SyncRule syncRule;
        public SyncRule SyncRule
        {
            get => syncRule;
            set
            {
                syncRule = (SyncRule)value.Clone();
                this.DataContext = syncRule;
            }
        }

        public SyncDetailsWindow()
        {
            InitializeComponent();
            SyncRule = new SyncRule();
        }

        public SyncDetailsWindow(IPAddress ipAddress, short port) : this()
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        private void AddExclusion_Click(object sender, RoutedEventArgs e)
        {
            var eDialog = new ExclusionDialog();
            AddItem(eDialog, SyncRule.FileExclusions);
        }

        private void EditExclusion_Click(object sender, RoutedEventArgs e)
        {
            var eDialog = new ExclusionDialog();
            eDialog.Title = "Edytuj wykluczenie";
            EditItem(eDialog, exclusionsListView, SyncRule.FileExclusions);
        }

        private void DeleteExclusion_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem(exclusionsListView, SyncRule.FileExclusions);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (SyncRule.LocalDir == null || SyncRule.LocalDir.Trim().Length == 0)
                return;

            if (!Directory.Exists(SyncRule.LocalDir))
                return;

            if (SyncRule.RemoteDir == null || SyncRule.RemoteDir.Trim().Length == 0)
                return;

            save = true;

            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!save)
                syncRule = null;
        }

        private void SelectRemoteDir_Click(object sender, RoutedEventArgs e)
        {
            var rfbDialog = new RemoteFolderBrowserDialog(ipAddress, port);
            rfbDialog.Owner = this;
            rfbDialog.ShowDialog();
        }

        private void SelectLocalDir_Click(object sender, RoutedEventArgs e)
        {
            var fbDialog = new System.Windows.Forms.FolderBrowserDialog();
            fbDialog.SelectedPath = SyncRule.LocalDir;
            if (fbDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SyncRule.LocalDir = fbDialog.SelectedPath;
        }

        public object GetItem()
        {
            return SyncRule;
        }

        public void SetItem(object item)
        {
            SyncRule = item as SyncRule;
        }
    }
}
