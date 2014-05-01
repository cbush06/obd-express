namespace BasicHandlers
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
        public static readonly string NAME_ENGINE_COOLANT_TEMPERATURE_C = "Coolant Temp. " + (new string(new char[] { '(', (char)176, 'C', ')' }));
        public static readonly string NAME_ENGINE_COOLANT_TEMPERATURE_F = "Coolant Temp. " + (new string(new char[] { '(', (char)176, 'F', ')' }));
        public static readonly string NAME_ENGINE_OIL_TEMPERATURE_C = "Oil Temp. " + (new string(new char[] { '(', (char)176, 'C', ')' }));
        public static readonly string NAME_ENGINE_OIL_TEMPERATURE_F = "Oil Temp. " + (new string(new char[] { '(', (char)176, 'F', ')' }));
        public static readonly string NAME_FUEL_LEVEL = "Fuel Level";
        public static readonly string NAME_VIN_NUMBER = "VIN Number";
        public static readonly string NAME_ENGINE_RPM = "Engine RPM";
        public static readonly string NAME_VEHICLE_SPEED_MPH = "Vehicle Speed (mph)";
        public static readonly string NAME_VEHICLE_SPEED_KPH = "Vehicle Speed (kph)";
        public static readonly string NAME_INTAKE_AIR_TEMPERATURE_C = "Intake Air Temp. " + (new string(new char[] { '(', (char)176, 'C', ')' }));
        public static readonly string NAME_INTAKE_AIR_TEMPERATURE_F = "Intake Air Temp. " + (new string(new char[] { '(', (char)176, 'F', ')' }));
        public static readonly string NAME_BAROMETRIC_PRESSURE = "Barometric Pressure";
        public static readonly string NAME_AMBIENT_AIR_TEMPERATURE_C = "Ambient Air Temp. " + (new string(new char[] { '(', (char)176, 'C', ')' }));
        public static readonly string NAME_AMBIENT_AIR_TEMPERATURE_F = "Ambient Air Temp. " + (new string(new char[] { '(', (char)176, 'F', ')' }));
        public static readonly string NAME_OBD_STANDARDS = "OBD Standards of Vehicle";
    }
}
