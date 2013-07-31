using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ObdExpress.Ui.UserControls.Interfaces
{
    public interface IRegisteredPanel
    {
        /// <summary>
        /// Instructs the RegisteredPanel to start monitoring the bus.
        /// </summary>
        void StartMonitoring();

        /// <summary>
        /// Instructs the RegisteredPanel to stop monitoring the bus.
        /// </summary>
        void StopMonitoring();

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
