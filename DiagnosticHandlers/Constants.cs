using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagnosticHandlers
{
    public class Constants
    {
        /*
         * Basic Properties.
         */
        public static readonly bool MUTABLE = true;
        public static readonly bool IMMUTABLE = false;

        public static readonly bool CUSTOM_HEADER = true;
        public static readonly bool DEFAULT_HEADER = false;

        public static readonly bool OBD_REQUEST = true;
        public static readonly bool NON_OBD_REQUEST = false;

        public static readonly bool RESPONSE_EXPECTED = true;
        public static readonly bool RESPONSE_NOT_EXPECTED = false;

        /*
         * Handler Names.
         */
        public static readonly string NAME_ECU_DTC_COUNT = "Diagnostic Trouble Code Count";
        public static readonly string NAME_MIL_LAMP_INDICATOR = "Malfunction Indicator Lamp";
        public static readonly string NAME_DISTANCE_TRAVELED_MIL_LAMP_KM = "Distance Traveled With MIL Lamp On (km)";
        public static readonly string NAME_DISTANCE_TRAVELED_MIL_LAMP_MI = "Distance Traveled With MIL Lamp On (mi)";
        public static readonly string NAME_WARMUPS_SINCE_CODES_CLEARED = "Warm-ups Since Codes Cleared";
        public static readonly string NAME_DISTANCE_TRAVELED_SINCE_CODES_CLEARED_KM = "Distance Since Codes Cleared (km)";
        public static readonly string NAME_DISTANCE_TRAVELED_SINCE_CODES_CLEARED_MI = "Distance Since Codes Cleared (mi)";
    }
}
