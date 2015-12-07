using ELM327API.Processing.Interfaces;
using System;
using ELM327API.Processing.DataStructures;

namespace ELM327API.Processing.Handlers
{
    /// <summary>
    /// This Handler executes a Unified Diagnostics Service request for
    /// the 0x10 (DiagnosticSessionControl) service and the 0x01 (defaultSession)
    /// subfunction.
    /// 
    /// Reference: ISO15765-3, Section 9.2.1, pg. 42
    /// </summary>
    class DiagnosticSessionControlHandler : IHandler
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
            get { return Constants.NAME_DIAGNOSTIC_SESSION_CONTROL; }
        }

        public HandlerCategory Category
        {
            get { return HandlerCategory.UNIFIED_DIAGNOSTIC_SERVICES; }
        }

        public string Request
        {
            get { return "1001"; }
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
            get { return typeof(UInt32); }
        }

        public bool IsCustomHeader
        {
            get { return Constants.CUSTOM_HEADER; }
        }

        private string _header = String.Empty;
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }

        public bool IsOBD
        {
            get { return Constants.NON_OBD_REQUEST; }
        }

        public byte OBDSID
        {
            get { throw new NotImplementedException(); }
        }

        public byte OBDPID
        {
            get { throw new NotImplementedException(); }
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

        public ProtocolsEnum Compatibility
        {
            get { return ProtocolsEnum.AUTO; }
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

            UInt16[] value = new UInt16[2];

            if(data.Length == 4)
            {
                value[0] = (UInt16)((data[0] << 8) | data[1]);
                value[1] = (UInt16)((data[2] << 8) | data[3]);

                arg = new ELM327ListenerEventArgs(this, value);
            }
            else
            {
                arg = new ELM327ListenerEventArgs(this, null, true, "Only " + data.Length + " bytes of data were returned. Expected 4 bytes.");
            }

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
