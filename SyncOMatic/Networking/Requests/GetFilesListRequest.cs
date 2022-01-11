using SyncOMatic.Model;
using System.Text;

namespace SyncOMatic.Networking.Requests
{
    public class GetFilesListRequest : IRequest
    {
        private SyncRule syncRule;
        private bool moreData = true;

        public GetFilesListRequest(SyncRule syncRule)
        {
            this.syncRule = syncRule;
            RequestCode = RequestCodes.GetFileList;
            SendsData = true;
        }

        public RequestCodes RequestCode { get; private set; }

        public bool SendsData { get; private set; }

        public byte[] GetData()
        {
            byte[] data = new byte[0];

            if (moreData)
                data = Encoding.UTF8.GetBytes(syncRule.RemoteDir);

            moreData = false;

            return data;
        }
    }
}