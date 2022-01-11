using SyncOMatic.Requests;
using SyncOMatic.Responses;
using System;

namespace SyncOMatic
{
    public class SyncTask
    {
        private RemoteDevice device;

        public SyncTask(RemoteDevice device)
        {
            this.device = device;
        }

        public async void run()
        {
            var syncClient = new SyncClient(device.IpAddress, device.Port);

            foreach (var syncRule in device.SyncRules)
            {
                if (!syncRule.IsActive)
                    continue;

                if (syncRule.SyncMethod == SyncMethod.ReadOnly || syncRule.SyncMethod == SyncMethod.ReadWrite)
                {
                    IResponse response = await syncClient.SendRequestAsync(new GetFilesListRequest(syncRule));
                }
            }
        }
    }
}