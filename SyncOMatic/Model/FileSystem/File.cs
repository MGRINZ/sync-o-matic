using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic.Model.FileSystem
{
    public class File : INotifyPropertyChanged
    {

        protected string name = "";
        private string path;
        public string Name
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value != null ? value : "";
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Path
        {
            get => path;
            set
            {
                if (value != path)
                {
                    path = value;
                    NotifyPropertyChanged("Path");

                    if (name.Length == 0)
                        Name = System.IO.Path.GetFileName(path);
                }
            }
        }
        public SharedSubfolder Parent { get; private set; }
        public DateTime ModifyTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public File() { }

        public File(string fullPath, SharedSubfolder parent)
        {
            Path = fullPath;
            Name = System.IO.Path.GetFileName(fullPath);
            Parent = parent;

            if (Path != null)
                ModifyTime = System.IO.File.GetLastWriteTime(Path);
        }
    }
}
