using ObdExpress.Ui.DataStructures;
using ObdExpress.Global.Threads;
using ObdExpress.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ObdExpress.Ui.UserControls.Interfaces;
using System.ComponentModel;
using ObdExpress.Ui.Windows;

namespace ObdExpress.Ui.UserControls.HomePanels
{
    /// <summary>
    /// Interaction logic for DashboardPanel.xaml
    /// </summary>
    public partial class DashboardPanel : UserControl, IRegisteredPanel, INotifyPropertyChanged
    {
        public const short REGISTERED_EVENT_TYPE_REMOVE_PANEL = 0x0000;
        public const short REGISTERED_EVENT_TYPE_START_MONITORING = 0x0001;
        public const short REGISTERED_EVENT_TYPE_STOP_MONITORING = 0x0002;

        private bool _isMonitoring = false;

        private DashboardItem item4 = new DashboardItem();
        private DashboardItem item3 = new DashboardItem();
        private DashboardItem item2 = new DashboardItem();
        private DashboardItem item1 = new DashboardItem();

        private Thread testThread;

        /// <summary>
        /// Title shown.
        /// </summary>
        public string Title
        {
            get
            {
                return "Dashboard";
            }
        }

        /// <summary>
        /// Sets the number of columns shown.
        /// </summary>
        private int _columns = 3;
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
        /// The items displayed in this DashbaordPanel.
        /// </summary>
        private ObservableCollection<DashboardItem> _dashboardItems = new ObservableCollection<DashboardItem>();
        public ObservableCollection<DashboardItem> DashboardItems
        {
            get
            {
                return this._dashboardItems;
            }
        }

        /// <summary>
        /// Event called when the Remove menu item is selected.
        /// </summary>
        private event RoutedEventHandler Remove;

        /// <summary>
        /// Event called when the Start Monitoring menu item is selected.
        /// </summary>
        private event RoutedEventHandler StartMonitoringEvent;

        /// <summary>
        /// Event called when the Stop Monitoring menu item is selected.
        /// </summary>
        private event RoutedEventHandler StopMonitoringEvent;

        public DashboardPanel()
        {
            item1.Unit = "MPH";
            item1.Value = "25";

            item2.Unit = "RPM";
            item2.Value = "2500";

            item3.Unit = "F";
            item3.Value = "300";

            item4.Unit = "OIL";
            item4.Value = "60";

            this._dashboardItems.Add(item1);
            this._dashboardItems.Add(item2);
            this._dashboardItems.Add(item3);
            this._dashboardItems.Add(item4);

            InitializeComponent();
        }

        private void UpdateMph(string value)
        {
            this.item1.Value = value;
        }

        public void TestRoutine()
        {
            int speed = 0;

            while (true)
            {
                for (speed = 0; speed < 101; speed++)
                {
                    this.Dispatcher.Invoke(new Delegates.UITextUpdateDelegate(this.UpdateMph), String.Format("{0:D}", speed));
                    try
                    {
                        Thread.Sleep(10);
                    }
                    catch (ThreadAbortException e)
                    {
                        return;
                    }
                    catch (ThreadInterruptedException e)
                    {
                        return;
                    }

                    if (Variables.ShouldClose || !this._isMonitoring)
                    {
                        return;
                    }
                }

                for (; speed > -1; speed--)
                {
                    this.Dispatcher.Invoke(new Delegates.UITextUpdateDelegate(this.UpdateMph), String.Format("{0:D}", speed));
                    try
                    {
                        Thread.Sleep(10);
                    }
                    catch (ThreadAbortException e)
                    {
                        return;
                    }
                    catch (ThreadInterruptedException e)
                    {
                        return;
                    }

                    if (Variables.ShouldClose || !this._isMonitoring)
                    {
                        return;
                    }
                }
            }
        }

        private void menItemStartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            this.StartMonitoring();
        }

        private void menItemStopMonitoring_Click(object sender, RoutedEventArgs e)
        {
            this.StopMonitoring();
        }

        private void menItemProperties_Click(object sender, RoutedEventArgs e)
        {
            DashboardPanelProperties props = new DashboardPanelProperties(this);

            props.Owner = MainWindow.TopWindow;
            props.ShowDialog();
        }

        private void menItemRemove_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        #region IRegisteredPanel Implementation
        public void StartMonitoring()
        {
            if (this.testThread == null || !(this.testThread.IsAlive))
            {
                this._isMonitoring = true;
                this.testThread = new Thread(new ThreadStart(this.TestRoutine));
                this.testThread.Start();

                if (this.StartMonitoringEvent != null)
                {
                    this.StartMonitoringEvent(this, null);
                }
            }
        }

        public void StopMonitoring()
        {
            if (this.testThread != null && this.testThread.IsAlive)
            {
                this.testThread.Abort();
                this._isMonitoring = false;

                if (this.StopMonitoringEvent != null)
                {
                    this.StopMonitoringEvent(this, null);
                }
            }
        }

        public void RegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER)
        {
            switch (EVENT_HANDLER_TYPE)
            {
                case DashboardPanel.REGISTERED_EVENT_TYPE_REMOVE_PANEL:
                    {
                        this.Remove += ROUTED_EVENT_HANDLER;
                        break;
                    }

                case DashboardPanel.REGISTERED_EVENT_TYPE_START_MONITORING:
                    {
                        this.StartMonitoringEvent += ROUTED_EVENT_HANDLER;
                        break;
                    }

                case DashboardPanel.REGISTERED_EVENT_TYPE_STOP_MONITORING:
                    {
                        this.StopMonitoringEvent += ROUTED_EVENT_HANDLER;
                        break;
                    }
            }
        }

        public void UnRegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER)
        {
            switch (EVENT_HANDLER_TYPE)
            {
                case DashboardPanel.REGISTERED_EVENT_TYPE_REMOVE_PANEL:
                    {
                        this.Remove -= ROUTED_EVENT_HANDLER;
                        break;
                    }

                case DashboardPanel.REGISTERED_EVENT_TYPE_START_MONITORING:
                    {
                        this.StartMonitoringEvent -= ROUTED_EVENT_HANDLER;
                        break;
                    }

                case DashboardPanel.REGISTERED_EVENT_TYPE_STOP_MONITORING:
                    {
                        this.StopMonitoringEvent -= ROUTED_EVENT_HANDLER;
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
