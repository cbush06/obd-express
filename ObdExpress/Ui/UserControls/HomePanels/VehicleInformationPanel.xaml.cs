using ObdExpress.Ui.UserControls.Interfaces;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObdExpress.Ui.UserControls.HomePanels
{
    /// <summary>
    /// Interaction logic for VehicleInformationPanel.xaml
    /// </summary>
    public partial class VehicleInformationPanel : UserControl, IRegisteredPanel
    {
        public const short REGISTERED_EVENT_TYPE_REMOVE_PANEL = 0x0000;

        /// <summary>
        /// Event called when the Remove menu item is selected.
        /// </summary>
        public event RoutedEventHandler Remove;

        public VehicleInformationPanel()
        {
            InitializeComponent();
        }

        private void menItemRemove_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        #region IRegisteredPanel Implementation
        public void StartMonitoring()
        {
            throw new NotImplementedException();
        }

        public void StopMonitoring()
        {
            throw new NotImplementedException();
        }

        public void RegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER)
        {
            switch (EVENT_HANDLER_TYPE)
            {
                case VehicleInformationPanel.REGISTERED_EVENT_TYPE_REMOVE_PANEL:
                    {
                        this.Remove += ROUTED_EVENT_HANDLER;
                        break;
                    }
            }
        }

        public void UnRegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER)
        {
            switch (EVENT_HANDLER_TYPE)
            {
                case VehicleInformationPanel.REGISTERED_EVENT_TYPE_REMOVE_PANEL:
                    {
                        this.Remove -= ROUTED_EVENT_HANDLER;
                        break;
                    }
            }
        }
        #endregion
    }
}
