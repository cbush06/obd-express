using ELM327API.Processing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELM327API.Processing.DataStructures;

namespace ELM327API.Processing.Handlers
{
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

        private string _header = CAN11BitLegislatedIdentifiers.REQ_FM_TEST_EQUIP_TO_ECU1.ToString("X3");
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
            get { return (this.RegisteredListeners != null); }
        }

        public bool HasRegisteredSingleListeners
        {
            get { return (this.RegisteredSingleListeners != null); }
        }

        public ProtocolsEnum Compatibility
        {
            get { return ProtocolsEnum.ALL; }
        }

        public void RegisterListener(Action<ELM327API.Processing.DataStructures.ELM327ListenerEventArgs> callback)
        {
            if (callback != null)
            {
                this.RegisteredListeners += callback;
            }
        }

        public void UnregisterListener(Action<ELM327API.Processing.DataStructures.ELM327ListenerEventArgs> callback)
        {
            if (callback != null && this.RegisteredListeners != null)
            {
                this.RegisteredListeners -= callback;
            }

            if (callback != null && this.RegisteredSingleListeners != null)
            {
                this.RegisteredSingleListeners -= callback;
            }
        }

        public void RegisterSingleListener(Action<ELM327API.Processing.DataStructures.ELM327ListenerEventArgs> callback)
        {
            if (callback != null)
            {
                this.RegisteredSingleListeners += callback;
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

            if (this.RegisteredListeners != null)
            {
                this.RegisteredListeners(arg);
            }

            if (this.RegisteredSingleListeners != null)
            {
                this.RegisteredSingleListeners(arg);
                this.RegisteredSingleListeners = null;
            }
        }
    }
}
