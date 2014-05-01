using ELM327API.Connection.Interfaces;
using ELM327API.Global;
using log4net;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ELM327API.Connection.Classes
{
    /// <summary>
    /// An IConnector that connects to a specific port.
    /// </summary>
    public class PortConnector : IConnector
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
        private ConnectionSettings _connectionSettings = null;

        // Port used by this connector
        SerialPort _currentPort = null;

        /// <summary>
        /// Create a PortConnector for the specified port.
        /// </summary>
        /// <param name="portName">The port to create this connector for.</param>
        public PortConnector(string portName, ConnectionSettings connectionSettings)
        {
            this._portName = portName;
            this._connectionSettings = connectionSettings;
        }

        public void GetSerialPort()
        {
            bool success = false;

            // Expected device description
            string deviceDescription = _connectionSettings.DeviceDescription;

            // Actual description
            string receivedDescription = "";

            try
            {
                _currentPort = new SerialPort(this._portName);

                // Tell the user what we're testing
                this.CheckingPort(this._portName);

                // Prepare the port
                _currentPort.BaudRate = _connectionSettings.BaudRate;
                _currentPort.DataBits = _connectionSettings.DataBits;
                _currentPort.Parity = _connectionSettings.Parity;
                _currentPort.StopBits = _connectionSettings.StopBits;
                _currentPort.NewLine = "\r";
                _currentPort.ReadTimeout = 50;
                _currentPort.WriteTimeout = 50;

                // Log the configuration
                PortConnector.log.Info("Checking port " + _currentPort.PortName + " with parameters"
                                            + " BaudRate = " + _currentPort.BaudRate.ToString()
                                            + ", DataBits = " + _currentPort.DataBits.ToString()
                                            + ", Parity = " + _currentPort.Parity.ToString()
                                            + ", StopBits = " + _currentPort.StopBits.ToString()
                                            + ", Device Identifier = " + deviceDescription);

                // Open and attempt a write and read
                PortConnector.log.Info("Opening port...");
                _currentPort.Open();

                // Try to write and read
                try
                {
                    PortConnector.log.Info("Writing [AT D]...");
                    WriteLineDiscardInBuffer(@"AT D");

                    WriteLineDiscardInBuffer(@"");

                    PortConnector.log.Info("Writing [AT L0]...");
                    WriteLineDiscardInBuffer(@"AT L0");

                    PortConnector.log.Info("Writing [AT E0]...");
                    WriteLineDiscardInBuffer(@"AT E0");

                    PortConnector.log.Info("Writing [AT @1] to check Device Identifier...");
                    receivedDescription = DiscardInBufferWriteAndReadExisting(@"AT @1");

                    if (receivedDescription.Length > 0 && receivedDescription[0] == '>') receivedDescription = receivedDescription.Substring(1);
                }
                catch (TimeoutException e)
                {
                    PortConnector.log.Error("TimeoutException has occurred. Assuming no response.", e);
                    this.UpdateMessages("NO RESPONSE!");
                    this.PortSuccess(false);
                }

                // Parse response
                if (receivedDescription.Length > 0)
                {
                    if (receivedDescription.Equals(deviceDescription))
                    {
                        PortConnector.log.Info("Successfully connected on port " + _currentPort.PortName + "!");
                        this.UpdateMessages("SUCCESS!");
                        this.PortSuccess(true);
                        success = true;
                        this.ConnectionEstablished(_currentPort);
                    }
                    else
                    {
                        PortConnector.log.Error("Response to [AT @1] determined to be invalid Device Identifier: " + receivedDescription);
                        this.UpdateMessages("INVALID DEVICE NAME: " + receivedDescription);
                        this.PortSuccess(false);
                        success = false;
                    }
                }
            }
            catch (IOException e)
            {
                PortConnector.log.Error("IOException has occurred. Assuming No Device or Error.", e);
                this.UpdateMessages("NO DEVICE OR ERROR!");
                this.PortSuccess(false);
            }
            catch (InvalidOperationException e)
            {
                PortConnector.log.Error("InvalidOperationException has occurred. Assuming No Device or Error.", e);
                this.UpdateMessages("NO DEVICE OR ERROR!");
                this.PortSuccess(false);
            }
            catch (UnauthorizedAccessException e)
            {
                PortConnector.log.Error("UnauthorizedAccessException has occurred. Assuming No Device or Error.", e);
                this.UpdateMessages("PORT IN USE, ACCESS DENIED!");
                this.PortSuccess(false);
            }

            if (!success)
            {
                this._currentPort.Close();
            }

            this.ConnectionComplete(success);
        }

        /// <summary>
        /// Clear the input buffer, write the output, and read the entire input buffer (new lines included). Then, remove the prompt character and any new line or carriage return characters.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public String DiscardInBufferWriteAndReadExisting(String output)
        {
            _currentPort.DiscardInBuffer();
            _currentPort.WriteLine(output);
            Thread.Sleep(60);
            return _currentPort.ReadExisting().Replace(">", "").Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// Write the output and then discard any input from the port.
        /// </summary>
        /// <param name="output"></param>
        public void WriteLineDiscardInBuffer(String output)
        {
            _currentPort.WriteLine(output);
            Thread.Sleep(60);
            _currentPort.DiscardInBuffer();
        }
        
        /// <summary>
        /// Attempts to safely stop the thread by notifying the loop to return.
        /// </summary>
        public void Kill()
        {
            return;
        }
    }
}
