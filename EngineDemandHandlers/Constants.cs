namespace EngineDemandHandlers
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
        public static readonly string NAME_THROTTLE_POSITION = "Throttle Position";
        public static readonly string NAME_RELATIVE_THROTTLE_POSITION = "Relative Throttle Position";
        public static readonly string NAME_CALCULATED_ENGINE_LOAD = "Calculated Load";
        public static readonly string NAME_ABSOLUTE_ENGINE_LOAD = "Absolute Load";
        public static readonly string NAME_ABSOLUTE_THROTTLE_POSITION_B = "Abs. Throttle Pos. B";
        public static readonly string NAME_ABSOLUTE_THROTTLE_POSITION_C = "Abs. Throttle Pos. C";
        public static readonly string NAME_ACCELERATOR_PEDAL_POSITION_D = "Accel. Pedal Pos. D";
        public static readonly string NAME_ACCELERATOR_PEDAL_POSITION_E = "Accel. Pedal Pos. E";
        public static readonly string NAME_ACCELERATOR_PEDAL_POSITION_F = "Accel. Pedal Pos. F";
        public static readonly string NAME_RELATIVE_ACCELERATOR_PEDAL_POSITION = "Relative Accel. Pedal Pos.";
        public static readonly string NAME_COMMANDED_THROTTLE_ACTUATOR = "Cmd. Throttle Actuator";
        public static readonly string NAME_DRIVER_DEMANDED_TORQUE= "Driver Demanded Torque";
        public static readonly string NAME_ACTUAL_ENGINE_TORQUE = "Actual Engine Torque";
        public static readonly string NAME_ENGINE_REFERENCE_TORQUE = "Engine Reference Torque";
    }
}
