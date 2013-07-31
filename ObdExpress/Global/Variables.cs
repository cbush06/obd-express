using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObdExpress.Global
{
    public class Variables
    {
        /// <summary>
        /// Set by main window on close to alert any running threads to exit.
        /// </summary>
        public static bool ShouldClose = false;
    }
}
