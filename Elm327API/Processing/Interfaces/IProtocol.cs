using ELM327API.Global;
using ELM327API.Processing.DataStructures;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace ELM327API.Processing.Interfaces
{
    /// <summary>
    /// Interface specifying mandatory properties and methods of Protocol classes.
    /// </summary>
    abstract class IProtocol
    {
        /// <summary>
        /// Get the logger.
        /// </summary>
        protected static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Allows the ELM327 class to set the Semaphore this Protocol will use for gaining access to the port.
        /// </summary>
        protected Semaphore ConnectionSemaphore { get; set; }

        /// <summary>
        /// Port the Protocol processor will send and receive on. The ELM327 class will set this.
        /// </summary>
        protected SerialPort Connection { get; set; }

        /// <summary>
        /// Allows the watchdog to stop a long-running execution.
        /// </summary>
        public bool StopExecution { get; set; }

        /// <summary>
        /// Provide a current running time for each request/response cycle to the Watchdog.
        /// </summary>
        public long CurrentRunningTime { get; set; }

        /// <summary>
        /// Event that notifies listeners of the response time of the last request/response cycle. This was added
        /// primarily as a means of updating the Watchdog so he can keep a running average response time.
        /// </summary>
        public abstract event NoReturnWithLongParam BroadcastResponseTime;

        /// <summary>
        /// List of primary ECUs. There is no set minimum or maximum number of these, but
        /// it should include the most common ECUs for the individual protocol being defined.
        /// 
        /// NOTE: The first entry (index 0) should always be the functional/broadcast request address. This 
        ///       entry does not need to provide a response address, only a request address.
        /// </summary>
        public abstract List<ECU> EcuAddresses { get; }

        /// <summary>
        /// This is the ECU selected by the user for the ELM327 to correspond with. This should always
        /// default to <see cref="ELM327API.Global.Constants.NoSelection"/>.
        /// </summary>
        public abstract ECU SelectedEcuFilter { get; set; }

        /// <summary>
        /// Executes a request/response cycle for the specified Handler.
        /// </summary>
        /// <param name="handler">The Handler for whom this Protocol processor will be executing a request/response cycle.</param>
        /// <returns>True if the execution was successful; otherwise, false.</returns>
        public abstract bool Execute(IHandler handler);

        /// <summary>
        /// This method cycles through the list of ECU Addresses (see <see cref="IProtocol.EcuAddresses"/>) and
        /// queries each one to see if it exists. A list of responsive ECUs is returned.
        /// </summary>
        /// <returns>List of ECUs that responded to queries.</returns>
        public abstract List<ECU> AutoDetectEcus();

        /// <summary>
        /// Allows the ELM327 class to set the SerialPort and Semaphore this Protocol will use to communicate.
        /// </summary>
        /// <param name="connection">SerialPort the connection to the ELM327 device is on.</param>
        /// <param name="connectionSemaphore">Semaphore to be used for controlling access to the SerialPort.</param>
        public void SetConnectionProperties(SerialPort connection, Semaphore connectionSemaphore)
        {
            Connection = connection;
            ConnectionSemaphore = connectionSemaphore;
        }

        /// <summary>
        /// Changes the request header set on the ELM327.
        /// </summary>
        /// <param name="header">Request header to set.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        protected bool SetRequestHeader(string header)
        {
            string response = String.Empty;

            // Request the Semaphore
            ConnectionSemaphore.WaitOne();

            // Set header
            response = ExecuteATCommand(@"SH" + header);

            // If we were not successful at setting the header, quit.
            if (!(response.Equals("OK")))
            {
                log.Error("Attempt at Set Headers [AT SH " + header + "] failed. Response: " + response.ToString());
                ConnectionSemaphore.Release();
                return false;
            }

            // Release the Semaphore
            ConnectionSemaphore.Release();

            return true;
        }

        /// <summary>
        /// Executes a single AT command and returns the response. This method prefaces the command with AT, so you do not need to.
        /// </summary>
        /// <param name="command">AT Command to be sent to the ELM327 device. DO NOT preface the command with AT.</param>
        /// <returns>Response from the ELM327 device.</returns>
        protected string ExecuteATCommand(string command)
        {
            Stopwatch stopWatch = new Stopwatch();
            string returnValue = String.Empty;
            int iterator = 0;

            try
            {
                while((returnValue.Equals(String.Empty) || returnValue.Equals(Constants.STOPPED_MESSAGE) || returnValue.Equals("?")) && iterator < Constants.AT_COMMAND_RETRIES) {
                    // Keep count. Try no more than numberOfTries times...
                    iterator++;

                    // Clear the In/Out buffer
                    Connection.DiscardInBuffer();
                    Connection.DiscardOutBuffer();

                    // Write out our command
                    IProtocol.log.Info(@"Attempt [" + iterator.ToString() + "] Sending Command: AT" + command);
                    Connection.WriteLine(@"AT" + command);

                    // Read the response
                    returnValue = Connection.ReadLine();

                    // Log results
                    IProtocol.log.Info(@"Attempt [" + iterator.ToString() + "] Receiving Response: " + returnValue);
                }

                // Check for the > character
                if (returnValue.Length > 0 && returnValue[0] == '>')
                {
                    return returnValue.Substring(1);
                }

                return returnValue;
            }
            catch (Exception e)
            {
                log.Error("Exception thrown while executing an AT command: AT " + command, e);
                return String.Empty;
            }
        }

        /// <summary>
        /// Executes a single command.
        /// </summary>
        /// <param name="command">Command to be sent to the ELM327 device.</param>
        protected void ExecuteCommand(string command)
        {
            try
            {
                // Clear the In/Out buffer
                Connection.DiscardInBuffer();
                Connection.DiscardOutBuffer();

                // Write out our command
                Connection.WriteLine(command);
            }
            catch (Exception e)
            {
                log.Error("Exception thrown while executing command: " + command, e);
                return;
            }
        }
    }
}
