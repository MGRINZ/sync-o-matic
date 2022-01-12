using SyncOMatic.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SyncOMatic.Networking.Responses
{
    public class SharedFoldersResponse : IResponse
    {
        public IList<SharedFolder> SharedFolders { get; private set; }
        private IEnumerator<SharedFolder> sharedFoldersEnum;

        public SharedFoldersResponse()
        {
            SharedFolders = new List<SharedFolder>();
        }

        public SharedFoldersResponse(IPAddress clientIp)
        {
            foreach (var device in App.RemoteDevices)
            {
                if (!device.IpAddress.Equals(clientIp))
                    continue;

                SharedFolders = device.SharedFolders;
                sharedFoldersEnum = SharedFolders.GetEnumerator();
                break;
            }
        }

        public byte[] GetDataToSend()
        {
            List<byte> data = new List<byte>();

            if (sharedFoldersEnum == null)
                return new byte[0];

            while (sharedFoldersEnum.MoveNext())
            {
                SharedFolder folder = sharedFoldersEnum.Current;

                if (!folder.IsActive)
                    continue;

                data.AddRange(Encoding.UTF8.GetBytes(folder.Name));
                data.Add(Convert.ToByte(folder.CanRead));
                data.Add(Convert.ToByte(folder.CanWrite));
                folder.LoadLocalSubfolders();
                data.Add(Convert.ToByte(folder.IsEmpty));

                break;
            }

            return data.ToArray();
        }

        public void AppendReceivedData(byte[] data)
        {
            int length = data.Length;
            var sharedFolder = new SharedFolder();
            sharedFolder.Name = Encoding.UTF8.GetString(data.AsSpan().Slice(0, length - 3));
            sharedFolder.Path = $"/{sharedFolder.Name}";
            sharedFolder.CanRead = Convert.ToBoolean(data[length - 3]);
            sharedFolder.CanWrite = Convert.ToBoolean(data[length - 2]);
            sharedFolder.IsEmpty = Convert.ToBoolean(data[length - 1]);
            SharedFolders.Add(sharedFolder);
        }

        public void ParseRequestData(byte[] data)
        {

        }
    }
}