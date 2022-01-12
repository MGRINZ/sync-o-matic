using SyncOMatic.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using File = SyncOMatic.Model.FileSystem.File;

namespace SyncOMatic.Networking.Responses
{
    public class FilesListResponse : IResponse
    {
        public IList<SharedFolder> sharedFolders;
        private SharedSubfolder localSubfolder;

        public IList<File> RemoteFiles { get; private set; }

        private IList<File> localFiles;
        private IEnumerator<File> localFilesEnum;
        private string remotePath;

        public FilesListResponse()
        {
            RemoteFiles = new List<File>();
        }

        public FilesListResponse(IPAddress clientIp)
        {
            foreach (var device in App.RemoteDevices)
            {
                if (!device.IpAddress.Equals(clientIp))
                    continue;

                sharedFolders = device.SharedFolders;
                break;
            }
        }

        public void ParseRequestData(byte[] data)
        {
            remotePath = Encoding.UTF8.GetString(data);

            string[] remotePathTree = remotePath.Split("/");
            string remoteRoot = remotePathTree[1];  // index 0 is empty
            string relativePath = Path.Combine(remotePathTree.Skip(2).ToArray());
            foreach (var folder in sharedFolders)
            {
                if (folder.Name == remoteRoot)
                {
                    string localPath = Path.Combine(folder.Path, relativePath);
                    localSubfolder = new SharedSubfolder();
                    localSubfolder.Path = localPath;

                    localFiles = LoadFiles(localSubfolder);
                    localFilesEnum = localFiles.GetEnumerator();
                    break;
                }
            }
        }

        public List<File> LoadFiles(SharedSubfolder folder)
        {
            List<File> files = new List<File>();
            
            folder.LoadFiles();
            files.AddRange(folder.Files);

            folder.LoadLocalSubfolders();
            foreach (var subfolder in folder.Subfolders)
                files.AddRange(LoadFiles(subfolder));

            return files;
        }

        public byte[] GetDataToSend()
        {
            List<byte> data = new List<byte>();

            if (localFilesEnum == null)
                return new byte[0];

            if (localFilesEnum.MoveNext())
            {
                File file = localFilesEnum.Current;

                string[] subfolderPathTree = localSubfolder.Path.Split("\\");
                IEnumerable<string> relativeFilePathTree = file.Path.Split("\\").Skip(subfolderPathTree.Length);
                string relativeFilePath = string.Join("/", relativeFilePathTree);

                data.AddRange(Encoding.UTF8.GetBytes($"{remotePath}/{relativeFilePath}"));
                data.AddRange(BitConverter.GetBytes(file.ModifyTime.Ticks));
            }

            return data.ToArray();
        }
        public void AppendReceivedData(byte[] data)
        {
            File file = new File();
            file.Path = Encoding.UTF8.GetString(data.AsSpan().Slice(0, data.Length - 8));
            file.ModifyTime = new DateTime(BitConverter.ToInt64(data.AsSpan().Slice(data.Length - 8, 8)));
            RemoteFiles.Add(file);
        }
    }
}