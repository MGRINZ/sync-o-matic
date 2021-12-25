using System;
using System.ComponentModel;

namespace SyncOMatic
{
    public class SharedFolder : INotifyPropertyChanged, ICloneable
    {
        public bool IsActive { get; set; }
        private string path;
        private bool canRead;
        private bool canWrite;
        public string Path
        {
            get => path;
            set
            {
                if (value != path)
                {
                    path = value;
                    NotifyPropertyChanged("Path");
                }
            }
        }
        public bool CanRead
        {
            get => canRead;
            set
            {
                if (value != canRead)
                {
                    canRead = value;
                    NotifyPropertyChanged("CanRead");
                }
            } 
        }
        public bool CanWrite
        {
            get => canWrite;
            set
            {
                if (value != canWrite)
                {
                    canWrite = value;
                    NotifyPropertyChanged("CanWrite");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SharedFolder()
        {
            IsActive = false;
            CanRead = true;
            CanWrite = true;
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