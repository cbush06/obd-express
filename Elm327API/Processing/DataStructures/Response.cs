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
