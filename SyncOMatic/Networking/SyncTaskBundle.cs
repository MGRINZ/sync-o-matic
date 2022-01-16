using SyncOMatic.Model;
using SyncOMatic.Networking.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic.Networking
{
    public class SyncTaskBundle
    {
        public List<SyncTask> GetBundle { get; private set; }
        public Dictionary<RemoteDevice, List<SyncTask>> SendBundle { get; private set; }

        public bool InProgress;

        public SyncTaskBundle()
        {
            GetBundle = new List<SyncTask>();
            SendBundle = new Dictionary<RemoteDevice, List<SyncTask>>();
            InProgress = false;
        }

        public void Add(SyncTask syncTask)
        {
            var syncRule = syncTask.SyncRule;
            if (syncRule.SyncMethod == SyncMethod.ReadOnly || syncRule.SyncMethod == SyncMethod.ReadWrite)
                GetBundle.Add(syncTask);

            if (syncRule.SyncMethod == SyncMethod.WriteOnly || syncRule.SyncMethod == SyncMethod.ReadWrite)
            {
                if (!SendBundle.ContainsKey(syncTask.RemoteDevice))
                    SendBundle.Add(syncTask.RemoteDevice, new List<SyncTask>());
                SendBundle[syncTask.RemoteDevice].Add(syncTask);
            }
        }

        public async Task FetchFilesList()
        {
            List<Task> fetchTasks = new List<Task>();
            foreach (var syncTask in GetBundle)
                fetchTasks.Add(syncTask.FetchFilesList());
            await Task.WhenAll(fetchTasks);
        }

        public async Task Run()
        {
            List<Task> runningTasks = new List<Task>();
            InProgress = true;
            
            foreach (var syncTask in GetBundle)
                runningTasks.Add(syncTask.RequestFiles());
            
            await Task.WhenAll(runningTasks);

            string tempPath = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                App.AppName
            );
            if(System.IO.Directory.Exists(tempPath))
                System.IO.Directory.Delete(tempPath, true);

            await SendFiles();
        }

        public async Task SendFiles()
        {
            foreach (var deviceTaskPair in SendBundle)
            {
                var device = deviceTaskPair.Key;
                SyncClient syncClient = new SyncClient(device.IpAddress, device.Port);
                await syncClient.SendRequestAsync(new SyncRequest(deviceTaskPair.Value));
            } 
        }
    }
}
