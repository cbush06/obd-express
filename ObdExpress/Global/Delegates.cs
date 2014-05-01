using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObdExpress.Global
{
    public delegate void NoReturnWithStringParam(String stringParam);
    public delegate void NoReturnWithSerialPortParam(SerialPort connection);
    public delegate void NoReturnWithNoParams();
}
