using SyncOMatic.Model;
using SyncOMatic.Model.FileSystem;
using SyncOMatic.Networking.Requests;
using SyncOMatic.Networking.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyncOMatic.Networking
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
                    FilesListResponse response = (FilesListResponse) await syncClient.SendRequestAsync(new GetFilesListRequest(syncRule));
                    compareFiles(syncRule, response.RemoteFiles);
                }
            }
        }

        private void compareFiles(SyncRule syncRule, IList<File> remoteFiles)
        {
            foreach (var remoteFile in remoteFiles)
            {
                string localRelativePath = System.IO.Path.Combine(remoteFile.Path.Split("/").Skip(2).ToArray());
                string localPath = System.IO.Path.Combine(syncRule.LocalDir, localRelativePath);
                
                if (System.IO.File.Exists(localPath))
                {
                    File localFile = new File(localPath, null);
                    if (remoteFile.ModifyTime > localFile.ModifyTime)
                        requestFile(remoteFile);
                }
                else
                    requestFile(remoteFile);
            }
        }

        private void requestFile(File remoteFile)
        {
            
        }
    }
}