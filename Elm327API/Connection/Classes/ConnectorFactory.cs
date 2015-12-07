using ELM327API.Connection.Interfaces;
using ELM327API.Global;
using log4net;
using System.Collections.Generic;
using System.IO.Ports;

namespace ELM327API.Connection.Classes
{
    /// <summary>
    /// A factory for creating Connector objects.
    /// </summary>
    public class ConnectorFactory
    {
        /// <summary>
        /// Get the logger.
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Objects describing the available connectors. Use these to request an actual connector.
        /// </summary>
        private LinkedList<MetaConnector> _metaConnectors = new LinkedList<MetaConnector>();
        public LinkedList<MetaConnector> MetaConnectors
        {
            get
            {
                return _metaConnectors;
            }
        }

        /// <summary>
        /// Create a Connector Factory.
        /// </summary>
        public ConnectorFactory()
        {
            // Get the Port Names
            string[] portNames = SerialPort.GetPortNames();

            // Add an entry for the AutoConnector
            _metaConnectors.AddLast(new MetaConnector("", "Auto Connector"));

            // Add a connector for each Port Name to the list
            foreach (string port in portNames)
            {
                _metaConnectors.AddLast(new MetaConnector(port, port));
            }
        }

        /// <summary>
        /// Get an instance of the connector described by the MetaConnector.
        /// </summary>
        /// <param name="metaConnector">Object describing a connector.</param>
        /// <returns>(IConnector) A connector that will attempt to establish a connection with a Serial Port to which an ELM327 device is attached.</returns>
        public IConnector GetConnector(MetaConnector metaConnector, ConnectionSettings connectionSettings)
        {
            IConnector connector = null;

            // If this MetaConnector represents a particular port (as opposed to the AutoConnector)
            if (metaConnector.PortName.Length > 0)
            {
                connector = new PortConnector(metaConnector.PortName, connectionSettings);
            }
            else
            {
                log.Info("Returning AutoConnector");
                connector = new AutoConnector(connectionSettings);
            }

            return connector;
        }
    }
}
