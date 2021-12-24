using System;
using System.Collections.Generic;
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

namespace SyncOMatic
{
    /// <summary>
    /// Logika interakcji dla klasy SyncDetailsWindow.xaml
    /// </summary>
    public partial class SyncDetailsWindow : Window
    {
        public SyncDetailsWindow()
        {
            InitializeComponent();
        }

        private void AddExclusion_Click(object sender, RoutedEventArgs e)
        {
            var eDialog = new ExclusionDialog();
            eDialog.Owner = this;
            eDialog.ShowDialog();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
