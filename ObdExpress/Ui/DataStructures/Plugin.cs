using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObdExpress.Ui.DataStructures
{
    public class Plugin
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Path to the Plugin File.
        /// </summary>
        public string PluginPath
        {
            get { return _pluginPath; }
        }
        private string _pluginPath = "";

        /// <summary>
        /// Read-only property indicating if the Plugin File exists at the given path.
        /// </summary>
        public bool PluginExists
        {
            get { return _pluginExists; }
        }
        private bool _pluginExists = false;

        /// <summary>
        /// Read-only property providing the assembly name of this plugin file.
        /// </summary>
        public string PluginAssemblyName
        {
            get { return _pluginAssemblyName; }
        }
        private string _pluginAssemblyName = "";

        /// <summary>
        /// Create a new Plugin object.
        /// </summary>
        /// <param name="pluginPath">Path to the PLugin File.</param>
        public Plugin(string pluginPath)
        {
            Assembly pluginAssembly = null;

            _pluginPath = pluginPath;
            _pluginExists = File.Exists(_pluginPath);

            try
            {
                pluginAssembly = Assembly.ReflectionOnlyLoadFrom(_pluginPath);
            }
            catch (Exception e)
            {
                Plugin.log.Error("An exception was thrown while attempting to load an assembly from [" + _pluginPath + "].", e);
            }

            if (pluginAssembly != null)
            {
                _pluginAssemblyName = pluginAssembly.GetName().Name;
            }
        }
    }
}
