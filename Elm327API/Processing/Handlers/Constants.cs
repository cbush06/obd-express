namespace ELM327API.Processing.Handlers
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
        public static readonly string NAME_DIAGNOSTIC_SESSION_CONTROL = "Diagnostic Session Control";
    }
}
