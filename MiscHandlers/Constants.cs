namespace MiscHandlers
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
        public static readonly string NAME_COMMANDED_EGR = "Commanded EGR";
        public static readonly string NAME_EGR_ERROR = "EGR Error";
        public static readonly string NAME_COMMANDED_EVAP_PURGE = "Commanded Evaporative Purge";
        public static readonly string NAME_EVAP_SYS_VAPOR_PRESSURE = "Evaporative System Vapor Pressure";
    }
}
