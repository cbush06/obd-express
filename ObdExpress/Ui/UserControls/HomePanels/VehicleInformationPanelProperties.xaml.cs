using ELM327API;
using ELM327API.Processing.DataStructures;
using ObdExpress.Global;
using ObdExpress.Ui.DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace ObdExpress.Ui.UserControls.HomePanels
{
    /// <summary>
    /// Interaction logic for DashboardPanelProperties.xaml
    /// </summary>
    public partial class VehicleInformationPanelProperties : Window, INotifyPropertyChanged
    {
        private VehicleInformationPanel _parent = null;

        /// <summary>
        /// Provides access to the selected item.
        /// </summary>
        private PanelPropertyOption _selectedItem = null;
        public PanelPropertyOption SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
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

        public VehicleInformationPanelProperties(VehicleInformationPanel parent)
        {
            int counter = 0;
            this._parent = parent;
            InitializeComponent();

            // Populate our list of choices
            foreach (Type nextHandlerType in ELM327Connection.LoadedHandlerTypes)
            {
                HandlerWrapper wrapper = new HandlerWrapper(nextHandlerType);

                if (wrapper.HandlerCategory == HandlerCategory.VEHICLE_INFORMATION)
                {
                    _panelPropertyOptions.Add(new PanelPropertyOption(wrapper, _panelPropertyOptions.Count));
                }
            }

            // Move items to their positions and mark the selected ones
            foreach (String nextHandlerName in ((String)Properties.ApplicationSettings.Default[Variables.SETTINGS_VEHICLEINFORMATION_HANDLERS]).Split(Variables.SETTINGS_SEPARATOR))
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

            this.OnPropertyChanged("DashboardPanelPropertyOptions");
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public VehicleInformationPanelProperties()
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
            if (this.SelectedItem != null)
            {
                if (this.SelectedItem.Position != 0)
                {
                    this.PanelPropertyOptions.Move(this.SelectedItem.Position, this.SelectedItem.Position - 1);
                    this.PanelPropertyOptions[this.SelectedItem.Position].Position = this.SelectedItem.Position;
                    this.SelectedItem.Position = this.SelectedItem.Position - 1;
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
            if (this.SelectedItem != null)
            {
                if (this.SelectedItem.Position < (this.PanelPropertyOptions.Count - 1))
                {
                    this.PanelPropertyOptions.Move(this.SelectedItem.Position, this.SelectedItem.Position + 1);
                    this.PanelPropertyOptions[this.SelectedItem.Position].Position = this.SelectedItem.Position;
                    this.SelectedItem.Position = this.SelectedItem.Position + 1;
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
            Properties.ApplicationSettings.Default[Variables.SETTINGS_VEHICLEINFORMATION_HANDLERS] = String.Empty;

            // Stop Monitoring so Handlers are reset before we change their listeners
            if (ELM327Connection.InOperation)
            {
                _parent.StopMonitoring();
            }

            // Clear the old DashboardItems
            _parent.DataItems.Clear();

            // Build the new value for this Setting
            foreach (PanelPropertyOption nextItem in this.PanelPropertyOptions)
            {
                if (nextItem.IsChecked)
                {
                    newSettingValue.Append("+" + nextItem.HandlerWrapper.HandlerType.Name + Variables.SETTINGS_SEPARATOR);

                    _parent.DataItems.Add(new DataItem(nextItem.HandlerWrapper.HandlerType));
                }
                else
                {
                    newSettingValue.Append(nextItem.HandlerWrapper.HandlerType.Name + Variables.SETTINGS_SEPARATOR);
                }
            }

            // Save the new value to the Setting
            Properties.ApplicationSettings.Default[Variables.SETTINGS_VEHICLEINFORMATION_HANDLERS] = newSettingValue.ToString();

            // Restart Monitoring so all DashboardItems are guaranteed to be registered with ELM327
            if (ELM327Connection.InOperation)
            {
                _parent.StartMonitoring(null);
            }

            this.Close();
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
