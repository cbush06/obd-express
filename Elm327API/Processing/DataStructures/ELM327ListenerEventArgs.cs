using ELM327API.Processing.Interfaces;
using System;

namespace ELM327API.Processing.DataStructures
{
    public class ELM327ListenerEventArgs
    {
        /// <summary>
        /// Reference to the Handler that generated this event.
        /// </summary>
        private IHandler _handler = null;
        public IHandler Handler
        {
            get { return _handler; }
        }

        /// <summary>
        /// Name of the handler that generated this event.
        /// </summary>
        public string HandlerName
        {
            get
            {
                if (_handler != null)
                {
                    return _handler.Name;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Reference to the processed form of the received response.
        /// </summary>
        private object _processedResponse = null;
        public object ProcessedResponse
        {
            get { return _processedResponse; }
        }

        /// <summary>
        /// Indicates if any errors were detected in the response.
        /// </summary>
        private bool _isBadResponse = false;
        public bool IsBadResponse
        {
            get { return _isBadResponse; }
        }

        /// <summary>
        /// Description of any errors detected in the response.
        /// </summary>
        private string _responseErrors = String.Empty;
        public string ResponseErrors
        {
            get { return _responseErrors; }
        }

        /// <summary>
        /// Creates a new ELM327ListenerEventArgs object.
        /// </summary>
        /// <param name="handler">The handler that generated this event.</param>
        /// <param name="processedResponse">Processed form of the received response.</param>
        public ELM327ListenerEventArgs(IHandler handler, object processedResponse)
        {
            _handler = handler;
            _processedResponse = processedResponse;
        }

        /// <summary>
        /// Creates a new ELM327ListenerEventArgs object.
        /// </summary>
        /// <param name="handler">The handler that generated this event.</param>
        /// <param name="processedResponse">Processed form of the received response.</param>
        /// <param name="isBadResponse">Indicates if any errors were detected in the response.</param>
        /// <param name="responseErrors">Description of any errors detected in the response.</param>
        public ELM327ListenerEventArgs(IHandler handler, object processedResponse, bool isBadResponse, string responseErrors)
        {
            _handler = handler;
            _processedResponse = processedResponse;
            _isBadResponse = isBadResponse;
            _responseErrors = responseErrors;
        }
    }
}
