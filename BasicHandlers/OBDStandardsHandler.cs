using ELM327API;
using ELM327API.Processing.DataStructures;
using ELM327API.Processing.Interfaces;
using System;

namespace BasicHandlers
{
    public class OBDStandardsHandler : IHandler
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
            get { return Constants.NAME_OBD_STANDARDS; }
        }

        public HandlerCategory Category
        {
            get { return HandlerCategory.VEHICLE_INFORMATION; }
        }

        public string Request
        {
            get { return "011C"; }
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
            get { return 0x1C; }
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

            switch (data[0])
            {
                case 0x01:
                {
                    value = "OBD II (California ARB)";
                    break;
                }

                case 0x02:
                {
                    value = "OBD (Federal EPA)";
                    break;
                }

                case 0x03:
                {
                    value = "OBD and OBD II";
                    break;
                }

                case 0x04:
                {
                    value = "OBD I";
                    break;
                }

                case 0x05:
                {
                    value = "Not OBD Compliant";
                    break;
                }

                case 0x06:
                {
                    value = "EOBD";
                    break;
                }

                case 0x07:
                {
                    value = "EOBD and OBD II";
                    break;
                }

                case 0x08:
                {
                    value = "EOBD and OBD";
                    break;
                }

                case 0x09:
                {
                    value = "EOBD, OBD, and OBDII";
                    break;
                }

                case 0x0A:
                {
                    value = "JOBD";
                    break;
                }

                case 0x0B:
                {
                    value = "JOBD and OBDII";
                    break;
                }

                case 0x0C:
                {
                    value = "JOBD and EOBD";
                    break;
                }

                case 0x0D:
                {
                    value = "JOBD, EOBD, and OBD II";
                    break;
                }

                case 0x0E:
                {
                    value = "Heavy Duty Vehicles (EURO IV) B1";
                    break;
                }

                case 0x0F:
                {
                    value = "Heavy Duty Vehicles (EURO V) B2";
                    break;
                }

                case 0x10:
                {
                    value = "Heavy Duty Vehicles (EURO EEC) C (gas engines)";
                    break;
                }

                case 0x11:
                {
                    value = "Engine Manufacturer Diagnostics (EMD)";
                    break;
                }

                default:
                {
                    if (data[0] > 0x11 && data[0] < 0xFB)
                    {
                        value = "ISO/SAE reserved";
                    }
                    else if (data[0] > 0xFA)
                    {
                        value = "ISO/SAE Not available for assignment";
                    }
                    break;
                }
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
