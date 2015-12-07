using ELM327API.Global;
using ELM327API.Processing.DataStructures;
using ELM327API.Processing.Interfaces;
using ELM327API.Processing.Protocols;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace ELM327API.Processing.Controllers
{
    public class ELM327
    {
        #region Private Members

        /// <summary>
        /// Get the logger.
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Queue of Handlers to be monitored.
        /// </summary>
        private ConcurrentQueue<IHandler> _monitoredHandlers = new ConcurrentQueue<IHandler>();

        /// <summary>
        /// Dictionary of Handlers keyed by their names.
        /// </summary>
        private ConcurrentDictionary<string, HandlerWrapper> _loadedHandlers = new ConcurrentDictionary<string, HandlerWrapper>();

        /// <summary>
        /// SerialPort with the ELM327 connection on it.
        /// </summary>
        private SerialPort _connection = null;

        /// <summary>
        /// Connection Settings used for the Serial Port connection with the ELM327 device.
        /// </summary>
        private ConnectionSettings _connectionSettings = null;

        /// <summary>
        /// Protocol used for parsing incoming responses and preparing outgoing requests.
        /// </summary>
        private IProtocol _protocol = null;

        /// <summary>
        /// Thread for executing IO operations on.
        /// </summary>
        private Thread _ioThread;

        /// <summary>
        /// Thread for executing Watchdog on.
        /// </summary>
        private Thread _watchdogThread;

        /// <summary>
        /// Used for safely exiting IO Thread
        /// </summary>
        private bool _continueRunningIOThread = true;

        /// <summary>
        /// Used for safely exiting Watchdog Thread
        /// </summary>
        private bool _continueRunningWatchdogThread = true;

        /// <summary>
        /// Semaphore used for controlling access to the ELM327.
        /// </summary>
        private Semaphore _semaphore = new Semaphore(1, 1);

        #endregion

        #region Public and Protected Members
        /// <summary>
        /// Event called when the Watchdog detects that the connection was lost or if no response was received for a certain amount of time.
        /// </summary>
        public event NoReturnWithNoParam ConnectionLost;

        /// <summary>
        /// The version identification string retrieved from the ELM327 device.
        /// </summary>
        private string _deviceVersion = "";
        public string DeviceVersion
        {
            get { return _deviceVersion; }
        }

        /// <summary>
        /// Gets and sets the ECU that requests will be addressed to and that responses will be filtered for.
        /// </summary>
        private ECU _selectedEcuFilter = Constants.NoSelection;
        public ECU SelectedEcuFilter
        {
            get { return _selectedEcuFilter; }
            set { _selectedEcuFilter = value; }
        }

        /// <summary>
        /// The protocol used by the vehicle. This is determined during the StartOperations() cycle.
        /// </summary>
        private ProtocolsEnum _vehicleProtocol = ProtocolsEnum.AUTO;
        public ProtocolsEnum VehicleProtocol
        {
            get { return _vehicleProtocol; }
        }

        /// <summary>
        /// True if IO operations are currently taking place.
        /// </summary>
        private bool _inOperation = false;
        public bool InOperation
        {
            get { return _inOperation; }
        }

        /// <summary>
        /// Port the ELM327 is currently connected on.
        /// </summary>
        public SerialPort ConnectedPort
        {
            get { return _connection; }
        }

        /// <summary>
        /// List of ECUs that were auto-detected.
        /// </summary>
        private List<ECU> _responsiveEcus = new List<ECU>();
        public List<ECU> ResponsiveEcus
        {
            get { return _responsiveEcus; }
        }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ELM327(SerialPort connection, ConnectionSettings connectionSettings)
        {
            // Store the connection
            _connection = connection;

            // Store the settings
            _connectionSettings = connectionSettings;
        }

        /// <summary>
        /// Executes a single AT command and returns the response. This method prefaces the command with AT, so you do not need to.
        /// </summary>
        /// <param name="command">AT Command to be sent to the ELM327 device. DO NOT preface the command with AT.</param>
        /// <returns>Response from the ELM327 device.</returns>
        private string ExecuteATCommand(String command)
        {
            string returnValue;

            try
            {
                // Discard in buffer
                _connection.DiscardInBuffer();

                // Write out our command and get the response
                returnValue = DiscardInBufferWriteAndReadExisting(@"AT " + command);

                return returnValue;
            }
            catch (Exception e)
            {
                log.Error("Exception thrown while executing an AT command.", e);
                return null;
            }
        }

        /// <summary>
        /// Clear the input buffer, write the output, and read the entire input buffer (new lines included). Then, remove the prompt character and any new line or carriage return characters.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public String DiscardInBufferWriteAndReadExisting(String output)
        {
            _connection.DiscardInBuffer();
            _connection.WriteLine(output);
            Thread.Sleep(60);
            return _connection.ReadExisting().Replace(">", "").Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// Prepares the ELM327 device for operation by setting various parameters to control formatting of requests/responses and optimize
        /// communications between the device and this application.
        /// </summary>
        private bool PrepareELM327()
        {
            // Variables
            StringBuilder sb = new StringBuilder(10);
            short protocolNumber = 0;

            // Ensure the connection is valid
            if(_connection != null && _connection.IsOpen)
            {
                // Prepare the port
                _connection.BaudRate = _connectionSettings.BaudRate;
                _connection.DataBits = _connectionSettings.DataBits;
                _connection.Parity = _connectionSettings.Parity;
                _connection.StopBits = _connectionSettings.StopBits;
                _connection.NewLine = "\r";
                _connection.ReadTimeout = 100;
                _connection.WriteTimeout = 50;

                /*
                 * Customize settings for speed and function
                 */
                // Turn response spaces off
                sb.Append(ExecuteATCommand(@"S0"));
                if (!(sb.ToString().Equals(@"OK")))
                {
                    log.Error("Attempt at RESPONSE SPACES OFF [AT S0] failed. Value returned: " + sb.ToString());
                    return false;
                }
                sb.Clear();

                // Turn linefeeds off
                //sb.Append(this.ExecuteATCommand(@"L0"));
                //if (!(sb.ToString().Equals(@"OK")))
                //{
                //    log.Error("Attempt at LINEFEEDS OFF [AT L0] failed. Value returned: " + sb.ToString());
                //    return false;
                //}
                //sb.Clear();

                // Echo Off
                sb.Append(ExecuteATCommand(@"E0"));
                if (!(sb.ToString().Equals(@"OK")))
                {
                    log.Error("Attempt at ECHO OFF [AT E0] failed. Value returned: " + sb.ToString());
                    return false;
                }
                sb.Clear();

                // Get Device Description
                sb.Append(ExecuteATCommand(@" @1"));
                if (!(sb.ToString().Equals(_connectionSettings.DeviceDescription)))
                {
                    log.Error("Attempt at DEVICE DESCRIPTION [AT @1] failed. Value returned: " + sb.ToString());
                    return false;
                }
                sb.Clear();

                // Get Version
                sb.Append(ExecuteATCommand(@"I"));
                if (sb.Length < 11)
                {
                    log.Error("Attempt at IDENTIFY YOURSELF [AT I] failed. Value returned: " + sb.ToString());
                    return false;
                }
                sb.Clear();

                // Set Headers on
                sb.Append(ExecuteATCommand(@"H1"));
                if (!(sb.ToString().Equals(@"OK")))
                {
                    log.Error("Attempt at HEADERS ON [AT H1] failed. Value returned: " + sb.ToString());
                    return false;
                }
                sb.Clear();

                /*
                // Set Data Length bytes on
                sb.Append(this.ExecuteATCommand(@"D1"));
                if (!(sb.ToString().Equals(@"OK")))
                {
                    log.Error("Attempt at DATA LENGTH BYTES ON [AT D1] failed. Value returned: " + sb.ToString());
                    return false;
                }
                sb.Clear();
                */

                // Determine the protocol used by the vehicle
                sb.Append(ExecuteATCommand(@"DPN"));
                if (sb.Length < 1 || sb.Length > 2)
                {
                    log.Error("Attempt at DETERMINE PROTOCOL NUMBER [AT DPN] failed. Value returned: " + sb.ToString());
                    return false;
                }
                else
                {
                    // If the device is in auto mode, it prefixes the protocol number with an 'A'
                    if (sb[0] == 'A')
                    {
                        protocolNumber = 1;
                    }
                    
                    // Is the character in the range 0 - 9 or A - C
                    if ((sb[protocolNumber] > 47 && sb[protocolNumber] < 58) || (sb[protocolNumber] > 64 && sb[protocolNumber] < 68))
                    {
                        _vehicleProtocol = (ProtocolsEnum)Convert.ToInt32(sb[protocolNumber].ToString(), 16);
                    }
                    else
                    {
                        log.Error("Attempt at DETERMINE PROTOCOL NUMBER [AT DPN] failed. Value returned: " + sb.ToString());
                        return false;
                    }
                }
                sb.Clear();

                // We're done!
                return true;
            }

            // Connection is either null or not open
            return false;
        }



        /// <summary>
        /// This method cycles through the list of ECUs for the selected protocol attempting
        /// to contact each one using the Unified Diagnostic Services (ISO 14229).
        /// </summary>
        public List<ECU> AutoDetectEcus()
        {
            List<ECU> responsiveEcus = _protocol.AutoDetectEcus();

            foreach(ECU nextEcu in responsiveEcus)
            {
                log.Info(nextEcu.Index + ": " + nextEcu.Name);
            }

            return responsiveEcus;
        }

        /// <summary>
        /// Creates a new Thread that handles processing of Input/Output with the ELM327.
        /// </summary>
        private void SpawnIOThread()
        {
            // Variables
            IHandler currentHandler;
            string buffer = "";

            // Log starting of IO Thread
            log.Info("Starting ELM327 IO Thread...");

            // Main Loop
            while (_continueRunningIOThread)
            {
                // Get the next handler
                if (!(_monitoredHandlers.IsEmpty) && (_monitoredHandlers.TryDequeue(out currentHandler)))
                {
                    // Set the ECU filter
                    _protocol.SelectedEcuFilter = _selectedEcuFilter;

                    // Have the current Protocol handler execute this handler's request
                    _protocol.Execute(currentHandler);

                    // If this handler still has registered listeners after its execution, enqueue it again
                    if (currentHandler.HasRegisteredListeners || currentHandler.HasRegisteredSingleListeners)
                    {
                        _monitoredHandlers.Enqueue(currentHandler);
                    }
                }
            }

            // Log exiting of IO Thread
            log.Info("Exiting ELM327 IO Thread...");
        }

        /// <summary>
        /// Attempts to safely stop the IO Thread.
        /// </summary>
        private void KillIOThread()
        {
            _continueRunningIOThread = false;
        }

        /// <summary>
        /// Creates a new Thread that monitors the connection to the ELM327 and its connection to the Vehicle.
        /// </summary>
        private void SpawnWatchdogThread()
        {
        }

        /// <summary>
        /// Attempts to safely stop the Watchdog Thread.
        /// </summary>
        private void KillWatchdogThread()
        {
            _continueRunningWatchdogThread = false;
        }

        /// <summary>
        /// This method starts all necessary operations to make the ELM327 API ready to accept requests and deliver responses. It does this
        /// by preparing the ELM 327 device, starting the IO thread, and, then, starting the Watchdog Thread.
        /// </summary>
        public bool StartOperations()
        {
            // Prepare the device
            if (!(PrepareELM327()))
            {
                log.Error("Error occurred while attempting to Prepare the ELM 327 device.");
                return false;
            }

            // Set the correct protocol
            switch (_vehicleProtocol)
            {
                case ProtocolsEnum.ISO_15765_4_CAN_11_BIT_ID_250:
                case ProtocolsEnum.ISO_15765_4_CAN_11_BIT_ID_500:
                case ProtocolsEnum.USER1_CAN_11_BIT_ID_125:
                case ProtocolsEnum.USER2_CAN_11_BIT_ID_50:
                    {
                        _protocol = new CAN_11_Bit_ISO15765_Protocol();
                        break;
                    }
                default:
                    {
                        log.Error("Encountered unsupported vehicle protocol while attempting to Start ELM 327 Operations: " + _vehicleProtocol.ToString());
                        return false;
                    }
            }

            // Prepare the protocol
            _protocol.SetConnectionProperties(_connection, _semaphore);

            // Detect ECUs
            _responsiveEcus = AutoDetectEcus();

            // Start the IO thread
            _ioThread = new Thread(new ThreadStart(SpawnIOThread));
            _continueRunningIOThread = true;
            try
            {
                _ioThread.Start();
            }
            catch (Exception e)
            {
                log.Error("Exception occurred while attempting to start IO Thread for ELM 327 operations.", e);
                return false;
            }

            // Indicate we are in operation
            _inOperation = true;

            return true;
        }

        /// <summary>
        /// This method stops all operations of the ELM327 API. It kills the Watchdog Thread, stops the IO thread, and resets the ELM 327 device.
        /// </summary>
        public void StopOperations()
        {
            // Stop the IO and Watchdog Threads
            _continueRunningWatchdogThread = false;
            _continueRunningIOThread = false;

            // Reset the ELM 327 device
            ExecuteATCommand(@"Z");

            // Indicate we are not in operation
            _inOperation = false;
        }

        /// <summary>
        /// Adds a handler to the list of available handlers. This is a necessary step before accepting and/or fulfilling any
        /// requests/responses that are processed by this handler.
        /// </summary>
        /// <param name="handlerType">The Type metaclass for the Handler to be added.</param>
        /// <returns>True if the Handler is successfully added; otherwise, false.</returns>
        public bool AddHandler(Type handlerType)
        {
            HandlerWrapper newHandlerWrapper = null;

            // Ensure the type provided is actually an IHandler type
            if (typeof(IHandler).IsAssignableFrom(handlerType))
            {
                // Create a new HandlerWrapper of the provided type
                try
                {
                    newHandlerWrapper = new HandlerWrapper(handlerType);
                }
                catch (Exception e)
                {
                    log.Error("An exception occurred while attempting to create a HandlerWrapper object from Type [" + handlerType.AssemblyQualifiedName + "].", e);
                    return false;
                }

                // Ensure the Handler does not already exist in the handler dictionary and add it
                if(_loadedHandlers.ContainsKey(handlerType.Name) || !(_loadedHandlers.TryAdd(handlerType.Name, newHandlerWrapper)))
                {
                    log.Error("An attempt was made to add a preexisting Handler to the Dictionary of loaded handlers.");
                    return false;
                }

                // If we made it here, we were successful
                return true;
            }
            else
            {
                // Log a failed attempt
                log.Error("An attempt was made to add an object as a Handler that did not implement the IHandler interface.");
            }
            return false;
        }

        /// <summary>
        /// Removes a handler from the list of available handlers. This will preclude any future requests/responses from being
        /// accepted and/or fulfilled that require this handler for processing.
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public bool RemoveHandler(Type handlerType)
        {
            HandlerWrapper oldHandlerWrapper = null;

            // Ensure the type provided is actually an IHandler type
            if (typeof(IHandler).IsAssignableFrom(handlerType))
            {
                // Ensure the Handler does not already exist in the handler dictionary and add it
                if(!(_loadedHandlers.ContainsKey(handlerType.Name)) || !(_loadedHandlers.TryRemove(handlerType.Name, out oldHandlerWrapper)))
                {
                    log.Error("An attempt was made to remove a Handler from the Dictionary of loaded handlers: " + handlerType.Name);
                    return false;
                }

                // If we made it here, we were successful
                return true;
            }
            else
            {
                // Log a failed attempt
                log.Error("An attempt was made to remove a Handler that did not implement the IHandler interface.");
            }
            return false;
        }

        /// <summary>
        /// Removes all handlers from the list of available handlers. This will preclude any future requests/responses from being
        /// accepted and/or fulfilled.
        /// </summary>
        public void ClearHandlers()
        {
            _loadedHandlers.Clear();
        }

        /// <summary>
        /// Adds a listener to the handler identified by the name provided. Callback will receive any incoming data processed by the Handler. This method also controls
        /// whether handlers are repeatedly queued up for IO operations. If a handler has at least one listener, it will be enqueued.
        /// </summary>
        /// <param name="handlerName">Name of the Handler to listen to.</param>
        /// <param name="callback">Method receiving an ELM327ListenerEventArgs object containing any incoming data processed by the Handler.</param>
        /// <returns>True if the Listener is successfully registered; otherwise, false.</returns>
        public bool RegisterListener(string handlerName, Action<ELM327ListenerEventArgs> callback)
        {
            // Variables
            HandlerWrapper affectedHandler = null;

            // Find a Handler with the provided name
            if (!(_loadedHandlers.TryGetValue(handlerName, out affectedHandler)))
            {
                log.Error("An attempt was made to register a listener on a handler that could not be found using the provided name: " + handlerName);
                return false;
            }

            // If the handler already has listeners, we know its already enqueued with the ELM327 controller; however,
            // if it doesn't already have listeners, we need to enqueue the handler along with registering the listener.
            if (affectedHandler.Handler.HasRegisteredListeners)
            {
                // If the handler is mutable, register the listener normally. If it's
                // immutable, register the listener as a single listener
                if (affectedHandler.Handler.IsMutable)
                {
                    affectedHandler.Handler.RegisterListener(callback);
                }
                else
                {
                    affectedHandler.Handler.RegisterSingleListener(callback);
                }
            }
            else
            {
                // If the handler is mutable, register the listener normally. If it's
                // immutable, register the listener as a single listener
                if (affectedHandler.Handler.IsMutable)
                {
                    affectedHandler.Handler.RegisterListener(callback);
                }
                else
                {
                    affectedHandler.Handler.RegisterSingleListener(callback);
                }

                // Enqueue the handler with the ELM327 controller for processing
                _monitoredHandlers.Enqueue(affectedHandler.Handler);
            }

            return true;
        }

        /// <summary>
        /// Removes a listener from the handler identified by the name provided.
        /// </summary>
        /// <param name="handlerName">The Handler to remove the listener from.</param>
        /// <param name="callback">The listener to be removed.</param>
        /// <returns>True if the Listener is successfully removed; otherwise, false.</returns>
        public bool UnregisterListener(string handlerName, Action<ELM327ListenerEventArgs> callback)
        {
            // Variables
            HandlerWrapper affectedHandler = null;

            // Find a Handler with the provided name
            if (!(_loadedHandlers.TryGetValue(handlerName, out affectedHandler)))
            {
                log.Error("An attempt was made to unregister a listener from a handler that could not be found using the provided name: " + handlerName);
                return false;
            }

            // If we made it here, we know that the callback is registered on the handler and we can proceed with removing it
            affectedHandler.Handler.UnregisterListener(callback);

            return true;
        }

        /// <summary>
        /// Registers a listener on the handler for one request only. After it has completed one request/response cycle, the Listener is removed from the
        /// handler and the response is returned to the listener via the callback. The callback method will receive the response in an ELM327ListenerEventArgs object.
        /// </summary>
        /// <param name="handlerName">The name of the Handler to use for the request/response cycle.</param>
        /// <param name="callback">The method that will be called and provided an ELM327ListenerEventArgs object after the Handler has received and processed the response.</param>
        public void AsyncQuery(string handlerName, Action<ELM327ListenerEventArgs> callback)
        {
            // Variables
            HandlerWrapper handlerWrapper = null;

            // Find a Handler with the provided name
            if (!(_loadedHandlers.TryGetValue(handlerName, out handlerWrapper)))
            {
                log.Error("An attempt was made to enqueue a handler for an AsyncQuery that could not be found using the provided name: " + handlerName);
                return;
            }

            // Add the listener to the RegisteredSingleListeners of this Handler
            handlerWrapper.Handler.RegisterSingleListener(callback);

            // Enqueue this Handler to ensure the request is fulfilled
            _monitoredHandlers.Enqueue(handlerWrapper.Handler);
        }

        /// <summary>
        /// Registers a listener on the handler for one request only. After it has completed the request/response cycle, the listener is removed from the handler
        /// and the response is returned from this method. This method blocks the calling thread until it returns; therefore, this is a synchronous call.
        /// </summary>
        /// <param name="handlerName">The name of the Handler to use for the request/response cycle.</param>
        /// <returns>The processed results.</returns>
        public ELM327ListenerEventArgs SyncQuery(string handlerName)
        {
            // Variables
            HandlerWrapper handlerWrapper = null;
            ELM327ListenerEventArgs returnValue = null;
            Stopwatch stopWatch = new Stopwatch();

            // Find a Handler with the provided name
            if (!(_loadedHandlers.TryGetValue(handlerName, out handlerWrapper)))
            {
                log.Error("An attempt was made to enqueue a handler for an AsyncQuery that could not be found using the provided name: " + handlerName);
                return null;
            }

            // Add an anonymous listener that gets the value for us
            handlerWrapper.Handler.RegisterSingleListener(
                new Action<ELM327ListenerEventArgs>(
                    (ELM327ListenerEventArgs args) => {
                        returnValue = args;
                    }
                )
            );

            // Wait no more than 100 milliseconds for a response
            stopWatch.Start();
            while (stopWatch.ElapsedMilliseconds < 100 && returnValue == null) ;
            stopWatch.Stop();

            return returnValue;
        }
    }
}
