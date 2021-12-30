namespace SyncOMatic.Responses
{
    public interface IResponse
    {
        public byte[] GetData();
        public void AppendData(byte[] data);
        public void AppendToSend(byte[] data);
    }
}
