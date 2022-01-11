namespace SyncOMatic.Networking.Responses
{
    public interface IResponse
    {
        public static byte NO_DATA_BYTE = 255;

        public byte[] GetDataToSend();
        public void AppendReceivedData(byte[] data);
        public void ParseRequestData(byte[] data);
    }
}
