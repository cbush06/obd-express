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
    public class AutoConnector : IConnector, IDisposable
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

        public void GetSerialPort()
        {
            bool success = false;

            // Reset KeepAlive
            this.KeepAlive = true;

            // Enumerate the ports
            string[] portNames = SerialPort.GetPortNames();

            // Expected device description
            string deviceDescription = "OBDII to RS232 Interpreter";

            // Actual description
            string receivedDescription = "";

            // Attempt to identify the ELM327 port
            foreach (string portName in portNames)
            {
                try
                {
                    _currentPort = new SerialPort(portName);

                    // Tell the user what we're testing
                    log.Info("AutoConnector is checking port: " + portName);
                    this.CheckingPort(portName);

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
                            break;
                        }
                    }

                    // Close the port
                    _currentPort.Close();
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

                // Allow us to elegantly exit the thread
                if (!this.KeepAlive)
                {
                    log.Info("KeepAlive was false. Exiting thread...");

                    // Close the port
                    if (_currentPort.IsOpen)
                    {
                        _currentPort.Close();
                    }

                    return;
                }

            }

            this.ConnectionComplete(success);
        }
        
        /// <summary>
        /// Attempts to safely stop the thread by notifying the loop to return.
        /// </summary>
        public void Kill()
        {
            this.KeepAlive = false;
        }

        /// <summary>
        /// Implementation of the IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            if (this._currentPort != null && this._currentPort.IsOpen)
            {
                this.Kill();
                Thread.Sleep(1);
                this._currentPort.Close();
                this._currentPort = null;
            }
        }
    }
}
