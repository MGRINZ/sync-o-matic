using SyncOMatic.Model;
using SyncOMatic.Networking;
using SyncOMatic.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SyncOMatic
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string AppName = "SyncOMatic";
        public static ObservableCollection<RemoteDevice> RemoteDevices { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RemoteDevices = new ObservableCollection<RemoteDevice>();

            SyncServer.GetInstance().Start();

            var mWindow = new MainWindow(RemoteDevices);
            mWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
