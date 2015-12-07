using log4net;
using System;

namespace ELM327API
{
    public class Utility
    {
        /// <summary>
        /// Get the logger.
        /// </summary>
        protected static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Converts a Hexidecimal string to a byte array. You must provide a string with an even number of characters. Do not preface the string with the hexidecimal sign (i.e. "0x").
        /// </summary>
        /// <param name="input">String with an even number of hexidecimal characters.</param>
        /// <returns>Byte array.</returns>
        public static byte[] HexStringToByteArray(string input)
        {
            byte[] returnValue = new byte[(input.Length / 2) + (input.Length % 2)];
            byte upper = 0x00;
            byte lower = 0x00;

            for (int i = 0; i < input.Length; i += 2)
            {
                // Reset values
                upper = 0x00;
                lower = 0x00;

                // Convert two characters at a time
                upper = Utility.HexFromCharToByte(input[i]);

                // Account for odd numbers of digits
                if (input.Length > (i + 1))
                {
                    lower = Utility.HexFromCharToByte(input[i + 1]);
                }

                // OR them together while shifting the upper bits to the left 4 places
                // Ensure endianess is correct.
                if (BitConverter.IsLittleEndian)
                {
                    returnValue[i / 2] = (byte)((upper << 4) | lower);
                    //returnValue[(returnValue.Length - 1) - (i / 2)] = (byte)((upper << 4) | lower);
                }
                else
                {
                    returnValue[i / 2] = (byte)((upper << 4) | lower);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Converts a Hexidecimal character to a byte.
        /// </summary>
        /// <param name="input">Character to be converted.</param>
        /// <returns>Byte value of a hexidecimal character.</returns>
        public static byte HexFromCharToByte(char input)
        {
            byte returnValue = 0x00;

            // If it's an integer (0 - 9), find out which one by dividing by 0's ASCII # (48) and taking the remainder
            if ((input > 47) && (input < 58))
            {
                returnValue = (byte)(input % 48);
            }
            // If it's a lower case letter (a - f), find out its index (i.e. a = 0, b = 1,...) and add 10 to it
            // Divide by a's ASCII # (97) and take the remainder, then add 10 to that remainder
            else if ((input > 96) && (input < 103))
            {
                returnValue = (byte)((input % 97) + 10);
            }
            // If it's an upper case letter (A - F), find out its index (i.e. A = 0, B = 1,...) and add 10 to it
            // Divide by A's ASCII # (65) and take the reaminder, then add 10 to that remainder
            else if ((input > 64) && (input < 71))
            {
                returnValue = (byte)((input % 65) + 10);
            }
            // If none of these conditions matched, an invalid input was provided
            else
            {
                log.Error("Invalid input provided for conversion from hexidecimal character to byte: " + input.ToString());
                throw new FormatException("Invalid character provided for conversion from Hexidecimal character to byte: " + input.ToString());
            }

            // Return the new value
            return returnValue;
        }
    }
}
