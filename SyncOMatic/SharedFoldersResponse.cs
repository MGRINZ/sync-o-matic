using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace SyncOMatic
{
    public class SharedFoldersResponse
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

        public byte[] GetData()
        {
            List<byte> data = new List<byte>();

            if (sharedFoldersEnum == null)
                return new byte[0];

            while(sharedFoldersEnum.MoveNext())
            {
                SharedFolder folder = sharedFoldersEnum.Current;
                
                if (!folder.IsActive)
                    continue;

                data.AddRange(Encoding.UTF8.GetBytes(folder.Path));
                data.Add(Convert.ToByte(folder.CanRead));
                data.Add(Convert.ToByte(folder.CanWrite));

                break;
            }

            return data.ToArray();
        }

        public void AppendData(byte[] data)
        {
            int length = data.Length;
            var sharedFolder = new SharedFolder();
            sharedFolder.Path = Encoding.UTF8.GetString(data.AsSpan().Slice(0, length - 2));
            sharedFolder.CanRead = Convert.ToBoolean(data[length - 2]);
            sharedFolder.CanWrite = Convert.ToBoolean(data[length - 1]);
            SharedFolders.Add(sharedFolder);
        }
    }
}