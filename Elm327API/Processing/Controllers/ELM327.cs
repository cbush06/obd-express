using ELM327API.Processing.DataStructures;
using ELM327API.Processing.Interfaces;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        /// Protocol used for parsing incoming responses and preparing outgoing requests.
        /// </summary>
        private IProtocol _protocol = null;

        #endregion

        #region Public and Protected Members

        /// <summary>
        /// Semaphore used for controlling access to the ELM327.
        /// </summary>
        protected Semaphore ConnectionSemaphore = new Semaphore(1, 1);

        /// <summary>
        /// Event called when the Watchdog detects that the connection was lost or if no response was received for a certain amount of time.
        /// </summary>
        public event NoReturnWithNoParam ConnectionLost;

        private string _deviceId = "";
        public string DeviceId
        {
            get
            {
                return this._deviceId;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ELM327()
        {
        }

        /// <summary>
        /// Prepares the ELM327 device for operation by setting various parameters to control formatting of requests/responses and optimize
        /// communications between the device and this application.
        /// </summary>
        private void PrepareELM327()
        {
            
        }

        /// <summary>
        /// Creates a new Thread that handles processing of Input/Output with the ELM327.
        /// </summary>
        private void SpawnIOThread()
        {
        }

        /// <summary>
        /// Attempts to safely stop the IO Thread.
        /// </summary>
        private void KillIOThread()
        {
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
        }

        /// <summary>
        /// This method starts all necessary operations to make the ELM327 API ready to accept requests and deliver responses.
        /// It does this by starting the IO Thread, starting the Watchdog Thread, and, then, preparing the ELM327 device.
        /// </summary>
        public void StartOperations()
        {
        }

        /// <summary>
        /// This method stops all operations of the ELM327 API. It resets the ELM327 device, kills the Watchdog Thread, 
        /// and, then, kills the IO Thread.
        /// </summary>
        public void StopOperations()
        {
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
                if(this._loadedHandlers.ContainsKey(newHandlerWrapper.Name) || !(this._loadedHandlers.TryAdd(newHandlerWrapper.Name, newHandlerWrapper)))
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
            HandlerWrapper newHandlerWrapper = null;
            HandlerWrapper oldHandlerWrapper = null;

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
                if(!(this._loadedHandlers.ContainsKey(newHandlerWrapper.Name)) || !(this._loadedHandlers.TryRemove(newHandlerWrapper.Name, out oldHandlerWrapper)))
                {
                    log.Error("An attempt was made to remove a Handler from the Dictionary of loaded handlers: " + newHandlerWrapper.Name);
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
            if (!(this._loadedHandlers.TryGetValue(handlerName, out affectedHandler)))
            {
                log.Error("An attempt was made to register a listener on a handler that could not be found using the provided name: " + handlerName);
                return false;
            }

            // Ensure this callback is not already in the handler's event's invocation list
            if (affectedHandler.Handler.IsListenerRegistered(callback))
            {
                log.Error("An attempt was made to register a listener on a handler on which it is already registered. Listener: " + callback.Method.Name + "; Handler: " + handlerName);
                return false;
            }

            // If we made it here, we know that the callback is not already registered on the handler and we can proceed with adding it
            if (affectedHandler.Handler.HasRegisteredListeners)
            {
                affectedHandler.Handler.RegisterListener(callback);
            }
            else
            {
                affectedHandler.Handler.RegisterListener(callback);
                this._monitoredHandlers.Enqueue(affectedHandler.Handler);
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
            if (!(this._loadedHandlers.TryGetValue(handlerName, out affectedHandler)))
            {
                log.Error("An attempt was made to unregister a listener from a handler that could not be found using the provided name: " + handlerName);
                return false;
            }

            // Ensure this callback is already in the handler's event's invocation list
            if (!(affectedHandler.Handler.IsListenerRegistered(callback)))
            {
                log.Error("An attempt was made to unregister a listener from a handler on which it not previously registered. Listener: " + callback.Method.Name + "; Handler: " + handlerName);
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
            if (!(this._loadedHandlers.TryGetValue(handlerName, out handlerWrapper)))
            {
                log.Error("An attempt was made to enqueue a handler for an AsyncQuery that could not be found using the provided name: " + handlerName);
                return;
            }

            // Add the listener to the RegisteredSingleListeners of this Handler
            handlerWrapper.Handler.RegisterSingleListener(callback);

            // Enqueue this Handler to ensure the request is fulfilled
            this._monitoredHandlers.Enqueue(handlerWrapper.Handler);
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
            if (!(this._loadedHandlers.TryGetValue(handlerName, out handlerWrapper)))
            {
                log.Error("An attempt was made to enqueue a handler for an AsyncQuery that could not be found using the provided name: " + handlerName);
                return null;
            }

            // Add an anonymous listener that gets the value for us
            handlerWrapper.Handler.RegisterSingleListener(
                new Action<ELM327ListenerEventArgs>((ELM327ListenerEventArgs args) =>
                    {
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
