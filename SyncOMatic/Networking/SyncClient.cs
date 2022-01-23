using SyncOMatic.Networking.Requests;
using SyncOMatic.Networking.Responses;
using SyncOMatic.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SyncOMatic.Networking
{
    public class SyncClient : SyncEndpoint
    {
        private TcpClient tcpClient;
        private IPAddress ipAddress;
        private short port;

        public SyncClient(IPAddress ipAddress, short port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
        }

        public async Task<IResponse> SendRequestAsync(IRequest request)
        {
            tcpClient = new TcpClient();

            IAsyncResult result = tcpClient.BeginConnect(ipAddress, port, null, null);
            result.AsyncWaitHandle.WaitOne(10000);
            if (!tcpClient.Connected)
            {
                MessageBox.Show($"Nie udało się nawiązać połączenia z {ipAddress}:{port}", "Błąd połączenia", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            tcpClient.EndConnect(result);

            tcpClient.GetStream().ReadTimeout = 10000;
            tcpClient.GetStream().WriteTimeout = 10000;

            SendRequestCode(request.RequestCode);
            SendClientId("0123456789"); // for future use

            IResponse response = GetResponseObject(request.RequestCode);

            if (request.SendsData)
            {
                byte[] requestBuffer;
                while ((requestBuffer = request.GetDataToSend()).Length > 0)
                {
                    byte[] requestLength = GetSendDataLength(requestBuffer);

                    try
                    {
                        await tcpClient.GetStream().WriteAsync(requestLength, 0, 4);
                        await tcpClient.GetStream().WriteAsync(requestBuffer, 0, requestBuffer.Length);

                        await ReceiveData(response);
                    }
                    catch (IOException e)
                    {
                        Logger.LogInfo(e);
                        return response;
                    }
                }
            }
            else
            {
                try
                {
                    await ReceiveData(response);
                }
                catch (IOException e)
                {
                    Logger.LogInfo(e);
                    return response;
                }
            }

            if(response != null)
                response.OnDataEnd();

            tcpClient.Close();
            return response;
        }

        private IResponse GetResponseObject(RequestCodes requestCode)
        {
            IResponse response = null;
            switch (requestCode)
            {
                case RequestCodes.GetSharedFolders:
                {
                    response = new SharedFoldersResponse();
                    break;
                }
                case RequestCodes.GetSharedSubfolders:
                {
                    response = new SharedSubfoldersResponse();
                    break;
                }
                case RequestCodes.GetFilesList:
                {
                    response = new GetFilesListResponse();
                    break;
                }
                case RequestCodes.GetFile:
                {
                    response = new GetFileResponse(ipAddress);
                    break;
                }
                case RequestCodes.Sync:
                {
                    response = new SyncResponse();
                    break;
                }
            }
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

        private async Task ReceiveData(IResponse response)
        {
            if (!response.ReceivesData)
                return;
            do
            {
                byte[] responseLengthBytes = new byte[4];
                tcpClient.GetStream().Read(responseLengthBytes, 0, 4);

                int responseLength = GetReceiveDataLength(responseLengthBytes);
                byte[] responseBuffer = new byte[responseLength];
                await tcpClient.GetStream().ReadAsync(responseBuffer, 0, responseLength);
                response.AppendReceivedData(responseBuffer);
            } while (tcpClient.GetStream().DataAvailable);
        }
    }
}
