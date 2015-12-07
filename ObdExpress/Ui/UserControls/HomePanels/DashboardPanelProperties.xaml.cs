using ELM327API;
using ELM327API.Processing.DataStructures;
using ObdExpress.Global;
using ObdExpress.Ui.DataStructures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace ObdExpress.Ui.UserControls.HomePanels
{
    /// <summary>
    /// Interaction logic for DashboardPanelProperties.xaml
    /// </summary>
    public partial class DashboardPanelProperties : Window, INotifyPropertyChanged
    {
        private DashboardPanel _parent = null;

        /// <summary>
        /// Provides access to the selected item.
        /// </summary>
        public PanelPropertyOption SelectedItem
        {
            get
            {
                return (PanelPropertyOption)dgPanels.SelectedItem;
            }
            set
            {
                dgPanels.SelectedItem = value;
            }
        }

        /// <summary>
        /// Provides access to the DashboardPanel's columns setting.
        /// </summary>
        private int _columns = 0;
        public int Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
                OnPropertyChanged("Columns");
            }
        }

        /// <summary>
        /// Datastructure backing the DataGrid.
        /// </summary>
        private ObservableCollection<PanelPropertyOption> _panelPropertyOptions = new ObservableCollection<PanelPropertyOption>();
        public ObservableCollection<PanelPropertyOption> PanelPropertyOptions
        {
            get
            {
                return _panelPropertyOptions;
            }
        }

        /// <summary>
        /// Overloaded constructor that accepts a reference to a collection of dashboad items. Presumably, this is the collection used by the Dashboard Panel.
        /// </summary>
        /// <param name="dashboardItems">Collection of dashboard items to edit.</param>
        public DashboardPanelProperties(DashboardPanel parent)
        {
            int counter = 0;
            _parent = parent;
            InitializeComponent();

            // Populate our list of choices
            foreach (Type nextHandlerType in ELM327Connection.LoadedHandlerTypes)
            {
                HandlerWrapper wrapper = new HandlerWrapper(nextHandlerType);

                if (wrapper.HandlerCategory == HandlerCategory.REAL_TIME_STATUS)
                {
                    _panelPropertyOptions.Add(new PanelPropertyOption(wrapper, _panelPropertyOptions.Count));
                }
            }

            // Move items to their positions and mark the selected ones
            foreach (String nextHandlerName in ((String)Properties.ApplicationSettings.Default[Variables.SETTINGS_DASHBOARD_HANDLERS]).Split(Variables.SETTINGS_SEPARATOR))
            {
                String actualHandlerName = String.Copy(nextHandlerName);
                bool isShown = false;

                if (actualHandlerName.Length > 0)
                {
                    if (actualHandlerName[0] == '+')
                    {
                        isShown = true;
                        actualHandlerName = actualHandlerName.Substring(1);
                    }

                    for(int i = 0; i < _panelPropertyOptions.Count; i++)
                    {
                        PanelPropertyOption nextOption = _panelPropertyOptions[i];

                        if (nextOption.HandlerWrapper.HandlerType.Name.Equals(actualHandlerName))
                        {
                            _panelPropertyOptions[i].IsChecked = isShown;
                            _panelPropertyOptions[i].Position = counter;
                            _panelPropertyOptions.Move(i, counter++);
                            break;
                        }
                    }
                }
            }

            // Set the columns
            _columns = _parent.Columns;

            OnPropertyChanged("PanelPropertyOptions");
            OnPropertyChanged("Columns");
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DashboardPanelProperties()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Moves a Dashboard Item up 1 position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                if (SelectedItem.Position != 0)
                {
                    PanelPropertyOptions.Move(SelectedItem.Position, SelectedItem.Position - 1);
                    PanelPropertyOptions[SelectedItem.Position].Position = SelectedItem.Position;
                    SelectedItem.Position = SelectedItem.Position - 1;
                }
            }
        }

        /// <summary>
        /// Moves a Dashboard Item down 1 position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                if (SelectedItem.Position < (PanelPropertyOptions.Count - 1))
                {
                    PanelPropertyOptions.Move(SelectedItem.Position, SelectedItem.Position + 1);
                    PanelPropertyOptions[SelectedItem.Position].Position = SelectedItem.Position;
                    SelectedItem.Position = SelectedItem.Position + 1;
                }
            }
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder newSettingValue = new StringBuilder();

            // Clear the Application Setting for Selected Dashboard Panel Handlers
            Properties.ApplicationSettings.Default[Variables.SETTINGS_DASHBOARD_HANDLERS] = String.Empty;

            // Stop Monitoring so Handlers are reset before we change their listeners
            if (ELM327Connection.InOperation)
            {
                _parent.StopMonitoring();
            }

            // Clear the old DashboardItems
            _parent.DashboardItems.Clear();

            // Build the new value for this Setting
            foreach (PanelPropertyOption nextItem in PanelPropertyOptions)
            {
                if (nextItem.IsChecked)
                {
                    newSettingValue.Append("+" + nextItem.HandlerWrapper.HandlerType.Name + Variables.SETTINGS_SEPARATOR);

                    _parent.DashboardItems.Add(new DataItem(nextItem.HandlerWrapper.HandlerType));
                }
                else
                {
                    newSettingValue.Append(nextItem.HandlerWrapper.HandlerType.Name + Variables.SETTINGS_SEPARATOR);
                }
            }

            // Save the new value to the Setting
            Properties.ApplicationSettings.Default[Variables.SETTINGS_DASHBOARD_HANDLERS] = newSettingValue.ToString();

            // Save the columns to the Setting
            Properties.ApplicationSettings.Default[Variables.SETTINGS_DASHBOARD_COLUMNS] = Columns;

            // Update the Dashboard Panel's columns and rows
            _parent.Columns = _columns;
            _parent.Rows = (int)(Math.Ceiling((double)_parent.DashboardItems.Count / (double)_parent.Columns));

            // Restart Monitoring so all DashboardItems are guaranteed to be registered with ELM327
            if (ELM327Connection.InOperation)
            {
                _parent.StartMonitoring(null);
            }

            Close();
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
