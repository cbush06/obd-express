using ELM327API;
using ELM327API.Processing.DataStructures;
using ELM327API.Processing.Interfaces;
using System;

namespace DiagnosticHandlers
{
    public class MilLampIndicatorHandler : IHandler
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
            get { return Constants.NAME_ECU_DTC_COUNT; }
        }

        public string Request
        {
            get { return "0101"; }
        }

        public HandlerCategory Category
        {
            get { return HandlerCategory.TROUBLE_IDENTIFICATION; }
        }

        public bool IsMutable
        {
            get { return Constants.IMMUTABLE; }
        }

        public string Unit
        {
            get { return ""; }
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
            get { return 0x01; }
        }

        public byte OBDPID
        {
            get { return 0x01; }
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
            String value = "";
            byte milLampBit = (byte)0x00;

            // Bit 7 of byte 1 = MIL Lamp Status
            milLampBit = (byte)(data[0] & 0x80);

            if (milLampBit > 0)
            {
                value = "ON";
            }
            else
            {
                value = "OFF";
            }

            arg = new ELM327ListenerEventArgs(this, value);

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
