using ELM327API.Processing.DataStructures;

namespace ELM327API.Global
{
    public class Constants
    {
        /// <summary>
        /// Number of times to try sending an AT command before giving up.
        /// </summary>
        public static int AT_COMMAND_RETRIES = 3;

        /// <summary>
        /// This ECU represents a non-selection by the user. The active protocol
        /// should interpret this as indicating that requests are broadcasted and
        /// responses are not filtered in any way.
        /// </summary>
        public static readonly ECU NoSelection = new ECU("All ECUs", -1, 0, 0);

        /*
         * Standard error messages for ELM327.
         */
        public const string NO_DATA_MESSAGE = "NO DATA";
        public const string SEARCHING_MESSAGE = "SEARCHING...";
        public const string UNABLE_TO_CONNECT_MESSAGE = "UNABLE TO CONNECT";
        public const string STOPPED_MESSAGE = "STOPPED";
    }
}
