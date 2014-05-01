using log4net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        /// Executes a request/response cycle for the specified Handler.
        /// </summary>
        /// <param name="handler">The Handler for whom this Protocol processor will be executing a request/response cycle.</param>
        /// <returns>True if the execution was successful; otherwise, false.</returns>
        public abstract bool Execute(IHandler handler);

        /// <summary>
        /// Allows the ELM327 class to set the SerialPort and Semaphore this Protocol will use to communicate.
        /// </summary>
        /// <param name="connection">SerialPort the connection to the ELM327 device is on.</param>
        /// <param name="connectionSemaphore">Semaphore to be used for controlling access to the SerialPort.</param>
        public void SetConnectionProperties(SerialPort connection, Semaphore connectionSemaphore)
        {
            this.Connection = connection;
            this.ConnectionSemaphore = connectionSemaphore;
        }

        /// <summary>
        /// Executes a single AT command and returns the response. This method prefaces the command with AT, so you do not need to.
        /// </summary>
        /// <param name="command">AT Command to be sent to the ELM327 device. DO NOT preface the command with AT.</param>
        /// <returns>Response from the ELM327 device.</returns>
        protected string ExecuteATCommand(string command)
        {
            string returnValue;

            try
            {
                // Clear the In/Out buffer
                this.Connection.DiscardInBuffer();
                this.Connection.DiscardOutBuffer();

                // Write out our command
                this.Connection.WriteLine(@"AT" + command);

                // Read the response
                returnValue = this.Connection.ReadLine();

                // Check for the > character
                if (returnValue[0] == '>')
                {
                    return returnValue.Substring(1);
                }

                return returnValue;
            }
            catch (Exception e)
            {
                log.Error("Exception thrown while executing an AT command: AT " + command, e);
                return null;
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
                this.Connection.DiscardInBuffer();
                this.Connection.DiscardOutBuffer();

                // Write out our command
                this.Connection.WriteLine(command);
            }
            catch (Exception e)
            {
                log.Error("Exception thrown while executing command: " + command, e);
                return;
            }
        }
    }
}
