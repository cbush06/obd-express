using System;
using System.IO.Ports;

namespace ObdExpress.Global
{
    public delegate void NoReturnWithStringParam(String stringParam);
    public delegate void NoReturnWithSerialPortParam(SerialPort connection);
    public delegate void NoReturnWithNoParams();
}
