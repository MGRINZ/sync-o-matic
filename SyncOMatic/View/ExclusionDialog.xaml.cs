using SyncOMatic.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    
    public partial class ExclusionDialog : Window, IAddEditDelete
    {
        private bool save = false;

        private FileExclusion fileExclusion;
        public FileExclusion FileExclusion
        {
            get => fileExclusion;
            set
            {
                fileExclusion = (FileExclusion)value.Clone();
                this.DataContext = fileExclusion;
            }
        }

        public ExclusionDialog()
        {
            InitializeComponent();
            FileExclusion = new FileExclusion("");
        }

        public object GetItem()
        {
            return FileExclusion;
        }

        public int SelectedIndex { get; set; }

        public void SetItem(object item)
        {
            FileExclusion = item as FileExclusion;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (FileExclusion.Pattern.Trim().Length == 0)
                return;

            save = true;

            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!save)
                fileExclusion = null;
        }
    }
}
