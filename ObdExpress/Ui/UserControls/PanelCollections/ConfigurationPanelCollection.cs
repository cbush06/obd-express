using ObdExpress.Global;
using ObdExpress.Ui.UserControls.ConfigurationPanels;
using ObdExpress.Ui.UserControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObdExpress.Ui.UserControls.PanelCollections
{
    public class ConfigurationPanelCollection : IPanelCollection
    {

        private List<IRegisteredPanel> _panels = new List<IRegisteredPanel>();
        public List<IRegisteredPanel> Panels { get{ return this._panels; } }

        public ConfigurationPanelCollection()
        {
            this._panels.Add(new ConnectionSettingsPanel());
        }

        public void OnPanelCollectionShown()
        {
            foreach(IRegisteredPanel nextPanel in this._panels)
            {
                ELM327Connection.ConnectionEstablishedEvent += nextPanel.StartMonitoring;
                ELM327Connection.ConnectionClosingEvent += nextPanel.StopMonitoring;

                // If a connection is already established with the ELM327, notify the panels
                if (ELM327Connection.InOperation)
                {
                    nextPanel.StartMonitoring(ELM327Connection.ELM327Device.ConnectedPort);
                }
            }
        }

        public void OnPanelCollectionHidden()
        {
            foreach (IRegisteredPanel nextPanel in this._panels)
            {
                nextPanel.StopMonitoring();
            }
        }
    }
}
