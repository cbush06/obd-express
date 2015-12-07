using ELM327API.Global;
using ELM327API.Processing.Controllers;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;

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
        /// Class for handling IO with the ELM327 device.
        /// </summary>
        private ELM327 _elm327device;

        /// <summary>
        /// The ELM327 device class that manages IO operations with the ELM327.
        /// </summary>
        public static ELM327 ELM327Device
        {
            get
            {
                return ELM327Connection._singleton._elm327device;
            }
        }

        /// <summary>
        /// True if there is currently an active connection to the ELM327 device.
        /// </summary>
        public static bool InOperation
        {
            get
            {
                if ((ELM327Connection._singleton._connection != null && ELM327Connection._singleton._elm327device != null) &&
                    (ELM327Connection._singleton._connection.IsOpen && ELM327Connection._singleton._elm327device.InOperation))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// List of handlers loaded into the application.
        /// </summary>
        private static List<Type> _loadedHandlerTypes = new List<Type>();
        public static List<Type> LoadedHandlerTypes
        {
            get
            {
                return _loadedHandlerTypes;
            }
        }

        /// <summary>
        /// Only ELM327Connection available
        /// </summary>
        private static ELM327Connection _singleton = new ELM327Connection();

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
        public static void ConnectionEstablished(SerialPort connection, ConnectionSettings connectionSettings)
        {
            ELM327Connection._singleton._connection = connection;

            lock (ELM327Connection._singleton._connection)
            {
                ELM327Connection._singleton._elm327device = new ELM327(ELM327Connection._singleton._connection, connectionSettings);
                ELM327Connection._singleton._elm327device.ConnectionLost += ELM327Connection.DestroyConnection;

                // If we could not start operations, stop everything
                if (!(ELM327Connection._singleton._elm327device.StartOperations()))
                {
                    // Log an error
                    log.Error("Error occurred when attempting to start operations on ELM327 device.");

                    // Close connection
                    try
                    {
                        ELM327Connection._singleton._connection.Close();
                    }
                    catch (IOException e)
                    {
                        log.Error("Error occurred while trying to close connection after a failed attempt to start ELM327 operations.", e);
                    }

                    // Clear everything
                    ELM327Connection._singleton._connection = null;
                    ELM327Connection._singleton._elm327device = null;

                    return;
                }

                // If a connection has been successfully established, load our protocol handlers
                // and notify our listeners that the connection is live and read for communication
                if (ELM327Connection.ConnectionEstablishedEvent != null)
                {
                    ELM327Connection._singleton._elm327device.ClearHandlers();

                    foreach (Type nextHandlerType in _loadedHandlerTypes)
                    {
                        ELM327Connection._singleton._elm327device.AddHandler(nextHandlerType);
                    }

                    ELM327Connection.ConnectionEstablishedEvent(connection);
                }
            }
        }

        /// <summary>
        /// Safely closes a connection by notifying listeners before and after the connection is closed.
        /// </summary>
        public static void DestroyConnection()
        {
            if (ELM327Connection._singleton._connection != null && ELM327Connection._singleton._connection.IsOpen)
            {
                lock (ELM327Connection._singleton._connection)
                {
                    // Notify our listeners that the connection is closing
                    if (ELM327Connection.ConnectionClosingEvent != null)
                    {
                        ELM327Connection.ConnectionClosingEvent();
                    }

                    try
                    {
                        // Stop operations
                        ELM327Connection._singleton._elm327device.StopOperations();
                        ELM327Connection._singleton._elm327device = null;
                        ELM327Connection._singleton._connection.Close();
                    }
                    catch (Exception e)
                    {
                        log.Error("Error while attempting to close ELM327 SerialPort connection.", e);
                    }

                    // Notify our listeners that the connection has closed
                    if (ELM327Connection.ConnectionDestroyedEvent != null)
                    {
                        ELM327Connection.ConnectionDestroyedEvent();
                    }

                    ELM327Connection._singleton._connection = null;
                }
            }
        }

    }
}
