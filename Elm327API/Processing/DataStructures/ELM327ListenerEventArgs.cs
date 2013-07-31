using ELM327API.Processing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            get
            {
                return this._handler;
            }
        }

        /// <summary>
        /// Name of the handler that generated this event.
        /// </summary>
        public string HandlerName
        {
            get
            {
                if (this._handler != null)
                {
                    return this._handler.Name;
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
            get
            {
                return this._processedResponse;
            }
        }

        /// <summary>
        /// Creates a new ELM327ListenerEventArgs object.
        /// </summary>
        /// <param name="handler">The handler that generated this event.</param>
        /// <param name="processedResponse">Processed form of the received response.</param>
        public ELM327ListenerEventArgs(IHandler handler, object processedResponse)
        {
            this._handler = handler;
            this._processedResponse = processedResponse;
        }
    }
}
