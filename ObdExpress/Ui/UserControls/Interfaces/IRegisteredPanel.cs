using ELM327API.Processing.DataStructures;
using ObdExpress.Global;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ObdExpress.Ui.UserControls.Interfaces
{
    public interface IRegisteredPanel
    {
        /// <summary>
        /// Returns a user-friendly name for this panel.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Indicates if this panel should be hidden or not.
        /// </summary>
        bool IsShown { get; }

        /// <summary>
        /// Returns true if this panel has been paused previously.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Event Listener called when this panel should be hidden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void HidePanel(object sender, RoutedEventArgs e);

        /// <summary>
        /// Event Listener called when this panel should be shown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ShowPanel(object sender, RoutedEventArgs e);

        /// <summary>
        /// Event listener called when an ELM327 connection is established. This is the Registered Panel's chance to hook into its corresponding ELM327 data handler.
        /// </summary>
        void StartMonitoring(SerialPort s);

        /// <summary>
        /// Event listener called when an ELM327 connection is about to be destroyed. This is the Registered Panel's chance to unregister itself.
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// Event listener called when monitoring should be temporarily paused (e.g. when panel collections are changing).
        /// </summary>
        void PauseMonitoring();

        /// <summary>
        /// Event listener called when monitoring should be unpaused (e.g. when a panel collection is re-shown).
        /// </summary>
        void UnPauseMonitoring();

        /// <summary>
        /// Receives data updates from the ELM327 device.
        /// </summary>
        /// <param name="e"></param>
        void Update(ELM327ListenerEventArgs e);

        /// <summary>
        /// Registers an event handler to a particular type of event this panel might generate.
        /// </summary>
        /// <param name="EVENT_HANDLER_TYPE">A constant integer representing a kind of event this panel is able to generate. Options for this should be provided as static members of the UserControls that implement the RegisteredPanel.</param>
        /// <param name="ROUTED_EVENT_HANDLER">The handler that will be listening to the event.</param>
        void RegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER);

        /// <summary>
        /// UnRegisters an event handler to a particular type of event this panel might generate.
        /// </summary>
        /// <param name="EVENT_HANDLER_TYPE">A constant integer representing a kind of event this panel is able to generate. Options for this should be provided as static members of the UserControls that implement the RegisteredPanel.</param>
        /// <param name="ROUTED_EVENT_HANDLER">The handler that will be removed from the event.</param>
        void UnRegisterEventHandler(int EVENT_HANDLER_TYPE, RoutedEventHandler ROUTED_EVENT_HANDLER);
    }
}
