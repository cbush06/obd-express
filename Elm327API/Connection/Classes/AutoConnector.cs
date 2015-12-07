using ELM327API.Connection.Interfaces;
using ELM327API.Global;
using log4net;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ELM327API.Connection.Classes
{
    public class AutoConnector : IConnector
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

        // Used to kill the thread
        private bool KeepAlive = true;

        // Port used by this connector
        private SerialPort _currentPort = null;

        // Connection Settings Used by this Connector
        private ConnectionSettings _connectionSettings = null;

        public AutoConnector(ConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
        }

        public void GetSerialPort()
        {
            bool success = false;

            // Reset KeepAlive
            KeepAlive = true;

            // Enumerate the ports
            string[] portNames = SerialPort.GetPortNames();

            // Expected device description
            string deviceDescription = _connectionSettings.DeviceDescription;

            // Actual description
            string receivedDescription = "";

            // Attempt to identify the ELM327 port
            foreach (string portName in portNames)
            {
                try
                {
                    _currentPort = new SerialPort(portName);

                    // Tell the user what we're testing
                    CheckingPort(portName);

                    // Prepare the port
                    _currentPort.BaudRate = _connectionSettings.BaudRate;
                    _currentPort.DataBits = _connectionSettings.DataBits;
                    _currentPort.Parity = _connectionSettings.Parity;
                    _currentPort.StopBits = _connectionSettings.StopBits;
                    _currentPort.NewLine = "\r";
                    _currentPort.ReadTimeout = _connectionSettings.ReadTimeout;
                    _currentPort.WriteTimeout = _connectionSettings.WriteTimout;

                    // Log the configuration
                    AutoConnector.log.Info("Checking port " + portName + " with parameters"
                                            + " BaudRate = " + _currentPort.BaudRate.ToString()
                                            + ", DataBits = " + _currentPort.DataBits.ToString()
                                            + ", Parity = " + _currentPort.Parity.ToString()
                                            + ", StopBits = " + _currentPort.StopBits.ToString()
                                            + ", ReadTimeout = " + _currentPort.ReadTimeout.ToString()
                                            + ", WriteTimeout = " + _currentPort.WriteTimeout.ToString()
                                            + ", Device Identifier = " + deviceDescription);

                    // Open and attempt a write and read
                    AutoConnector.log.Info("Opening port...");
                    _currentPort.Open();

                    // Try to write and read
                    try
                    {
                        AutoConnector.log.Info("Writing [AT D]...");
                        WriteLineDiscardInBuffer(@"AT D");

                        WriteLineDiscardInBuffer(@"");

                        AutoConnector.log.Info("Writing [AT L0]...");
                        WriteLineDiscardInBuffer(@"AT L0");

                        AutoConnector.log.Info("Writing [AT E0]...");
                        WriteLineDiscardInBuffer(@"AT E0");

                        AutoConnector.log.Info("Writing [AT @1] to check Device Identifier...");
                        receivedDescription = DiscardInBufferWriteAndReadExisting(@"AT @1");

                        if (receivedDescription.Length > 0 && receivedDescription[0] == '>') receivedDescription = receivedDescription.Substring(1);

                    }
                    catch (TimeoutException e)
                    {
                        AutoConnector.log.Error("TimeoutException has occurred. Assuming no response.", e);
                        UpdateMessages("NO RESPONSE!");
                        PortSuccess(false);
                    }

                    // Parse response
                    if (receivedDescription.Length > 0)
                    {
                        if (receivedDescription.Equals(deviceDescription))
                        {
                            AutoConnector.log.Info("Successfully connected on port " + portName + "!");
                            UpdateMessages("SUCCESS!");
                            PortSuccess(true);
                            success = true;
                            ConnectionEstablished(_currentPort);
                            break;
                        }
                        else
                        {
                            AutoConnector.log.Error("Response to [AT @1] determined to be invalid Device Identifier: " + receivedDescription);
                            UpdateMessages("INVALID DEVICE NAME: " + receivedDescription);
                            PortSuccess(false);
                            success = false;
                            break;
                        }
                    }

                    // Close the port
                    _currentPort.Close();
                }
                catch (IOException e)
                {
                    AutoConnector.log.Error("IOException has occurred. Assuming No Device or Error.", e);
                    UpdateMessages("NO DEVICE OR ERROR!");
                    PortSuccess(false);
                }
                catch (InvalidOperationException e)
                {
                    AutoConnector.log.Error("InvalidOperationException has occurred. Assuming No Device or Error.", e);
                    UpdateMessages("NO DEVICE OR ERROR!");
                    PortSuccess(false);
                }
                catch (UnauthorizedAccessException e)
                {
                    AutoConnector.log.Error("UnauthorizedAccessException has occurred. Assuming No Device or Error.", e);
                    UpdateMessages("PORT IN USE, ACCESS DENIED!");
                    PortSuccess(false);
                }

                // Allow us to elegantly exit the thread
                if (!KeepAlive)
                {
                    AutoConnector.log.Info("KeepAlive was false. Exiting thread...");

                    // Close the port
                    if (_currentPort.IsOpen)
                    {
                        _currentPort.Close();
                    }

                    return;
                }

            }

            if (!success)
            {
                _currentPort.Close();
            }

            ConnectionComplete(success);
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
            KeepAlive = false;
        }
    }
}
