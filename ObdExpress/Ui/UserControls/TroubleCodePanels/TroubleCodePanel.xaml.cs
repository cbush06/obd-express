using ELM327API.Processing.DataStructures;
using ObdExpress.Global;
using ObdExpress.Ui.UserControls.Interfaces;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace ObdExpress.Ui.UserControls.TroubleCodePanels
{
    /// <summary>
    /// Interaction logic for VehicleInformationPanel.xaml
    /// </summary>
    public partial class TroubleCodePanel : UserControl, IRegisteredPanel
    {
        /// <summary>
        /// Event called when this panel should be hidden.
        /// </summary>
        public event RoutedEventHandler Hide;

        /// <summary>
        /// Event called when this panel should be shown.
        /// </summary>
        public event RoutedEventHandler Show;

        public TroubleCodePanel()
        {
            ELM327Connection.ConnectionEstablishedEvent += StartMonitoring;
            ELM327Connection.ConnectionClosingEvent += StopMonitoring;

            InitializeComponent();
        }

        private void menItemRemove_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        #region IRegisteredPanel Implementation
        public string Title
        {
            get
            {
                return "Trouble Codes";
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
        }

        public void HidePanel(object sender, RoutedEventArgs e)
        {
            _isShown = false;
            if (this.Hide != null)
            {
                this.Hide(this, e);
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
