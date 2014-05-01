using ELM327API.Connection.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using log4net;
using System.Collections.ObjectModel;
using System.Threading;
using ELM327API.Connection.Interfaces;
using System.IO.Ports;
using System.ComponentModel;
using ObdExpress.Global;
using System.IO;
using ELM327API.Global;

namespace ObdExpress.Ui.Windows
{
    /// <summary>
    /// Interaction logic for Elm327Connector.xaml
    /// </summary>
    public partial class ELM327Connector : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Get the logger.
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Provides access to necessary functions for managing ports, finding an ELM327 connection, and connecting.
        /// </summary>
        private ConnectorFactory _conFactory = new ConnectorFactory();

        /// <summary>
        /// List of metaconnectors describing available ports found by the ConnectionFactory.
        /// </summary>
        public LinkedList<MetaConnector> MetaConnectors
        {
            get
            {
                return this._conFactory.MetaConnectors;
            }
        }

        /// <summary>
        /// The selected connector.
        /// </summary>
        private MetaConnector _selectedConnector;
        public MetaConnector SelectedConnector
        {
            get
            {
                return this._selectedConnector;
            }
            set
            {
                this._selectedConnector = value;
                OnPropertyChanged("SelectedConnector");
            }
        }

        /// <summary>
        /// Stores a reference to the connector whose GetPort() method is running on a new Thread.
        /// </summary>
        private IConnector _runningConnector = null;

        /// <summary>
        /// Stores a reference to the current PortConnectionStatus object.
        /// </summary>
        private PortConnectionStatus _curPortConnectionStatus = null;

        /// <summary>
        /// Stores a list of structures that back the datagrid.
        /// </summary>
        private ObservableCollection<PortConnectionStatus> _portConnectionStatusList = new ObservableCollection<PortConnectionStatus>();
        public ObservableCollection<PortConnectionStatus> PortConnectionStatusList
        {
            get
            {
                return this._portConnectionStatusList;
            }
        }

        /// <summary>
        /// Thread used for getting a serial port.
        /// </summary>
        private Thread _getSerialPortThread = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ELM327Connector()
        {
            InitializeComponent();

            this.SelectedConnector = this._conFactory.MetaConnectors.ElementAt(0);
        }

        /// <summary>
        /// Initiates the connection process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // Clear the port connection status list
            this._portConnectionStatusList.Clear();
            this._curPortConnectionStatus = null;

            // Get the correct Connector
            IConnector connector = this._conFactory.GetConnector(this.SelectedConnector, ConstructConnectionSettings());
            
            // Hook into the connector's events
            connector.CheckingPort += this.Connector_NextPort;
            connector.UpdateMessages += this.Connector_UpdateMessage;
            connector.ConnectionComplete += this.Connector_Complete;
            connector.ConnectionEstablished += this.Connector_ConnectionEstablished;
            connector.PortSuccess += this.Connector_PortSuccess;

            // If this application is already connected on a SerialPort, close the connection
            if (ELM327Connection.Connection != null && ELM327Connection.Connection.IsOpen)
            {
                this.lblStatus.Content = "Closing existing ELM327 connection on port " + ELM327Connection.Connection.PortName + "...";
                log.Info("Closing existing ELM327 connection on port [" + ELM327Connection.Connection.PortName + "].");
                ELM327Connection.DestroyConnection();
            }

            // Log
            log.Info("Spawning thread for finding an Elm327 port and connecting.");

            // Disable Controls
            this.btnOk.IsEnabled = false;
            this.cmbPortList.IsEnabled = false;
            this.btnCancel.Content = "Cancel";

            // Start our Progress Bar
            this.pbProgressBar.IsIndeterminate = true;

            // Update the Status
            this.lblStatus.Content = "Connecting...";

            // Store a reference to the new IConnector
            this._runningConnector = connector;

