using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELM327API
{
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
    /// Delegate that has a void return value and no paramters.
    /// </summary>
    public delegate void NoReturnWithNoParam();
}
