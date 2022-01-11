using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace SyncOMatic
{
    public class RemoteDevice : INotifyPropertyChanged, ICloneable
    {
        private string hostname;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Hostname
        {
            get => hostname;
            private set
            {
                if (value != hostname)
                {
                    hostname = value;
                    NotifyPropertyChanged("Hostname");
                }
            }
        }
        public IPAddress IpAddress { get; set; }
        public short Port { get; set; }
        public DateTime LastSync { get; set; }
        public ObservableCollection<SharedFolder> SharedFolders { get; private set; }
        public ObservableCollection<SyncRule> SyncRules { get; private set; }

        public RemoteDevice()
        {
            // Some defaults
            IsActive = false;
            Name = "Komputer";
            IpAddress = IPAddress.Parse("127.0.0.1");
            Port = 10000;
            SharedFolders = new ObservableCollection<SharedFolder>();
            SyncRules = new ObservableCollection<SyncRule>();
        }

        public async Task UpdateHostnameAsync()
        {
            try
            {
                Hostname = (await System.Net.Dns.GetHostEntryAsync(IpAddress)).HostName;
            }
            catch
            {
                Hostname = "-";
            }
        }

        private void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}