using ObdExpress.Ui.DataStructures;
using ObdExpress.Global;
using System.Collections.ObjectModel;
using System.Windows;
using ObdExpress.Ui.UserControls.Interfaces;
using System.ComponentModel;
using ObdExpress.Ui.Windows;
using System.IO.Ports;
using ELM327API.Processing.DataStructures;
using System.Windows.Controls;
using System;

namespace ObdExpress.Ui.UserControls.HomePanels
{
    /// <summary>
    /// Interaction logic for DashboardPanel.xaml
    /// </summary>
    public partial class DashboardPanel : UserControl, IRegisteredPanel, INotifyPropertyChanged
    {
        
        /// <summary>
        /// Sets the number of columns shown.
        /// </summary>
        private int _columns = 0;
        public int Columns
        {
            get
            {
                return this._columns;
            }
            set
            {
                this._columns = value;
                this.OnPropertyChanged("Columns");
            }
        }

        /// <summary>
        /// Sets the number of rows shown.
        /// </summary>
        private int _rows = 0;
        public int Rows
        {
            get
            {
                return this._rows;
            }
            set
            {
                this._rows = value;
                this.OnPropertyChanged("Rows");
            }
        }

        /// <summary>
        /// The items displayed in this DashbaordPanel.
        /// </summary>
        private ObservableCollection<DataItem> _dashboardItems = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> DashboardItems
        {
            get
            {
                return this._dashboardItems;
            }
        }

        /// <summary>
        /// Event called when this panel should be hidden.
        /// </summary>
        private event RoutedEventHandler Hide;

        /// <summary>
        /// Event called when this panel should be shown.
        /// </summary>
        private event RoutedEventHandler Show;

        /// <summary>
        /// Create a new Dashboard Panel.
        /// </summary>
        public DashboardPanel()
        {
            InitializeComponent();

            // Load the Dashboard Items in Order and Show the ones Selected in the Application Settings
            foreach (String nextHandlerName in ((String)Properties.ApplicationSettings.Default[Variables.SETTINGS_DASHBOARD_HANDLERS]).Split(Variables.SETTINGS_SEPARATOR))
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
                                newDashboardItem.Position = _dashboardItems.Count;
                                _dashboardItems.Add(newDashboardItem);
                            }
                        }
                    }
                }
            }

            // Set the Columns Property based on Application Settings
            this.Columns = (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_DASHBOARD_COLUMNS];

            // Set the Rows Property
            this.Rows = (int)(Math.Ceiling((double)_dashboardItems.Count / (double)this.Columns));

            ELM327Connection.ConnectionEstablishedEvent += StartMonitoring;
            ELM327Connection.ConnectionClosingEvent += StopMonitoring;
        }

        /// <summary>
        /// Show the Properties Dialog to edit Dashboard Panel items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menItemProperties_Click(object sender, RoutedEventArgs e)
        {
            DashboardPanelProperties props = new DashboardPanelProperties(this);

            props.Owner = MainWindow.TopWindow;
            props.ShowDialog();
        }

        /// <summary>
        /// Alert this Panel's parent that the user has removed it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menItemRemove_Click(object sender, RoutedEventArgs e)
        {
            if (this.Hide != null)
            {
                this.Hide(this, new RoutedEventArgs(e.RoutedEvent, this));
            }
        }

        #region IRegisteredPanel Implementation
        public string Title
        {
            get
            {
                return "Dashboard";
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

            // Start Monitoring since this Panel will now be shown
            this.StartMonitoring(null);
        }

        public void HidePanel(object sender, RoutedEventArgs e)
        {
            _isShown = false;
            if (this.Hide != null)
            {
                this.Hide(this, e);
            }

            // Stop Monitoring since this Panel will no longer be shown
            this.StopMonitoring();
        }

        public void StartMonitoring(SerialPort s)
        {
            foreach (DataItem d in this._dashboardItems)
            {
                if (ELM327Connection.ELM327Device != null)
                {
                    ELM327Connection.ELM327Device.RegisterListener(d.HandlerType.Name, this.Update);
                }
            }
        }

        public void StopMonitoring()
        {
            foreach (DataItem d in this._dashboardItems)
            {
                if (ELM327Connection.ELM327Device != null)
                {
                    ELM327Connection.ELM327Device.UnregisterListener(d.HandlerType.Name, this.Update);
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

        public void Update(ELM327ListenerEventArgs e)
        {
            foreach (DataItem d in this._dashboardItems)
            {
                if(d.HandlerType.Equals(e.Handler.GetType()))
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

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
