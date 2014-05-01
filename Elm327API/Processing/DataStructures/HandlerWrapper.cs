using ELM327API.Processing.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM327API.Processing.DataStructures
{
    public class HandlerWrapper
    {
        /// <summary>
        /// Get the logger.
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Stores the Type of the Handler this object wraps.
        /// </summary>
        private Type _handlerType;
        public Type HandlerType
        {
            get
            {
                return this._handlerType;
            }
        }

        /// <summary>
        /// Stores a reference to the 
        /// </summary>
        private IHandler _handler;
        public IHandler Handler
        {
            get
            {
                return this._handler;
            }
        }

        /// <summary>
        /// Stores the name of this Handler.
        /// </summary>
        public string Name
        {
            get
            {
                return this._handler.Name;
            }
        }

        /// <summary>
        /// Stores the unit of measure.
        /// </summary>
        public string Unit
        {
            get
            {
                return this._handler.Unit;
            }
        }

        /// <summary>
        /// Stores the category of this Handler.
        /// </summary>
        public HandlerCategory HandlerCategory
        {
            get
            {
                return this._handler.Category;
            }
        }

        /// <summary>
        /// Creates a new HandlerWrapper. handlerType must be the type of a class that implements IHandler.
        /// </summary>
        /// <param name="handlerType">Type of a class that implements IHandler.</param>
        public HandlerWrapper(Type handlerType)
        {
            // Verify that handlerType implements the IHandler interface
            // Can we assign handlerType to an IHandler reference???
            if (typeof(IHandler).IsAssignableFrom(handlerType))
            {
                try
                {
                    // Create the handler
                    this._handler = (IHandler)Activator.CreateInstance(handlerType);

                    // Store a reference to this type so that we may instantiante it the first time it is requested
                    this._handlerType = handlerType;
                }
                catch (Exception e)
                {
                    log.Error("Exception thrown while attempting to retrieve the static Name property from Type [" + handlerType.AssemblyQualifiedName + "].", e);
                }
            }
            else
            {
                log.Error("Attempted to create a new HandlerWrapper with a Type [" + handlerType.AssemblyQualifiedName + "] that does not implement IHandler.");
            }
        }
    }
}
