using System;

namespace SyncOMatic
{
    public class SharedFolder : SharedSubfolder, ICloneable
    {
        public bool IsActive { get; set; }
        
        private bool canRead;
        private bool canWrite;

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

        public SharedFolder()
        {
            IsActive = false;
            CanRead = true;
            CanWrite = true;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}