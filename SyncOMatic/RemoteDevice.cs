﻿using System;
using System.Collections.ObjectModel;
using System.Net;

namespace SyncOMatic
{
    public class RemoteDevice
    {
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Hostname { get; set; }
        public IPAddress IpAddress { get; set; }
        public short Port { get; set; }
        public DateTime LastSync { get; set; }
        public ObservableCollection<SharedFolder> SharedFolders { get; private set; }
        public ObservableCollection<SyncRule> SyncRules{ get; private set; }
    }
}