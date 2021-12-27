using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic
{
    public class SyncClient
    {
        private TcpClient tcpClient;
        private IPAddress ipAddress;
        private short port;

        public SyncClient(IPAddress ipAddress, short port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public async Task<SharedFoldersResponse> SendRequestAsync(SharedFoldersRequest request)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(ipAddress, port);
            if (!tcpClient.Connected)
                return null;

            SendRequestCode(request.RequestCode);
            SendClientId("0123456789"); // for future use

            SharedFoldersResponse response = new SharedFoldersResponse();

            //while
                //request.GetData();
                //await tcpClient.GetStream().WriteAsync();

                do
                {
                    int length = GetReceiveDataLength();
                    byte[] buffer = new byte[length];
                    await tcpClient.GetStream().ReadAsync(buffer, 0, length);
                    response.AppendData(buffer);
                } while (tcpClient.GetStream().DataAvailable);
            //

            tcpClient.Close();
            return response;
        }

        private void SendRequestCode(RequestCodes requestCode)
        {
            tcpClient.GetStream().WriteByte((byte)requestCode);
        }

        private void SendClientId(string clientId)
        {
            if (clientId.Length != 10)
                throw new Exception("Invalid cliendId.");
            byte[] buffer;
            buffer = Encoding.ASCII.GetBytes(clientId);
            tcpClient.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        private int GetReceiveDataLength()
        {
            byte[] length = new byte[4];
            tcpClient.GetStream().Read(length, 0, 4);

            int bufferLength = 0;

            bufferLength |= length[0];
            for (int i = 1; i < 4; i++)
            {
                bufferLength <<= 8;
                bufferLength |= length[i];
            }
            return bufferLength;
        }
    }
}
