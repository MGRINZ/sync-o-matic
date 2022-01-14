using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic.Model.FileSystem
{
    public struct LocalRemoteFilesPair
    {
        public File LocalFile { get; set; }
        public File RemoteFile { get; set; }

        public LocalRemoteFilesPair(File f1, File f2)
        {
            LocalFile = f1;
            RemoteFile = f2;
        }
    }
}
