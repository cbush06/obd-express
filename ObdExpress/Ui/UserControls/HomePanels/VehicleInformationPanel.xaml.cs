using ELM327API.Processing.DataStructures;
using ObdExpress.Global;
using ObdExpress.Ui.DataStructures;
using ObdExpress.Ui.UserControls.Interfaces;
using ObdExpress.Ui.Windows;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace ObdExpress.Ui.UserControls.HomePanels
{
    /// <summary>
    /// Interaction logic for VehicleInformationPanel.xaml
    /// </summary>
    public partial class VehicleInformationPanel : UserControl, IRegisteredPanel
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
        /// Provide the collection of DataItems to display.
        /// </summary>
        public ObservableCollection<DataItem> DataItems
        {
            get
            {
                return _dataItems;
            }
        }
        private ObservableCollection<DataItem> _dataItems = new ObservableCollection<DataItem>();

        public VehicleInformationPanel()
        {
            // Load the Dashboard Items in Order and Show the ones Selected in the Application Settings
            foreach (String nextHandlerName in ((String)Properties.ApplicationSettings.Default[Variables.SETTINGS_VEHICLEINFORMATION_HANDLERS]).Split(Variables.SETTINGS_SEPARATOR))
            {
                String actualHandlerName = String.Copy(nextHandlerName);

                if (actualHandlerName.Length > 0)
                {
                    if (actualHandlerName[0] == '+')
                    {
                        actualHandlerName = actualHandlerName.Substring(1);

                        for (int i = 0; i < ELM327Connection.LoadedHandlerTypes.Count; i++)
                        {
                            Type nextHandlerType = ELM327Connection.LoadedHandlerTypes[i];

                            if (nextHandlerType.Name.Equals(actualHandlerName))
                            {
                                DataItem newDashboardItem = new DataItem(nextHandlerType);
                                newDashboardItem.Position = _dataItems.Count;
                                _dataItems.Add(newDashboardItem);
                            }
                        }
                    }
                }
            }

            ELM327Connection.ConnectionEstablishedEvent += StartMonitoring;
            ELM327Connection.ConnectionClosingEvent += StopMonitoring;

            InitializeComponent();
        }

        public void Properties_Click(object sender, RoutedEventArgs args)
        {
            VehicleInformationPanelProperties propertiesWindow = new VehicleInformationPanelProperties(this);
            propertiesWindow.Owner = MainWindow.TopWindow;
            propertiesWindow.ShowDialog();
        }

        #region IRegisteredPanel Implementation
        public string Title
        {
            get
            {
                return "Vehicle Information";
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
            if (this.Show != null)
            {
                this.Show(this, e);
            }

            // Start Monitoring
            this.StartMonitoring(null);
        }

        public void HidePanel(object sender, RoutedEventArgs e)
        {
            _isShown = false;
            if (this.Hide != null)
            {
                this.Hide(this, e);
            }

            // Stop Monitoring
            this.StopMonitoring();
        }

        public void StartMonitoring(SerialPort s)
        {
            foreach (DataItem d in this._dataItems)
            {
                if (ELM327Connection.ELM327Device != null)
                {
                    ELM327Connection.ELM327Device.AsyncQuery(d.HandlerType.Name, this.Update);
                }
            }
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

        public void StopMonitoring()
        {
            foreach (DataItem d in this._dataItems)
            {
                if (ELM327Connection.ELM327Device != null)
                {
                    ELM327Connection.ELM327Device.UnregisterListener(d.HandlerType.Name, this.Update);
                }
            }
        }

        public void Update(ELM327ListenerEventArgs e)
        {
            foreach (DataItem d in this._dataItems)
            {
                if (d.HandlerType.Equals(e.Handler.GetType()))
                {
                    d.Value = e.ProcessedResponse.ToString();
                }
            }
        }

        public void RegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER)
        {
            switch (EVENT_HANDLER_TYPE)
            {
                case Variables.REGISTERED_EVENT_TYPE_HIDE_PANEL:
                    {
                        this.Hide += ROUTED_EVENT_HANDLER;
                        break;
                    }

                case Variables.REGISTERED_EVENT_TYPE_SHOW_PANEL:
                    {
                        this.Show += ROUTED_EVENT_HANDLER;
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
                        this.Hide -= ROUTED_EVENT_HANDLER;
                        break;
                    }

                case Variables.REGISTERED_EVENT_TYPE_SHOW_PANEL:
                    {
                        this.Show -= ROUTED_EVENT_HANDLER;
                        break;
                    }
            }
        }
        #endregion
    }
}
