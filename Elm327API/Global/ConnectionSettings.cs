using System;
using System.IO.Ports;

namespace ELM327API.Global
{
    public class ConnectionSettings
    {
        /// <summary>
        /// Baud Rate to use on Serial Port connection.
        /// </summary>
        public int BaudRate
        {
            get { return _baudRate; }
        }
        private int _baudRate = 9600;

        /// <summary>
        /// Data Bits to use on Serial Port connection.
        /// </summary>
        public int DataBits
        {
            get { return _dataBits; }
        }
        private int _dataBits = 8;

        /// <summary>
        /// Parity setting for Serial Port connection.
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
        }
        private Parity _parity = Parity.None;

        /// <summary>
        /// Stop Bits setting for Serial Port connection.
        /// </summary>
        public StopBits StopBits
        {
            get { return _stopBits; }
        }
        private StopBits _stopBits = StopBits.One;

        /// <summary>
        /// Device Description expected from the ELM327 device. If the description it
        /// provides does not match this one, the connection will be rejected.
        /// </summary>
        public string DeviceDescription
        {
            get { return _deviceDescription; }
        }
        private string _deviceDescription = "OBDII to RS232 Interpreter";

        /// <summary>
        /// Create a new ConnectionSettings object passed into the ELM327API to configure the serial port connections used
        /// for communication between OBD Express and the ELM327 device it is connected to.
        /// </summary>
        /// <param name="baudRate">Baud Rate to connect with.</param>
        /// <param name="dataBits">Data Bits used for communication.</param>
        /// <param name="parity">Parity used for verification on communications.</param>
        /// <param name="stopBits">Stop Bits used for communication.</param>
        /// <param name="deviceDescription">Device Description to expect from the ELM327.</param>
        public ConnectionSettings(int baudRate, int dataBits, Parity parity, StopBits stopBits, string deviceDescription)
        {
            _baudRate = baudRate;
            _dataBits = dataBits;
            _parity = parity;
            _stopBits = stopBits;
            _deviceDescription = deviceDescription;
        }
    }
}
