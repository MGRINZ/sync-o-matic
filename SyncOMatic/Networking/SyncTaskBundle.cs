using SyncOMatic.Model;
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
        public List<SyncTask> SendBundle { get; private set; }
        public List<Task> FetchTasks { get; private set; }
        public bool InProgress;

        public SyncTaskBundle()
        {
            GetBundle = new List<SyncTask>();
            SendBundle = new List<SyncTask>();
            FetchTasks = new List<Task>();
            InProgress = false;
        }

        public void Add(SyncTask syncTask)
        {
            var syncRule = syncTask.SyncRule;
            if (syncRule.SyncMethod == SyncMethod.ReadOnly || syncRule.SyncMethod == SyncMethod.ReadWrite)
            {
                GetBundle.Add(syncTask);
                FetchTasks.Add(syncTask.FetchFilesList());
            }

            if (syncRule.SyncMethod == SyncMethod.WriteOnly || syncRule.SyncMethod == SyncMethod.ReadWrite)
                SendBundle.Add(syncTask);
        }

        public async Task Run()
        {
            List<Task> runningTasks = new List<Task>();
            InProgress = true;
            
            foreach (var syncTask in GetBundle)
                runningTasks.Add(syncTask.RequestFiles());
            
            await Task.WhenAll(runningTasks);
        }
    }
}
