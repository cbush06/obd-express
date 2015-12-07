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
                return _portName;
            }
        }

        /// <summary>
        /// The description for the user. This is also used in an override of the ToString() method.
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
        }

        /// <summary>
        /// Create a MetaConnector.
        /// </summary>
        /// <param name="portName">Name of the port it represents.</param>
        /// <param name="description">Description to be shown to the user.</param>
        public MetaConnector(string portName, string description)
        {
            _portName = portName;
            _description = description;
        }

        /// <summary>
        /// Override ToString() to return our own description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _description;
        }
    }
}