            // Spawn a thread
            this._getSerialPortThread = new Thread(new ThreadStart(connector.GetSerialPort));
            this._getSerialPortThread.SetApartmentState(ApartmentState.STA);
            this._getSerialPortThread.Start();
        }

        /// <summary>
        /// Closes the window when the user clicks the Cancel button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Listener for the connector's CheckingPort event.
        /// </summary>
        /// <param name="portName">Name of the next port the connector will be checking.</param>
        private void Connector_NextPort(string portName)
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    this._curPortConnectionStatus = new PortConnectionStatus(portName);
                    this._portConnectionStatusList.Add(this._curPortConnectionStatus);
                }
            ));
        }

        /// <summary>
        /// Listener for the connector's UpdateMessages event.
        /// </summary>
        /// <param name="message">Message being sent by the connector.</param>
        private void Connector_UpdateMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    if (this._curPortConnectionStatus != null)
                    {
                        if (this._curPortConnectionStatus.Status.Length > 0)
                        {
                            this._curPortConnectionStatus.Status += ", ";
                        }

                        this._curPortConnectionStatus.Status += message;
                    }
                }
            ));
        }

        /// <summary>
        /// Listener for the connector's PortSuccess event.
        /// </summary>
        /// <param name="success">Success of the connector's attempt at finding an ELM327 on the current port.</param>
        private void Connector_PortSuccess(bool success)
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    // Update the DataGrid
                    if (this._curPortConnectionStatus != null)
                    {
                        // If we found it...
                        if (success)
                        {
                            // Change the Status Bar
                            this.lblStatus.Content = "ELM327 found on " + this._curPortConnectionStatus.PortName + "!";
                            this._curPortConnectionStatus.Success = success;
                        }
                        else
                        {
                            this._curPortConnectionStatus.Success = success;
                        }
                    }
                }
            ));
        }

        /// <summary>
        /// Listener for the connector's ConnectionEstablished event.
        /// </summary>
        /// <param name="port">The SerialPort object wrapping the first port the connector successfully found an ELM327 on.</param>
        private void Connector_ConnectionEstablished(SerialPort port)
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    // Try to connect with ELM327 device for IO operations
                    ELM327Connection.ConnectionEstablished(port, ConstructConnectionSettings());

                    if (ELM327Connection.InOperation)
                    {
                        if (this._curPortConnectionStatus.Status.Length > 0)
                        {
                            this._curPortConnectionStatus.Status += ", ";
                        }

                        this._curPortConnectionStatus.Status += "Successful IO Operations!";
                        this._curPortConnectionStatus.Success = true;
                    }
                    else
                    {
                        if (this._curPortConnectionStatus.Status.Length > 0)
                        {
                            this._curPortConnectionStatus.Status += ", ";
                        }

                        this._curPortConnectionStatus.Status += "Failed to Start IO Operations!";
                        this._curPortConnectionStatus.Success = false;
                    }
                }
            ));
        }

        /// <summary>
        /// Listener for the connector's Complete event.
        /// </summary>
        /// <param name="success">Overall success of the connector at finding a port with an ELM327 on it.</param>
        private void Connector_Complete(bool success)
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    if (!(success) || !(ELM327Connection.InOperation))
                    {
                        // Enable Controls
                        this.btnOk.IsEnabled = true;
                        this.cmbPortList.IsEnabled = true;
                        this.btnCancel.Content = "Close";

                        // Start our Progress Bar
                        this.pbProgressBar.IsIndeterminate = false;

                        // Update the Status
                        this.lblStatus.Content = "Failed to establish connection.";
                    }
                    else
                    {
                        // Enable Controls
                        this.btnOk.IsEnabled = true;
                        this.cmbPortList.IsEnabled = true;
                        this.btnCancel.Content = "Close";

                        // Start our Progress Bar
                        this.pbProgressBar.IsIndeterminate = false;

                        // Update the Status
                        this.lblStatus.Content = "Connected to ELM327 on Port " + this._curPortConnectionStatus.PortName + ".";
                    }
                }
            ));
        }

        /// <summary>
        /// Create a new ConnectionSettings object using the Application Properties settings.
        /// </summary>
        /// <returns>New ConnectionSettings object.</returns>
        private ConnectionSettings ConstructConnectionSettings()
        {
            return new ConnectionSettings(
                (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_BAUDRATE],
                (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_DATABITS],
                (Parity)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_PARITY],
                (StopBits)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_STOPBITS],
                (string)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_DEVICEDESCRIPTION]
            );
        }

        /// <summary>
        /// Handles killing the connector thread, if it is running, before the window closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void winElm327Connector_Closing(object sender, CancelEventArgs e)
        {
            if (this._getSerialPortThread != null && this._getSerialPortThread.IsAlive)
            {
                // Notify the thread to exit
                this._runningConnector.Kill();
            }
        }

        /// <summary>
        /// Structure for backing the datagrid.
        /// </summary>
        public class PortConnectionStatus : INotifyPropertyChanged
        {
            private string _portName = "";
            public string PortName
            {
                get
                {
                    return this._portName;
                }
                set
                {
                    this._portName = value;
                    OnPropertyChanged("PortName");
                }
            }

            private string _status = "";
            public string Status
            {
                get
                {
                    return this._status;
                }
                set
                {
                    this._status = value;
                    OnPropertyChanged("Status");
                }
            }

            private bool? _success = null;
            public bool? Success 
            {
                get
                {
                    return this._success;
                }
                set
                {
                    this._success = value;
                    OnPropertyChanged("Success");
                }
            }

            public PortConnectionStatus(string portName)
            {
                this.PortName = portName;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName)
            {
                if(PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        #region Implemntation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
