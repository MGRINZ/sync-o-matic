﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SyncOMatic
{
    public class SyncServer
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
            if(instance == null)
                lock(instaceLock)
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
            while(true)
            {
                try
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    ServeClient(tcpClient);
                }
                catch(Exception e)
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

                SharedFoldersResponse response = null;

                switch (requestCode)
                {
                    case RequestCodes.GetSharedFolders:
                    {
                        response = new SharedFoldersResponse(clientIp);
                        break;
                    }
                }

                if(response != null)
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

        private async Task SendResponseAsync(TcpClient tcpClient, SharedFoldersResponse response)
        {
            byte[] buffer;
            while ((buffer = response.GetData()).Length > 0)
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

        private byte[] GetSendDataLength(byte[] data)
        {
            byte[] length = new byte[4];

            for (int i = 0; i < 4; i++)
                length[i] = (byte)((data.Length >> (8 * (3 - i))) & 0xff);

            return length;
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