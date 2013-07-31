using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObdExpress.Global
{
    /// <summary>
    /// Manages a single connection to the ELM327 used by this application.
    /// </summary>
    public class ELM327Connection
    {
        /// <summary>
        /// Get the logger.
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Notifies listeners when a new connection is established
        /// </summary>
        public static event NoReturnWithSerialPortParam ConnectionEstablishedEvent;

        /// <summary>
        /// Notifies listeners before a connection is closed
        /// </summary>
        public static event NoReturnWithNoParams ConnectionClosingEvent;

        /// <summary>
        /// Notifies listeners when a connection is closed and destroyed
        /// </summary>
        public static event NoReturnWithNoParams ConnectionDestroyedEvent;

        /// <summary>
        /// Only ELM327Connection available
        /// </summary>
        private static ELM327Connection _singleton = new ELM327Connection();

        /// <summary>
        /// The port the ELM327 is connected on.
        /// </summary>
        private SerialPort _connection = null;

        /// <summary>
        /// The SerialPort reference (if any) stored by this static class.
        /// </summary>
        public static SerialPort Connection
        {
            get
            {
                return ELM327Connection._singleton._connection;
            }
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private ELM327Connection()
        {
        }

        /// <summary>
        /// Stores a reference to the SerialPort with the new connection and notifies listeners.
        /// </summary>
        /// <param name="connection">SerialPort with the new connection.</param>
        public static void ConnectionEstablished(SerialPort connection)
        {
            lock (ELM327Connection._singleton._connection)
            {
                ELM327Connection._singleton._connection = connection;
                ELM327Connection.ConnectionEstablishedEvent(connection);
            }
        }

        /// <summary>
        /// Safely closes a connection by notifying listeners before and after the connection is closed.
        /// </summary>
        public static void DestroyConnection()
        {
            lock (ELM327Connection._singleton._connection)
            {
                ELM327Connection.ConnectionClosingEvent();

                if (ELM327Connection._singleton._connection != null && ELM327Connection._singleton._connection.IsOpen)
                {
                    try
                    {
                        ELM327Connection._singleton._connection.Close();
                    }
                    catch (IOException e)
                    {
                        log.Error("Error while attempting to close ELM327 SerialPort connection.", e);
                    }
                }

                ELM327Connection.ConnectionDestroyedEvent();
                ELM327Connection._singleton._connection = null;
            }
        }

    }
}
