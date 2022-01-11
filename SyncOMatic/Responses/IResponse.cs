namespace SyncOMatic.Responses
{
    public interface IResponse
    {
        public static byte NO_DATA_BYTE = 255;

        public byte[] GetData();
        public void AppendData(byte[] data);
        public void AppendToSend(byte[] data);
    }
}
