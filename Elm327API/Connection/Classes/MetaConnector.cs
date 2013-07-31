using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM327API.Connection.Classes
{
    /// <summary>
    /// Descriptor object for an IConnector object.
    /// </summary>
    public class MetaConnector
    {
        private string _portName = "";
        private string _description = "";
        
        /// <summary>
        /// Name of the Port this represents a Connector for. Empty if this represents the AutoConnector.
        /// </summary>
        public string PortName
        {
            get
            {
                return this._portName;
            }
        }

        /// <summary>
        /// The description for the user. This is also used in an override of the ToString() method.
        /// </summary>
        public string Description
        {
            get
            {
                return this._description;
            }
        }

        /// <summary>
        /// Create a MetaConnector.
        /// </summary>
        /// <param name="portName">Name of the port it represents.</param>
        /// <param name="description">Description to be shown to the user.</param>
        public MetaConnector(string portName, string description)
        {
            this._portName = portName;
            this._description = description;
        }

        /// <summary>
        /// Override ToString() to return our own description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._description;
        }
    }
}
