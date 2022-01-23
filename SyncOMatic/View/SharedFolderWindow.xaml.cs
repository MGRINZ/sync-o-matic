using SyncOMatic.Model.FileSystem;
using SyncOMatic.View;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace SyncOMatic.View
{
    public partial class SharedFolderWindow : Window, IAddEditDelete
    {
        public int SelectedIndex { get; set; }

        private SharedFolder sharedFolder;
        private bool save = false;

        public SharedFolder SharedFolder
        {
            get => sharedFolder;
            set
            {
                sharedFolder = (SharedFolder)value.Clone();
                this.DataContext = sharedFolder;
            }
        }

        public SharedFolderWindow()
        {
            InitializeComponent();
            SharedFolder = new SharedFolder();
            SelectedIndex = -1;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(SharedFolder.Path))
                return;

            if (SharedFolder.Name.Trim().Length == 0)
                return;

            RemoteDeviceWindow rdWindow = (RemoteDeviceWindow)this.Owner;

            foreach (var folder in rdWindow.RemoteDevice.SharedFolders)
            {
                if (SelectedIndex != -1 && rdWindow.RemoteDevice.SharedFolders.IndexOf(folder) == SelectedIndex)
                    continue;

                if (folder.Path == SharedFolder.Path)
                {
                    MessageBox.Show(this, "Katalog o podanej ścieżce już istnieje. Wprowadź inną ścieżkę.", "Ścieżka istnieje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (folder.Name == SharedFolder.Name)
                {
                    MessageBox.Show(this, "Katalog o podanej nazwie już istnieje. Wprowadź inną nazwę.", "Folder istnieje", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            save = true;

            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if(!save)
                sharedFolder = null;
        }

        private void SelectDir_Click(object sender, RoutedEventArgs e)
        {
            var fbDialog = new System.Windows.Forms.FolderBrowserDialog();
            fbDialog.SelectedPath = SharedFolder.Path;
            if (fbDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SharedFolder.Path = fbDialog.SelectedPath;

        }
        public object GetItem()
        {
            return SharedFolder;
        }

        public void SetItem(object item)
        {
            SharedFolder = item as SharedFolder;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MinHeight = this.Height;
            this.MaxHeight = this.Height;
        }
    }
}
