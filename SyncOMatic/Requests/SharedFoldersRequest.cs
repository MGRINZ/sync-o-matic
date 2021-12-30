namespace SyncOMatic.Requests
{
    public class SharedFoldersRequest : IRequest
    {
        public RequestCodes RequestCode { get; private set; }
        public bool SendsData { get; private set; }

        public SharedFoldersRequest()
        {
            RequestCode = RequestCodes.GetSharedFolders;
            SendsData = false;
        }

        public byte[] GetData()
        {
            return new byte[0];
        }
    }
}
