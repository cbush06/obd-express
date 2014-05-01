using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM327API.Processing.DataStructures
{
    class Response
    {
        public ulong DataLength = 0;
        public ulong ReceivedLength = 0;
        public ushort ReceivedFrames = 0;
        public byte[] Data;
    }
}
