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
                // We use lazy instantiation here to conserve resources; therefore, we must
                // check to see if the handler has been instantiated (as a result of the 
                // application attempting to use this handler previously).
                if (this._handler == null)
                {
                    try
                    {
                        // Use a little "reflection glue" here to instantiate an unknown type
                        // dynamically.
                        this._handler = (IHandler)Activator.CreateInstance(this._handlerType);
                    }
                    catch (Exception e)
                    {
                        log.Error("Exception thrown while attempting to create object from Type [" + this._handlerType.AssemblyQualifiedName + "].", e);
                        return null;
                    }
                }
                return this._handler;
            }
        }

        /// <summary>
        /// Stores the name of this Handler.
        /// </summary>
        private string _name;
        public string Name
        {
            get
            {
                return this._name;
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
                    // Now that we have verified it, we need to retrieve our static Name property from the
                    // type (also using reflection) and populate the fields of this wrapper with those values.
                    this._name = (string)handlerType.GetProperty("Name").GetValue(null);

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
