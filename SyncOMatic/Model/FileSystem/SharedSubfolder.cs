using SyncOMatic.Networking;
using SyncOMatic.Networking.Requests;
using SyncOMatic.Networking.Responses;
using SyncOMatic.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace SyncOMatic.Model.FileSystem
{
    public class SharedSubfolder : File
    {
        public IPAddress IpAddress { get; set; }
        public short Port { get; set; }

        public ObservableCollection<SharedSubfolder> Subfolders { get; private set; }
        public List<File> Files { get; private set; }
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

        public bool LoadFiles()
        {
            if (Files == null)
                Files = new List<File>();

            Files.Clear();

            try
            {
                string[] files = Directory.GetFiles(Path);

                foreach (var file in files)
                    Files.Add(new File(file, this));

                return true;
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.LogError(e);
                return false;
            }
        }

        public SharedSubfolder() : this(null, null) { }

        private SharedSubfolder(string path, SharedSubfolder parent) : base(path, parent)
        {
            Subfolders = new ObservableCollection<SharedSubfolder>();
            IsEmpty = true;
            remoteSubfoldersLoaded = false;

            if (Parent != null)
            {
                IpAddress = Parent.IpAddress;
                Port = Parent.Port;
            }
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
            catch (DirectoryNotFoundException e)
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
            SharedSubfoldersResponse response = (SharedSubfoldersResponse)await syncClient.SendRequestAsync(new SharedSubfoldersRequest(Path));

            Subfolders.Clear();
            foreach (var subfolder in response.Subfolders)
            {
                subfolder.IpAddress = IpAddress;
                subfolder.Port = Port;
                Subfolders.Add(subfolder);
            }

            remoteSubfoldersLoaded = true;
        }
    }
}