using ELM327API.Processing.DataStructures;
using ObdExpress.Global;
using ObdExpress.Ui.UserControls.Interfaces;
using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace ObdExpress.Ui.UserControls.ConfigurationPanels
{
    /// <summary>
    /// Interaction logic for VehicleInformationPanel.xaml
    /// </summary>
    public partial class ConnectionSettingsPanel : UserControl, IRegisteredPanel, INotifyPropertyChanged
    {
        /// <summary>
        /// Event called when this panel should be hidden.
        /// </summary>
        public event RoutedEventHandler Hide;

        /// <summary>
        /// Event called when this panel should be shown.
        /// </summary>
        public event RoutedEventHandler Show;

        /// <summary>
        /// List of availale baud rates.
        /// </summary>
        public Int32[] BaudRates
        {
            get { return _baudRates; }
        }
        private Int32[] _baudRates = new Int32[] { 75, 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };

        public Int32 SelectedBaudRate
        {
            get 
            { 
                return _selectedBaudRate; 
            }
            set 
            { 
                _selectedBaudRate = value;
                OnPropertyChanged("SelectedBaudRate");
                Field_Changed();
            }
        }
        private Int32 _selectedBaudRate = 9600;

        /// <summary>
        /// List of available data bit settings.
        /// </summary>
        public Int32[] DataBits
        {
            get { return _dataBits; }
        }
        private Int32[] _dataBits = new Int32[] { 5, 6, 7, 8, 9 };

        public Int32 SelectedDataBits
        {
            get 
            { 
                return _selectedDataBits; 
            }
            set 
            { 
                _selectedDataBits = value;
                OnPropertyChanged("SelectedDataBits");
                Field_Changed();
            }
        }
        private Int32 _selectedDataBits = 8;

        /// <summary>
        /// Stores the selected Parity value.
        /// </summary>
        public string SelectedParity
        {
            get 
            { 
                return Enum.GetName(typeof(Parity), _selectedParity); 
            }
            set 
            { 
                _selectedParity = (Parity)Enum.Parse(typeof(Parity), value);
                OnPropertyChanged("SelectedParity");
                Field_Changed();
            }
        }
        private Parity _selectedParity = Parity.None;

        /// <summary>
        /// Stores the selected StopBits value.
        /// </summary>
        public string SelectedStopBits
        {
            get 
            { 
                return Enum.GetName(typeof(StopBits), _selectedStopBits); 
            }
            set 
            { 
                _selectedStopBits = (StopBits)Enum.Parse(typeof(StopBits), value);
                OnPropertyChanged("SelectedStopBits");
                Field_Changed();
            }
        }
        private StopBits _selectedStopBits = StopBits.One;

        /// <summary>
        /// Stores the value of Device Description.
        /// </summary>
        public string DeviceDescription
        {
            get 
            { 
                return _deviceDescription; 
            }
            set 
            { 
                _deviceDescription = value;
                OnPropertyChanged("DeviceDescription");
                Field_Changed();
            }
        }
        private string _deviceDescription = "";

        /// <summary>
        /// Property that allows us to bold the Title
        /// </summary>
        private FontWeight _panelTitleFontWeight = FontWeights.Normal;
        public FontWeight PanelTitleFontWeight
        {
            get
            {
                return _panelTitleFontWeight;
            }
            set
            {
                _panelTitleFontWeight = value;
                OnPropertyChanged("PanelTitleFontWeight");
            }
        }

        /// <summary>
        /// Used to make the Change handler wait until after the panel loads to
        /// begin bolding the title and appending an asterisk when values are chagned.
        /// </summary>
        private bool _allowChanges = false;

        public ConnectionSettingsPanel()
        {
            ELM327Connection.ConnectionEstablishedEvent += StartMonitoring;
            ELM327Connection.ConnectionClosingEvent += StopMonitoring;

            LoadApplicationPropertiesValues();

            InitializeComponent();

            _allowChanges = true;
        }

        private void LoadApplicationPropertiesValues()
        {
            _selectedBaudRate = (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_BAUDRATE];
            _selectedDataBits = (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_DATABITS];
            _selectedParity = (Parity)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_PARITY];
            _selectedStopBits = (StopBits)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_STOPBITS];
            _deviceDescription = (string)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_DEVICEDESCRIPTION];
        }

        private void btnSave_Click(object sender, RoutedEventArgs args)
        {
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_BAUDRATE] = _selectedBaudRate;
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_DATABITS] = _selectedDataBits;
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_PARITY] = _selectedParity;
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_STOPBITS] = _selectedStopBits;
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONNECTION_DEVICEDESCRIPTION] = _deviceDescription;

            Properties.ApplicationSettings.Default.Save();

            Fields_Unchanged();
        }

        private void btnReset_Click(object sender, RoutedEventArgs args)
        {
            SelectedBaudRate = 9600;
            SelectedDataBits = 8;
            SelectedParity = Enum.GetName(typeof(Parity), Parity.None);
            SelectedStopBits = Enum.GetName(typeof(StopBits), StopBits.One);
            DeviceDescription = "OBDII to RS232 Interpreter";

            Field_Changed();
        }

        private void Field_Changed()
        {
            if (_title[_title.Length - 1] != '*')
            {
                Title += " *";
                PanelTitleFontWeight = FontWeights.Bold;
            }
        }

        private void Fields_Unchanged()
        {
            if (_title[_title.Length - 1] == '*')
            {
                Title = _title.Substring(0, _title.Length - 2);
            }
            PanelTitleFontWeight = FontWeights.Normal;
        }

        #region IRegisteredPanel Implementation
        private string _title = "Connection Settings";
        public string Title
        {
            get 
            { 
                return _title; 
            }
            set 
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        private bool _isShown = true;
        public bool IsShown
        {
            get
            {
                return _isShown;
            }
        }

        private bool _isPaused = false;
        public bool IsPaused
        {
            get
            {
                return _isPaused;
            }
        }

        public void ShowPanel(object sender, RoutedEventArgs e)
        {
            _isShown = true;
            if (Show != null)
            {
                Show(this, e);
            }
        }

        public void HidePanel(object sender, RoutedEventArgs e)
        {
            _isShown = false;
            if (Hide != null)
            {
                Hide(this, e);
            }
        }

        public void StartMonitoring(SerialPort s)
        {
            return;
        }

        public void StopMonitoring()
        {
            return;
        }

        public void PauseMonitoring()
        {
            _isPaused = true;
            StopMonitoring();
        }

        public void UnPauseMonitoring()
        {
            _isPaused = false;
            StartMonitoring(null);
        }

        public void Update(ELM327ListenerEventArgs e)
        {
            return;
        }

        public void RegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER)
        {
            switch (EVENT_HANDLER_TYPE)
            {
                case Variables.REGISTERED_EVENT_TYPE_HIDE_PANEL:
                    {
                        Hide += ROUTED_EVENT_HANDLER;
                        break;
                    }

                case Variables.REGISTERED_EVENT_TYPE_SHOW_PANEL:
                    {
                        Show += ROUTED_EVENT_HANDLER;
                        break;
                    }
            }
        }

        public void UnRegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER)
        {
            switch (EVENT_HANDLER_TYPE)
            {
                case Variables.REGISTERED_EVENT_TYPE_HIDE_PANEL:
                    {
                        Hide -= ROUTED_EVENT_HANDLER;
                        break;
                    }

                case Variables.REGISTERED_EVENT_TYPE_SHOW_PANEL:
                    {
                        Show -= ROUTED_EVENT_HANDLER;
                        break;
                    }
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
