using ELM327API.Processing.DataStructures;

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
    /// Legislated 11-bit CAN Identifiers per ISO 15765-4.
    /// See ISO 15765-4, Section 6.3.2.2, page 12.
    /// </summary>
    public enum CAN11BitLegislatedIdentifiers : uint
    {
        NONE                      = 0x000,  // This option is effectively NULL for this enum
        REQ_FM_TEST_EQUIP_TO_FUNC = 0x7DF,
        REQ_FM_TEST_EQUIP_TO_ECU1 = 0x7E0,  // Recommended REQUEST ID for ECM (Engine Control Module)
        RSP_FM_ECU1_TO_TEST_EQUIP = 0x7E8,  // Recommended RESPONSE ID for ECM (Engine Control Module)
        REQ_FM_TEST_EQUIP_TO_ECU2 = 0x7E1,  // Recommended REQUEST ID for TCM (Transmission Control Module)
        RSP_FM_ECU2_TO_TEST_EQUIP = 0x7E9,  // Recommended RESPONSE ID for TCM (Transmission Control Module)
        REQ_FM_TEST_EQUIP_TO_ECU3 = 0x7E2,
        RSP_FM_ECU3_TO_TEST_EQUIP = 0x7EA,
        REQ_FM_TEST_EQUIP_TO_ECU4 = 0x7E3,
        RSP_FM_ECU4_TO_TEST_EQUIP = 0x7EB,
        REQ_FM_TEST_EQUIP_TO_ECU5 = 0x7E4,
        RSP_FM_ECU5_TO_TEST_EQUIP = 0x7EC,
        REQ_FM_TEST_EQUIP_TO_ECU6 = 0x7E5,
        RSP_FM_ECU6_TO_TEST_EQUIP = 0x7ED,
        REQ_FM_TEST_EQUIP_TO_ECU7 = 0x7E6,
        RSP_FM_ECU7_TO_TEST_EQUIP = 0x7EE,
        REQ_FM_TEST_EQUIP_TO_ECU8 = 0x7E7,
        RSP_FM_ECU8_TO_TEST_EQUIP = 0x7EF
    }

    /// <summary>
    /// Categorize the handlers to indicate which which parts of OBD Express each code
    /// should be made available to.
    /// </summary>
    public enum HandlerCategory
    {
        UNIFIED_DIAGNOSTIC_SERVICES,    // UDS Messages: Examples include DiagnosticSessionControl, ECUReset, TesterPresent, etc.
        REAL_TIME_STATUS,               // Real-time Status: Examples include RPM, Speed, Temperatures, etc. Used by the dashboard, chart logging, etc.
        VEHICLE_INFORMATION,            // Vehicle Information: Examples include VIN, Fuel Type, Manufacturer, and Year Model. Used by Vehicle Information Panel and Labels for Log Dumps.
        TROUBLE_IDENTIFICATION,         // Trouble Identification: Examples include MIL Lamp, Distance Since MIL Lamp Turned on, etc. Used by Trouble Codes sections, Vehicle Information Panel, etc.
        ALL                             // All: Make the handler available to all sections of OBD Express. Use of this category is STRONGLY discouraged.
    }
}
