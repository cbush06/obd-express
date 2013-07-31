using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM327API.Connection.Interfaces
{
    public interface IConnector
    {
        /// <summary>
        /// Notifies the user which port is being checked
        /// </summary>
        event NoReturnWithStringParam CheckingPort;

        /// <summary>
        /// Passes intermittent messages to the user
        /// </summary>
        event NoReturnWithStringParam UpdateMessages;

        /// <summary>
        /// Indicates if the PortConnector being used is done
        /// </summary>
        event NoReturnWithBoolParam ConnectionComplete;

        /// <summary>
        /// If a connection was established, passes the successful port back
        /// </summary>
        event NoReturnWithSerialPortParam ConnectionEstablished;

        /// <summary>
        /// Indicates if the current port was successful or not
        /// </summary>
        event NoReturnWithBoolParam PortSuccess;

        /// <summary>
        /// If the GetSerialPort method has already been started, this notifies it to exit. This is useful if
        /// the method is ran on another thread.
        /// </summary>
        void Kill();

        /// <summary>
        /// Attempts to verify that the port this connector was created for has an ELM327 attached to it.
        /// If verification is successful, the ConnectionEstablished event is called and all listeners
        /// are passed a reference to the SerialPort object.
        /// </summary>
        void GetSerialPort();
    }
}
