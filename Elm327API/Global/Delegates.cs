using ELM327API.Processing.DataStructures;
using System.IO.Ports;

namespace ELM327API
{
    /// <summary>
    /// Delegate to be used by IHandlers for notifying listeners.
    /// </summary>
    /// <param name="args">Args provided that contain processed data.</param>
    public delegate void NoReturnWithELM327ListenerEventArgsParam(ELM327ListenerEventArgs args);

    /// <summary>
    /// Delegate that has a void return value and a single string parameter.
    /// </summary>
    /// <param name="stringParameter">String parameter.</param>
    public delegate void NoReturnWithStringParam(string stringParameter);

    /// <summary>
    /// Delegate that has a void return value and a single boolean parameter.
    /// </summary>
    /// <param name="success">Boolean parameter.</param>
    public delegate void NoReturnWithBoolParam(bool boolParameter);

    /// <summary>
    /// Delegate that has a void return value and a single SerialPort parameter.
    /// </summary>
    /// <param name="port">SerialPort parameter.</param>
    public delegate void NoReturnWithSerialPortParam(SerialPort serialPortParameter);

    /// <summary>
    /// Delegate that has a void return value and a double parameter.
    /// </summary>
    /// <param name="doubleParameter">Double parameter.</param>
    public delegate void NoReturnWithDoubleParam(double doubleParameter);

    /// <summary>
    /// Delegate that has a void return value and a long parameter.
    /// </summary>
    /// <param name="doubleParameter">Long parameter.</param>
    public delegate void NoReturnWithLongParam(long doubleParameter);

    /// <summary>
    /// Delegate that has a void return value and no paramters.
    /// </summary>
    public delegate void NoReturnWithNoParam();
}
