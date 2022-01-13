using SyncOMatic.Model.FileSystem;
using System.Text;

namespace SyncOMatic.Networking.Requests
{
    public class GetFileRequest : IRequest
    {
        private File remoteFile;
        private bool moreData = true;

        public RequestCodes RequestCode { get; private set; }
        public bool SendsData { get; private set; }

        public GetFileRequest(File remoteFile)
        {
            this.remoteFile = remoteFile;
            RequestCode = RequestCodes.GetFile;
            SendsData = true;
        }

        public byte[] GetDataToSend()
        {
            byte[] data = new byte[0];

            if (moreData)
                data = Encoding.UTF8.GetBytes(remoteFile.Path);

            moreData = false;

            return data;
        }
    }
}