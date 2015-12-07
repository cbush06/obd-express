using ELM327API;
using ELM327API.Processing.DataStructures;
using ELM327API.Processing.Interfaces;
using System;
using System.Text;

namespace BasicHandlers
{
    public class VINHandler : IHandler
    {
        /// <summary>
        /// Event registered real-time listeners use.
        /// </summary>
        private event Action<ELM327ListenerEventArgs> RegisteredListeners;

        /// <summary>
        /// Event registered single listeners use.
        /// </summary>
        private event Action<ELM327ListenerEventArgs> RegisteredSingleListeners;

        public string Name
        {
            get { return Constants.NAME_VIN_NUMBER; }
        }

        public HandlerCategory Category
        {
            get { return HandlerCategory.VEHICLE_INFORMATION; }
        }

        public string Request
        {
            get { return "0902"; }
        }

        public bool IsMutable
        {
            get { return Constants.IMMUTABLE; }
        }

        public string Unit
        {
            get { return String.Empty; }
        }

        public Type DataType
        {
            get { return typeof(String); }
        }

        public bool IsCustomHeader
        {
            get { return Constants.DEFAULT_HEADER; }
        }

        public string Header
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsOBD
        {
            get { return Constants.OBD_REQUEST; }
        }

        public byte OBDSID
        {
            get { return 0x09; }
        }

        public byte OBDPID
        {
            get { return 0x02; }
        }

        public bool ExpectsResponse
        {
            get { return Constants.RESPONSE_EXPECTED; }
        }

        public bool HasRegisteredListeners
        {
            get { return (RegisteredListeners != null); }
        }

        public bool HasRegisteredSingleListeners
        {
            get { return (RegisteredSingleListeners != null); }
        }

        public ELM327API.ProtocolsEnum Compatibility
        {
            get { return ProtocolsEnum.ALL; }
        }

        public void RegisterListener(Action<ELM327API.Processing.DataStructures.ELM327ListenerEventArgs> callback)
        {
            if (callback != null)
            {
                RegisteredListeners += callback;
            }
        }

        public void UnregisterListener(Action<ELM327API.Processing.DataStructures.ELM327ListenerEventArgs> callback)
        {
            if (callback != null && RegisteredListeners != null)
            {
                RegisteredListeners -= callback;
            }

            if (callback != null && RegisteredSingleListeners != null)
            {
                RegisteredSingleListeners -= callback;
            }
        }

        public void RegisterSingleListener(Action<ELM327API.Processing.DataStructures.ELM327ListenerEventArgs> callback)
        {
            if (callback != null)
            {
                RegisteredSingleListeners += callback;
            }
        }

        public void ProcessResponse(byte[] data)
        {
            ELM327ListenerEventArgs arg;
            StringBuilder value = new StringBuilder(data.Length);

            /* 
             * Convert the string of bytes into a string of characters
             * Skip the first byte. It directly precedes the first
             * actual VIN data in the FIRST FRAME. It indicates the
             * number of data items to expect (in this case 1 because
             * there is only 1 VIN number).
             */
            for (int i = 1; i < data.Length; i++)
            {
                value.Append((char)data[i]);
            }

            arg = new ELM327ListenerEventArgs(this, value.ToString());

            if (RegisteredListeners != null)
            {
                RegisteredListeners(arg);
            }

            if (RegisteredSingleListeners != null)
            {
                RegisteredSingleListeners(arg);
                RegisteredSingleListeners = null;
            }
        }
    }
}
