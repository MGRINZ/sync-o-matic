using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SyncOMatic
{
    public partial class SyncRule : INotifyPropertyChanged, ICloneable
    {
        public bool IsActive { get; set; }

        private string localDir;
        public string LocalDir
        {
            get => localDir;
            set
            {
                if (value != localDir)
                {
                    localDir = value;
                    NotifyPropertyChanged("LocalDir");
                }
            }
        }

        private string remoteDir;
        public string RemoteDir
        {
            get => remoteDir;
            set
            {
                if (value != remoteDir)
                {
                    remoteDir = value;
                    NotifyPropertyChanged("RemoteDir");
                }
            }
        }

        private SyncMethod syncMethod;
        public SyncMethod SyncMethod
        {
            get => syncMethod;
            set
            {
                if (value != syncMethod)
                {
                    syncMethod = value;
                    NotifyPropertyChanged("SyncMethod");
                }
            }
        }
        public ObservableCollection<FileExclusion> FileExclusions { get; set; }

        public SyncRule()
        {
            SyncMethod = SyncMethod.ReadWrite;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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