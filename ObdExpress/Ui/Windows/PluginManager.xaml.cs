using ELM327API.Processing.Interfaces;
using log4net;
using Microsoft.Win32;
using ObdExpress.Global;
using ObdExpress.Ui.DataStructures;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;

namespace ObdExpress.Ui.Windows
{
    /// <summary>
    /// Interaction logic for PluginManager.xaml
    /// </summary>
    public partial class PluginManager : Window, INotifyPropertyChanged
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Collection of Plugins loaded from the Application Properties and/or added by the user.
        /// </summary>
        public ObservableCollection<Plugin> PluginList
        {
            get
            {
                return _pluginList;
            }
            set
            {
                _pluginList = value;
            }
        }
        private ObservableCollection<Plugin> _pluginList = new ObservableCollection<Plugin>();

        /// <summary>
        /// Stores a reference to the selected item of the datagrid.
        /// </summary>
        public Plugin SelectedPlugin
        {
            get
            {
                return _selectedPlugin;
            }
            set
            {
                _selectedPlugin = value;
            }
        }
        private Plugin _selectedPlugin = null;

        public PluginManager()
        {
            string[] pluginPaths = ((string)Properties.ApplicationSettings.Default[Variables.SETTINGS_APPLICATION_PLUGINS]).Split(Variables.SETTINGS_SEPARATOR);

            InitializeComponent();

            foreach (string pluginPath in pluginPaths)
            {
                if (pluginPath.Length > 0)
                {
                    _pluginList.Add(new Plugin(pluginPath));
                }
            }

            OnPropertyChanged("PluginList");
        }

        public void btnDone_Click(object sender, RoutedEventArgs args)
        {
            _selectedPlugin = null;
            Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool? result = null;

            OpenFileDialog fileDlg = new OpenFileDialog();
            fileDlg.Filter = "Dynamic Linked Libraries|*.dll";
            fileDlg.Title = "Add Plugin Assembly...";
            fileDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            result = fileDlg.ShowDialog(this);

            if (result != null && result == true)
            {
                if (ValidateAndAddAssembly(fileDlg.FileName))
                {
                    // Notify the user of the impending restart
                    MessageBox.Show(this, "The application must now restart to ensure the Plugin is fully functional.", "Plugin Added!", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Restart
                    try
                    {
                        Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        PluginManager.log.Error("An exception was thrown while attempting to restart the application as a result of a plugin removal.", ex);
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.None;
            StringBuilder loadedPlugins = new StringBuilder();

            if (_selectedPlugin != null)
            {
                result = MessageBox.Show(this, "This action requires a restart of the program. Are you sure you wish to remove this plugin?", "Are You Sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }
            else
            {
                MessageBox.Show(this, "You must select an item to be removed.", "No Selection Made...", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            if (result == MessageBoxResult.Yes)
            {
                // Remove the item from the list of loaded plugins
                _pluginList.Remove(_selectedPlugin);
                _selectedPlugin = null;

                // Update the Application Setting
                foreach (Plugin nextPlugin in _pluginList)
                {
                    loadedPlugins.Append(nextPlugin.PluginPath + Variables.SETTINGS_SEPARATOR);
                }

                Properties.ApplicationSettings.Default[Variables.SETTINGS_APPLICATION_PLUGINS] = loadedPlugins.ToString();
                Properties.ApplicationSettings.Default.Save();

                // Restart
                try
                {
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                catch (Exception ex)
                {
                    PluginManager.log.Error("An exception was thrown while attempting to restart the application as a result of a plugin removal.", ex);
                }
            }
        }

        private bool ValidateAndAddAssembly(string path)
        {
            Assembly selectedAssembly = null;
            string productName = "";
            string assemblyName = "";
            string loadedAssemblies = "";

            try
            {
                selectedAssembly = Assembly.ReflectionOnlyLoadFrom(path);
            }
            catch (Exception e)
            {
                PluginManager.log.Error("An exception was thrown while trying to load assembly at [" + path + "].", e);
                MessageBox.Show(this, "An exception was thrown while trying to load the selected assembly.", "Exception Thrown (" + e.GetType().Name + ")", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (selectedAssembly != null)
            {
                // Get the Assembly Name
                assemblyName = selectedAssembly.GetName().Name;

                // Ensure this is Meant for OBD Express by checking the AssemblyProduct attribute
                foreach (CustomAttributeData nextData in selectedAssembly.GetCustomAttributesData())
                {
                    if (nextData.AttributeType == typeof(AssemblyProductAttribute))
                    {
                        if(nextData.ConstructorArguments.Count > 0)
                        {
                            productName = nextData.ConstructorArguments[0].Value.ToString();
                            break;
                        }
                    }
                }
            }

            // Is it actually an OBD Express plugin?
            if (!(productName.ToLower().Equals("obdexpress")))
            {
                MessageBox.Show(this, "The selected assembly is not an OBD Express plugin.\nProduct Name Reported: " + productName.ToLower() + "\nProduct Name Expected: obdexpress", "Not an OBD Express Plugin...", MessageBoxButton.OK, MessageBoxImage.Error);
                PluginManager.log.Error("An attempt was made to load an assembly that is not an OBD Express plugin at: " + path);
                return false;
            }

            // Has the plugin already been loaded?
            foreach (Plugin nextPlugin in _pluginList)
            {
                if (nextPlugin.PluginAssemblyName.Equals(assemblyName))
                {
                    MessageBox.Show(this, "A plugin with the name [" + assemblyName + "] already exists.", "Plugin Already Exists...", MessageBoxButton.OK, MessageBoxImage.Error);
                    PluginManager.log.Error("An attempt was made to load an assembly that already exists. Name = " + assemblyName + "; Path = " + path);
                    return false;
                }
            }

            // OK!!! We made it, now let's load it into our runtime environment
            selectedAssembly = Assembly.LoadFrom(path);

            // Now, register the handlers with the ELM327
            foreach (Type nextType in selectedAssembly.GetExportedTypes())
            {
                if (typeof(IHandler).IsAssignableFrom(nextType))
                {
                    ELM327Connection.LoadedHandlerTypes.Add(nextType);
                }
            }

            // Next, ave it into our Application Settings
            loadedAssemblies = (string)Properties.ApplicationSettings.Default[Variables.SETTINGS_APPLICATION_PLUGINS];
            loadedAssemblies += Variables.SETTINGS_SEPARATOR + path;
            Properties.ApplicationSettings.Default[Variables.SETTINGS_APPLICATION_PLUGINS] = loadedAssemblies;
            Properties.ApplicationSettings.Default.Save();

            // Last, let's add the plugin to the User Inteface
            _pluginList.Add(new Plugin(path));
            OnPropertyChanged("PluginList");

            // That's all folks!
            return true;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
