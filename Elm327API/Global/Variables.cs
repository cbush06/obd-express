using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM327API
{
    /// <summary>
    /// ELM327 Protocols by number
    /// </summary>
    public enum ProtocolsEnum
    {
        AUTO,                           // 0x0
        SAE_J1850_PWM,                  // 0x1
        SAE_J1850_VPW,                  // 0x2
        ISO_9141_2,                     // 0x3
        ISO_14230_4_KWP_5_BAUD,         // 0x4
        ISO_14230_4_KWP_FAST,           // 0x5
        ISO_15765_4_CAN_11_BIT_ID_500,  // 0x6
        ISO_15765_4_CAN_29_BIT_ID_500,  // 0x7
        ISO_15765_4_CAN_11_BIT_ID_250,  // 0x8
        ISO_15765_4_CAN_29_BIT_ID_250,  // 0x9
        SAE_J1939_CAN_29_BIT_ID_250,    // 0xA
        USER1_CAN_11_BIT_ID_125,        // 0xB
        USER2_CAN_11_BIT_ID_50,         // 0xC
        ALL                             // 0xD -- This is used by Handlers to indicate they are compatible with all protocols (their
                                        //        request is a generic OBD request to be interpreted by the ELM327).
    }

    /// <summary>
    /// Categorize the handlers to indicate which which parts of OBD Express each code
    /// should be made available to.
    /// </summary>
    public enum HandlerCategory
    {
        REAL_TIME_STATUS,               // Real-time Status: Examples include RPM, Speed, Temperatures, etc. Used by the dashboard, chart logging, etc.
        VEHICLE_INFORMATION,            // Vehicle Information: Examples include VIN, Fuel Type, Manufacturer, and Year Model. Used by Vehicle Information Panel and Labels for Log Dumps.
        TROUBLE_IDENTIFICATION,         // Trouble Identification: Examples include MIL Lamp, Distance Since MIL Lamp Turned on, etc. Used by Trouble Codes sections, Vehicle Information Panel, etc.
        ALL                             // All: Make the handler available to all sections of OBD Express. Use of this category is STRONGLY discouraged.
    }
}
