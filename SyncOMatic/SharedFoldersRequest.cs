using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic
{
    public class SharedFoldersRequest
    {
        public RequestCodes RequestCode { get; private set; }

        public SharedFoldersRequest()
        {
            RequestCode = RequestCodes.GetSharedFolders;
        }

        public byte[] GetData()
        {
            return new byte[0];
        }
    }
}
