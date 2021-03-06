﻿using ObdExpress.Global;
using ObdExpress.Ui.UserControls.HomePanels;
using ObdExpress.Ui.UserControls.Interfaces;
using System.Collections.Generic;

namespace ObdExpress.Ui.UserControls.PanelCollections
{
    public class HomePanelCollection : IPanelCollection
    {
        private List<IRegisteredPanel> _panels = new List<IRegisteredPanel>();
        public List<IRegisteredPanel> Panels { get{ return _panels; } }

        public HomePanelCollection()
        {
            _panels.Add(new DashboardPanel());
            _panels.Add(new VehicleInformationPanel());
        }

        public void OnPanelCollectionShown()
        {
            foreach(IRegisteredPanel nextPanel in _panels)
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
            foreach (IRegisteredPanel nextPanel in _panels)
            {
                nextPanel.StopMonitoring();
            }
        }
    }
}
