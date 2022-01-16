using SyncOMatic.Model;
using SyncOMatic.Model.FileSystem;
using SyncOMatic.Networking.Requests;
using SyncOMatic.Networking.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncOMatic.Networking
{
    public class SyncTask
    {
        public RemoteDevice RemoteDevice { get; private set; }
        public SyncRule SyncRule { get; private set; }
        public List<LocalRemoteFilesPair> Files { get; private set; }

        public SyncTask(RemoteDevice device, SyncRule syncRule)
        {
            this.RemoteDevice = device;
            this.SyncRule = syncRule;
            this.Files = new List<LocalRemoteFilesPair>();
        }

        public async Task FetchFilesList()
        {
            SyncClient syncClient = new SyncClient(RemoteDevice.IpAddress, RemoteDevice.Port);

            GetFilesListResponse response = (GetFilesListResponse) await syncClient.SendRequestAsync(new GetFilesListRequest(SyncRule));

            foreach (var remoteFile in response.RemoteFiles)
            {
                string localRelativePath = System.IO.Path.Combine(remoteFile.Path.Split("/").Skip(SyncRule.RemoteDir.Split("/").Length).ToArray());
                string localPath = System.IO.Path.Combine(SyncRule.LocalDir, localRelativePath);
                File localFile = new File(localPath, null);
                Files.Add(new LocalRemoteFilesPair(localFile, remoteFile));
            }
        }

        public async Task RequestFiles()
        {
            await Task.Run(async () =>
                {
                    foreach (var file in Files)
                    {
                        SyncClient syncClient = new SyncClient(RemoteDevice.IpAddress, RemoteDevice.Port);
                        GetFileResponse response = (GetFileResponse)await syncClient.SendRequestAsync(new GetFileRequest(file.RemoteFile));
                        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(file.LocalFile.Path));
                        System.IO.File.Move(response.ReceivedFile.Path, file.LocalFile.Path, true);
                        System.IO.File.SetLastWriteTime(file.LocalFile.Path, file.RemoteFile.ModifyTime);
                    }
                }
            );
        }
    }
}