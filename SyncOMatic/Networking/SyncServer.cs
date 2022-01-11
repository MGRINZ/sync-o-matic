using SyncOMatic.Networking.Requests;
using SyncOMatic.Networking.Responses;
using SyncOMatic.Utils;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic.Networking
{
    public class SyncServer : SyncEndpoint
    {
        private static SyncServer instance;
        private static readonly object instaceLock = new object();

        private TcpListener tcpListener;

        public IPAddress IpAddres { get; set; }
        public short Port { get; set; }

        private SyncServer()
        {
            IpAddres = IPAddress.Any;
            Port = 10000;
        }

        public static SyncServer GetInstance()
        {
            if (instance == null)
                lock (instaceLock)
                    if (instance == null)
                        instance = new SyncServer();
            return instance;
        }

        public void Start()
        {
            tcpListener = new TcpListener(IpAddres, Port);
            tcpListener.Start();
            HandleConnections();
        }

        private async void HandleConnections()
        {
            while (true)
            {
                try
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    ServeClient(tcpClient);
                }
                catch (Exception e)
                {
                    if (tcpListener == null)
                        break;

                    Logger.LogError(e);
                }
            }
        }

        private async void ServeClient(TcpClient tcpClient)
        {
            try
            {
                RequestCodes requestCode = GetRequestCode(tcpClient);
                string clientId = await GetClientId(tcpClient); // For future use
                IPAddress clientIp = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;

                IResponse response = null;

                switch (requestCode)
                {
                    case RequestCodes.GetSharedFolders:
                    {
                        response = new SharedFoldersResponse(clientIp);
                        break;
                    }
                    case RequestCodes.GetSharedSubfolders:
                    {
                        response = new SharedSubfoldersResponse(clientIp);
                        break;
                    }
                    case RequestCodes.GetFileList:
                    {
                        response = new FilesListResponse(clientIp);
                        break;
                    }
                }

                if (response != null)
                    await SendResponseAsync(tcpClient, response);

                tcpClient.Close();
            }
            catch
            {
                tcpClient.Close();
                return;
            }
        }

        private RequestCodes GetRequestCode(TcpClient tcpClient)
        {
            return (RequestCodes)tcpClient.GetStream().ReadByte();
        }

        /// <summary>
        /// Reads the client's id from network stream. Reserved for future use.
        /// </summary>
        /// <param name="tcpClient">The TcpClient which id is supposed to be read.</param>
        /// <returns>10 characters long alphanumeric string containing client's id.</returns>
        private async Task<string> GetClientId(TcpClient tcpClient)
        {
            byte[] buffer = new byte[10];
            await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer);
        }

        private async Task SendResponseAsync(TcpClient tcpClient, IResponse response)
        {

            if (tcpClient.GetStream().DataAvailable)
            {
                byte[] requestDataLengthBytes = new byte[4];
                tcpClient.GetStream().Read(requestDataLengthBytes, 0, 4);

                int requestDataLength = GetReceiveDataLength(requestDataLengthBytes);
                byte[] requestData = new byte[requestDataLength];
                await tcpClient.GetStream().ReadAsync(requestData, 0, requestDataLength);
                response.ParseRequestData(requestData);
                await SendData(tcpClient, response);
            }
            else
                await SendData(tcpClient, response);
        }

        private async Task SendData(TcpClient tcpClient, IResponse response)
        {
            byte[] buffer;
            while ((buffer = response.GetDataToSend()).Length > 0)
            {
                byte[] length = GetSendDataLength(buffer);

                try
                {
                    await tcpClient.GetStream().WriteAsync(length, 0, 4);
                    await tcpClient.GetStream().WriteAsync(buffer, 0, buffer.Length);
                }
                catch (IOException e)
                {
                    Logger.LogInfo(e);
                    return;
                }
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Stop()
        {
            if (tcpListener != null)
                tcpListener.Stop();
            tcpListener = null;
        }
    }
}
