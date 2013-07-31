using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObdExpress.Global
{
    // Delegates for notifying listeners of the actions of the connection managed by the ELM327Connection class.
    public delegate void NoReturnWithSerialPortParam(SerialPort connection);
    public delegate void NoReturnWithNoParams();
}
