using ELM327API.Processing.DataStructures;
using System;

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
        /// The body of the request frame (everything after the header). Normally, the SID and PID concatenated together will suffice.
        /// </summary>
        string Request { get; }

        /// <summary>
        /// This property specifies which section of OBD Express each handler should be made available to.
        /// </summary>
        HandlerCategory Category { get; }

        /// <summary>
        /// Does the value retrieved by this handler change? For some values (e.g. VIN number), the answer is no. Setting this property
        /// to FALSE will tell the ELM327 controller to only retrieve this value once each time a listener registers for it. There is no
        /// need to clog the IO bus with queries for values that do not change.
        /// </summary>
        bool IsMutable { get; }

        /// <summary>
        /// The unit(s) of measure for the data being retrieved. It is recommended that this be the abbreviation, as the UI may use this property
        /// as a suffix to any values received from this Handler as they are displayed to the user.
        /// </summary>
        string Unit { get; }

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
        /// Indicates if this is an OBD Diagnostic request.
        /// </summary>
        bool IsOBD { get; }

        /// <summary>
        /// The Service Identifier if this is an OBD message handler.
        /// </summary>
        byte OBDSID { get; }

        /// <summary>
        /// The Parameter Identifier if this is an OBD message handler.
        /// </summary>
        byte OBDPID { get; }

        /// <summary>
        /// Indicates if the Protocol processor should wait for a response to this handler's request.
        /// </summary>
        bool ExpectsResponse { get; }

        /// <summary>
        /// True if this handler has registerd listeners. This is used to add/remove it from the IO queue automatically, as needed.
        /// </summary>
        bool HasRegisteredListeners { get; }

        /// <summary>
        /// True if this handler has registered listeners that only want a single request/response cycle executed.
        /// </summary>
        bool HasRegisteredSingleListeners { get; }

        /// <summary>
        /// Indicates if this handler is compatible with one or all of the protocols.
        /// </summary>
        ProtocolsEnum Compatibility { get; }

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
        /// Processes a repsonse and broadcasts the results to registered listeners.
        /// </summary>
        /// <param name="response">The unprocessed response received from the ELM327.</param>
        void ProcessResponse(byte[] data);

        #endregion
    }
}
