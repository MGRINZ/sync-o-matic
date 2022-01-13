using SyncOMatic.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SyncOMatic.Networking.Responses
{
    public class GetFileResponse : IResponse
    {
        private IList<SharedFolder> sharedFolders;
        private FileStream fileStream;
        private string remotePath;
        private Fields currentField;

        private readonly IPAddress clientIp;

        public Model.FileSystem.File ReceivedFile { get; set; }

        public GetFileResponse(IPAddress clientIp)
        {
            foreach (var device in App.RemoteDevices)
            {
                if (!device.IpAddress.Equals(clientIp))
                    continue;

                sharedFolders = device.SharedFolders;
                break;
            }

            ReceivedFile = new Model.FileSystem.File();
            currentField = Fields.Path;
            this.clientIp = clientIp;
        }

        public void ParseRequestData(byte[] data)
        {
            remotePath = Encoding.UTF8.GetString(data);

            string[] remotePathTree = remotePath.Split("/");
            string remoteRoot = remotePathTree[1];  // index 0 is empty
            string relativePath = Path.Combine(remotePathTree.Skip(2).ToArray());
            foreach (var folder in sharedFolders)
            {
                if (folder.Name == remoteRoot && folder.CanRead)
                {
                    string localPath = Path.Combine(folder.Path, relativePath);
                    fileStream = new FileStream(localPath, FileMode.Open);
                    break;
                }
            }
        }

        public byte[] GetDataToSend()
        {
            byte[] data = new byte[0];
            if (currentField == Fields.Path)
            {
                data = Encoding.UTF8.GetBytes(remotePath);
                currentField = Fields.FileStream;
            }
            else if(currentField == Fields.FileStream)
            {
                long dataAvailable = fileStream.Length - fileStream.Position;
                if (dataAvailable > 256)
                    data = new byte[256];
                else
                    data = new byte[dataAvailable];
                fileStream.Read(data, 0, data.Length);
            }

            return data;
        }

        public void AppendReceivedData(byte[] data)
        {
            if (currentField == Fields.Path)
            {
                if (fileStream == null)
                {
                    string remotePath = Encoding.UTF8.GetString(data);

                    ReceivedFile.Path = Path.Combine(
                        Path.GetTempPath(),
                        "SyncOMatic",
                        clientIp.ToString(),
                        Path.Combine(remotePath.Split("/").Skip(1).ToArray())
                    );

                    Directory.CreateDirectory(Path.GetDirectoryName(ReceivedFile.Path));
                    fileStream = new FileStream(ReceivedFile.Path, FileMode.Create);
                    currentField = Fields.FileStream;
                }
            }
            else if(currentField == Fields.FileStream)
            {
                if(fileStream != null)
                    fileStream.Write(data);
            }
        }

        public void OnReceiveDataEnd()
        {
            if(fileStream != null)
                fileStream.Close();
        }

        enum Fields
        {
            Path,
            FileStream
        }
    }
}