using ELM327API.Global;
using ELM327API.Processing.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM327API.Processing.Interfaces
{
    /// <summary>
    /// Interface specifying the required fields for an ELM327 Handler.
    /// </summary>
    public interface IHandler
    {
        #region Properties
        
        /// <summary>
        /// String name for the Handler. This should be used to identify the data being requested and processed by this handler (e.g. Speed, 
        /// Coolant Temperature, RPM, etc.) Ensure the name is self-explanatory because this may also be used to identify this Handler to the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The body of the request frame (everything after the header).
        /// </summary>
        string Request { get; }

        /// <summary>
        /// The unit(s) of measure for the data being retrieved. It is recommended that this be the abbreviation, as the UI may use this property
        /// as a suffix to any values received from this Handler as they are displayed to the user.
        /// </summary>
        string Units { get; }

        /// <summary>
        /// Specifies the datatype of the response after it has been processed by this handler. Components of the application that analyze inputs
        /// from multiple handlers may need to determine the datatype of each handler's response.
        /// </summary>
        Type DataType { get; }

        /// <summary>
        /// True if this Handler retrieves a non-OBD value from the databus.
        /// </summary>
        bool IsCustomHeader { get; }

        /// <summary>
        /// If this Handler retrieves a non-OBD value, it must provide the address of the component it is addressing the response to.
        /// </summary>
        string Header { get; }

        /// <summary>
        /// True if this handler has registerd listeners. This is used to add/remove it from the IO queue automatically, as needed.
        /// </summary>
        bool HasRegisteredListeners { get; }

        /// <summary>
        /// True if this handler has registered listeners that only want a single request/response cycle executed.
        /// </summary>
        bool HasRegisteredSingleListeners { get; }

        /// <summary>
        /// True if this handler has been enqueued to execute only one request/response cycle and then quit. False, if
        /// this handler has been enqueued  to execute the request/response cycle as long as it has registered listeners.
        /// </summary>
        bool IsRealtime { get; set; }

        /// <summary>
        /// Indicates if this handler is compatible with one or all of the protocols.
        /// </summary>
        Protocols Compatibility { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Registers a listener to be called when data is received and processed by this handler.
        /// </summary>
        /// <param name="callback">The method to be added.</param>
        void RegisterListener( Action<ELM327ListenerEventArgs> callback );

        /// <summary>
        /// Unregisters a listener so it is no longer called when data is received and processed by this handler.
        /// </summary>
        /// <param name="callback">The method to be removed.</param>
        void UnregisterListener( Action<ELM327ListenerEventArgs> callback );

        /// <summary>
        /// Registers a listener to be called only once with data received from the next request made by this handler.
        /// </summary>
        /// <param name="callback">The method to be called.</param>
        void RegisterSingleListener(Action<ELM327ListenerEventArgs> callback);

        /// <summary>
        /// Determines if a callback is already a registered listener of this Handler. For this to return true, the callback must be a reference
        /// to the same method as the callback already registered on this Handler.
        /// </summary>
        /// <param name="callback">The listener in question.</param>
        /// <returns>True if the listener is already registered; otherwise, false.</returns>
        bool IsListenerRegistered( Action<ELM327ListenerEventArgs> callback );

        /// <summary>
        /// Processes a repsonse and broadcasts the results to registered listeners.
        /// </summary>
        /// <param name="response">The unprocessed response received from the ELM327.</param>
        void ProcessResponse( string response );

        #endregion
    }
}
