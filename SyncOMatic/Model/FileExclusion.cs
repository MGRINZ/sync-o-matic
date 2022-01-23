using SyncOMatic.Model.FileSystem;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace SyncOMatic.Model
{
    public class FileExclusion : INotifyPropertyChanged, ICloneable
    {
        private string pattern;

        public FileExclusion(string pattern)
        {
            this.pattern = pattern;
        }

        public string Pattern
        {
            get => pattern;
            set
            {
                if (value != pattern)
                {
                    pattern = value;
                    NotifyPropertyChanged("SyncMethod");
                }
            }
        }

        public bool MatchFile(File file)
        {
            string pattern = Regex.Escape("\\") + Regex.Escape(Pattern).Replace("?", ".").Replace("\\*", ".*?") + "$";
            if (Regex.IsMatch(file.Path, pattern))
                return true;
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return pattern;
        }
    }
}