using SyncOMatic.Requests;
using SyncOMatic.Responses;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace SyncOMatic
{
    public class SharedSubfolder : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name = "";
        private string path;
        public string Name
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
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
        public IPAddress IpAddress { get; set; }
        public short Port { get; set; }

        public ObservableCollection<SharedSubfolder> Subfolders { get; private set; }
        private bool isEmpty;
        private bool remoteSubfoldersLoaded;

        public bool IsEmpty
        {
            get => isEmpty;
            set
            {
                isEmpty = value;
                
                if (value == true)
                    Subfolders.Clear();
                else
                {
                    if (Subfolders.Count == 0)
                    {
                        var emptySubfolder = new SharedSubfolder();
                        emptySubfolder.Name = "(brak)";
                        Subfolders.Add(emptySubfolder);
                    }
                }
            }
        }

        public SharedSubfolder()
        {
            Subfolders = new ObservableCollection<SharedSubfolder>();
            IsEmpty = true;
            remoteSubfoldersLoaded = false;
        }

        private SharedSubfolder(string path, SharedSubfolder parent) : this()
        {
            Path = path;
            Name = System.IO.Path.GetFileName(path);
            Parent = parent;
            IpAddress = Parent.IpAddress;
            Port = Parent.Port;
        }

        protected void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public bool LoadLocalSubfolders()
        {
            if (Subfolders == null)
                Subfolders = new ObservableCollection<SharedSubfolder>();

            Subfolders.Clear();

            try
            {
                string[] paths = Directory.GetDirectories(Path);

                foreach (var path in paths)
                    Subfolders.Add(new SharedSubfolder(path, this));

                if (Subfolders.Count > 0)
                    IsEmpty = false;

                return true;
            }
            catch(DirectoryNotFoundException e)
            {
                Logger.LogError(e);
                return false;
            }
        }

        public async void LoadRemoteSubfolders()
        {
            if (remoteSubfoldersLoaded)
                return;

            var syncClient = new SyncClient(IpAddress, Port);
            SharedSubfoldersResponse response = (SharedSubfoldersResponse)await syncClient.SendRequestAsync(new SharedSubfoldersRequest(path));
            
            Subfolders.Clear();
            foreach(var subfolder in response.Subfolders)
            {
                subfolder.IpAddress = IpAddress;
                subfolder.Port = Port;
                Subfolders.Add(subfolder);
            }

            remoteSubfoldersLoaded = true;
        }
    }
}