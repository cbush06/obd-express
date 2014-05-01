using ELM327API.Processing.Interfaces;
using log4net;
using ObdExpress.Global;
using ObdExpress.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

// Look at our configuration for log4net
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ObdExpress
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Get the logger.
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public App()
        {
            this.Startup += this.OnApplicationStartup;
            this.Exit += this.OnApplicationExit;
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            ApplicationSettings.Default.Save();
            ELM327Connection.DestroyConnection();
        }

        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            // Load plugins
            LoadDllPlugins();
        }

        /// <summary>
        /// Attempt to load all DLL plugins.
        /// </summary>
        private void LoadDllPlugins()
        {
            string[] pluginPaths = ((string)ObdExpress.Properties.ApplicationSettings.Default[Variables.SETTINGS_APPLICATION_PLUGINS]).Split(Variables.SETTINGS_SEPARATOR);

            foreach (string nextPath in pluginPaths)
            {
                try
                {
                    Assembly nextPlugin = Assembly.LoadFrom(nextPath.Replace(@"\\", @"\"));

                    foreach (Type nextType in nextPlugin.GetExportedTypes())
                    {
                        if (nextType != null && typeof(IHandler).IsAssignableFrom(nextType))
                        {
                            ELM327Connection.LoadedHandlerTypes.Add(nextType);
                            App.log.Debug("Handler Loaded: " + nextType.Name);
                        }
                    }
                }
                catch (Exception e)
                {
                    App.log.Error("An exception was encountered while loading DLL plugin [" + nextPath + "]:", e);
                    continue;
                }
            }
        }
    }
}
