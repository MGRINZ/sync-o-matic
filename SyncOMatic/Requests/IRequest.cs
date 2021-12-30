namespace SyncOMatic.Requests
{
    public interface IRequest
    {
        public RequestCodes RequestCode { get; }
        public bool SendsData { get; }
        public abstract byte[] GetData();
    }
}
