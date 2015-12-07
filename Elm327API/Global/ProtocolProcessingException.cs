using System;

namespace ELM327API.Global
{
    public class ProtocolProcessingException : Exception
    {
        public ProtocolProcessingException(String message) : base(message)
        {
            // Nothing follows
        }

        public ProtocolProcessingException(String message, Exception innerException)
            : base(message, innerException)
        {
            // Nothing follows
        }
    }
}
