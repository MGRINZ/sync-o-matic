using System.Windows;

namespace SyncOMatic
{
    /// <summary>
    /// Logika interakcji dla klasy SharedFolderWindow.xaml
    /// </summary>
    public partial class SharedFolderWindow : Window
    {
        private SharedFolder sharedFolder;
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
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            sharedFolder = null;
            Close();
        }

        private void SelectDir_Click(object sender, RoutedEventArgs e)
        {
            var fbDialog = new System.Windows.Forms.FolderBrowserDialog();
            fbDialog.SelectedPath = SharedFolder.Path;
            if (fbDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                SharedFolder.Path = fbDialog.SelectedPath;
        }
    }
}
