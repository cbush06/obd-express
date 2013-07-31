using ELM327API.Connection.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ELM327API.Connection.Classes
{
    /// <summary>
    /// An IConnector that connects to a specific port.
    /// </summary>
    public class PortConnector : IConnector, IDisposable
    {
        /// <summary>
        /// Get the logger.
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event NoReturnWithStringParam CheckingPort;
        public event NoReturnWithStringParam UpdateMessages;
        public event NoReturnWithBoolParam ConnectionComplete;
        public event NoReturnWithSerialPortParam ConnectionEstablished;
        public event NoReturnWithBoolParam PortSuccess;

        // Port to create a connection for
        private string _portName = "";

        // Port used by this connector
        SerialPort _currentPort = null;

        /// <summary>
        /// Create a PortConnector for the specified port.
        /// </summary>
        /// <param name="portName">The port to create this connector for.</param>
        public PortConnector(string portName)
        {
            this._portName = portName;
        }

        public void GetSerialPort()
        {
            bool success = false;

            // Expected device description
            string deviceDescription = "OBDII to RS232 Interpreter";

            // Actual description
            string receivedDescription = "";

            try
            {
                _currentPort = new SerialPort(this._portName);

                // Tell the user what we're testing
                this.CheckingPort(this._portName);

                // Prepare the port
                _currentPort.BaudRate = 9600;
                _currentPort.DataBits = 8;
                _currentPort.Parity = Parity.None;
                _currentPort.StopBits = StopBits.One;
                _currentPort.NewLine = "\r";
                _currentPort.ReadTimeout = 50;
                _currentPort.WriteTimeout = 50;

                // Open and attempt a write and read
                _currentPort.Open();

                // Try to write and read
                try
                {
                    _currentPort.WriteLine(@"AT E0");
                    Thread.Sleep(40);
                    _currentPort.DiscardInBuffer();

                    _currentPort.WriteLine(@"AT @1");
                    receivedDescription = _currentPort.ReadLine();
                }
                catch (TimeoutException e)
                {
                    Console.Out.WriteLine(e.Message);
                    this.UpdateMessages("NO RESPONSE!");
                    this.PortSuccess(false);
                }

                // Parse response
                if (receivedDescription.Length > 0)
                {
                    if (receivedDescription.Equals(deviceDescription))
                    {
                        this.UpdateMessages("SUCCESS!");
                        this.PortSuccess(true);
                        success = true;
                        this.ConnectionEstablished(_currentPort);
                    }
                }
            }
            catch (IOException e)
            {
                this.UpdateMessages("NO DEVICE OR ERROR!");
                this.PortSuccess(false);
            }
            catch (InvalidOperationException e)
            {
                this.UpdateMessages("NO DEVICE OR ERROR!");
                this.PortSuccess(false);
            }
            catch (UnauthorizedAccessException e)
            {
                this.UpdateMessages("PORT IN USE, ACCESS DENIED!");
                this.PortSuccess(false);
            }

            this.ConnectionComplete(success);
        }


        /// <summary>
        /// Attempts to safely stop the thread by notifying the loop to return.
        /// </summary>
        public void Kill()
        {
            return;
        }

        /// <summary>
        /// Implementation of the IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            if (this._currentPort != null && this._currentPort.IsOpen)
            {
                this._currentPort.Close();
                this._currentPort = null;
            }
        }
    }
}
