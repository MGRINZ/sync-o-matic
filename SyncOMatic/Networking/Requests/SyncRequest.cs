using SyncOMatic.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic.Networking.Requests
{
    public class SyncRequest : IRequest
    {
        private IEnumerator<SyncTask> syncTasksEnum;

        public RequestCodes RequestCode { get; private set; }

        public bool SendsData { get; private set; }

        public SyncRequest(List<SyncTask> syncTasks)
        {
            RequestCode = RequestCodes.Sync;
            SendsData = true;

            syncTasksEnum = syncTasks.GetEnumerator();
        }

        public byte[] GetDataToSend()
        {
            List<byte> data = new List<byte>();

            if (syncTasksEnum == null)
                return new byte[0];

            if (syncTasksEnum.MoveNext())
            {
                SyncTask syncTask = syncTasksEnum.Current;

                var sharedFolder = new SharedFolder();
                sharedFolder.CanRead = true;
                sharedFolder.IsActive = true;
                sharedFolder.Path = syncTask.SyncRule.LocalDir;

                using (var sha = SHA256.Create())
                {
                    byte[] nameBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(syncTask.SyncRule.LocalDir));
                    sharedFolder.Name = "";
                    foreach (var b in nameBytes)
                        sharedFolder.Name += b.ToString("x2");
                }

                if(syncTask.RemoteDevice.TempSharedFolders.Where(sf => sf.Name == sharedFolder.Name).ToList().Count == 0)
                    syncTask.RemoteDevice.TempSharedFolders.Add(sharedFolder);

                data.AddRange(Encoding.UTF8.GetBytes(sharedFolder.Name));
                data.AddRange(Encoding.UTF8.GetBytes(syncTask.SyncRule.RemoteDir));
            }

            return data.ToArray();
        }
    }
}
