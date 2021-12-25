using System.ComponentModel;
using System.IO;
using System.Windows;

namespace SyncOMatic
{
    /// <summary>
    /// Logika interakcji dla klasy SharedFolderWindow.xaml
    /// </summary>
    public partial class SharedFolderWindow : Window, IAddEditDelete
    {
        private SharedFolder sharedFolder;
        private bool save;

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
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(SharedFolder.Path))
                return;

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
    }
}
