using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncOMatic
{
    public class SyncEndpoint
    {
        protected byte[] GetSendDataLength(byte[] data)
        {
            byte[] length = new byte[4];

            for (int i = 0; i < 4; i++)
                length[i] = (byte)((data.Length >> (8 * (3 - i))) & 0xff);

            return length;
        }

        protected int GetReceiveDataLength(byte[] length)
        {
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
