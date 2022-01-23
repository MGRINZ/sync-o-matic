using SyncOMatic.Model;
using SyncOMatic.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic.Networking.Responses
{
    public class SyncResponse : IResponse
    {
        public bool ReceivesData { get; } = false;

        private RemoteDevice remoteDevice;
        private IList<SharedFolder> sharedFolders;
        private SyncTaskBundle syncTaskBundle;

        public SyncResponse() { }

        public SyncResponse(IPAddress clientIp)
        {
            foreach (var device in App.RemoteDevices)
            {
                if (!device.IsActive)
                    continue;

                if (!device.IpAddress.Equals(clientIp))
                    continue;

                remoteDevice = device;
                sharedFolders = device.SharedFolders;
                break;
            }
            syncTaskBundle = new SyncTaskBundle();
        }

        public void ParseRequestData(byte[] data)
        {
            var syncRule = new SyncRule();
            syncRule.FileExclusions.Clear();
            syncRule.IsActive = true;
            syncRule.RemoteDir = $"/{Encoding.UTF8.GetString(data.AsSpan(0, 64))}";
            syncRule.SyncMethod = SyncMethod.ReadOnly;
            string remotePath = Encoding.UTF8.GetString(data.AsSpan(64));

            foreach (var folder in sharedFolders)
            {
                if (folder.Name == remotePath.Split("/")[1] && folder.CanWrite)
                {
                    syncRule.LocalDir = Path.Combine(
                        folder.Path,
                        Path.Combine(remotePath.Split("/").Skip(2).ToArray())
                    );
                    var syncTask = new SyncTask(remoteDevice, syncRule);

                    syncTaskBundle.Add(syncTask);

                    break;
                }
            }
        }

        public void OnDataEnd()
        {
            if (syncTaskBundle == null)
                return;

            SyncCoordinator syncCoordinator = SyncCoordinator.GetInstance();
            syncCoordinator.AddSyncBundle(syncTaskBundle);
            syncCoordinator.StartSync();
        }

        public byte[] GetDataToSend()
        {
            return new byte[0];
        }
        public void AppendReceivedData(byte[] data) { }
    }
}
