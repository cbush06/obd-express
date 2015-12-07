using System;

namespace ELM327API.Processing.DataStructures
{
    /// <summary>
    /// This is a data structure used to represent available ECUs on a connection.
    /// </summary>
    public class ECU
    {
        /// <summary>
        /// The string representation of this ECU.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        private string _name = String.Empty;

        /// <summary>
        /// The Index number identifying this ECU. This is only for ease of reference, as ECUs are not
        /// actually assigned index numbers. In most cases, it is expected that the convention used in
        /// ISO or SAE documents will be followed.
        /// See:
        ///     + ISO 15765-4, Section 6.3.2.2, Table 3
        ///     + SAE J2178-1, Section 9, Table 11
        /// </summary>
        public Int32 Index
        {
            get { return _index;  }
        }
        private Int32 _index = -1;

        /// <summary>
        /// Returns the actual physical address of the ECU on the network to which requests must be sent.
        /// </summary>
        public UInt32 RequestAddress
        {
            get { return _RequestAddress; }
        }
        private UInt32 _RequestAddress = 0x000;

        /// <summary>
        /// Returns the Request Address as a hexidecimal string.
        /// </summary>
        public string RequestAddressString
        {
            get { return _RequestAddress.ToString("X6"); }
        }

        /// <summary>
        /// Returns the actual physical address of the Test Unit on the network to which responses from this ECU will be sent.
        /// </summary>
        public UInt32 ReturnAddress
        {
            get { return _ReturnAddress; }
        }
        private UInt32 _ReturnAddress = 0x000;

        /// <summary>
        /// Returns the Return Address as a hexidecimal string.
        /// </summary>
        public string ReturnAddressString
        {
            get { return _ReturnAddress.ToString("X6"); }
        }

        /// <summary>
        /// Constructs a new ECU data structure to represent an available ECU on a connection.
        /// </summary>
        /// <param name="name">String representation of this ECU.</param>
        /// <param name="index">Index number to be used for ease of reference.</param>
        /// <param name="address">Physical address of this ECU on the network.</param>
        public ECU(string name, Int32 index, UInt32 requestAddress, UInt32 returnAddress)
        {
            _name = name;
            _index = index;
            _RequestAddress = requestAddress;
            _ReturnAddress = returnAddress;
        }
    }
}
