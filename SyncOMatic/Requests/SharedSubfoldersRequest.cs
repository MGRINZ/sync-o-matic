using System.Text;

namespace SyncOMatic.Requests
{
    public class SharedSubfoldersRequest : IRequest
    {
        private string path;
        private bool moreData = true;

        public RequestCodes RequestCode { get; private set; }
        public bool SendsData { get; private set; }

        public SharedSubfoldersRequest(string path)
        {
            RequestCode = RequestCodes.GetSharedSubfolders;
            SendsData = true;
            this.path = path;
        }

        public byte[] GetData()
        {
            byte[] data = new byte[0];

            if (moreData)
                data = Encoding.UTF8.GetBytes(path);

            moreData = false;

            return data;
        }
    }
}