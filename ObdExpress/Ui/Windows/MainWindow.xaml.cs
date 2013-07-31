using ObdExpress.Global;
using ObdExpress.Ui.DataStructures;
using ObdExpress.Ui.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObdExpress.Ui.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The navigation menu on the left of the MainWindow.
        /// </summary>
        private NavigationMenu _navMenu = new NavigationMenu();

        /// <summary>
        /// Stores the list of MenuItem buttons shown in the NavigationMenu.
        /// </summary>
        private ObservableCollection<DataStructures.MenuItem> _menuItems = new ObservableCollection<DataStructures.MenuItem>();
        public ObservableCollection<DataStructures.MenuItem> MenuItems
        {
            get
            {
                return this._menuItems;
            }
        }

        /// <summary>
        /// Stores the list of Menu Selection buttons show below the NavigationMenu.
        /// </summary>
        private ObservableCollection<DataStructures.TopMenu> _topMenuItems = new ObservableCollection<DataStructures.TopMenu>();
        public ObservableCollection<DataStructures.TopMenu> TopMenuItems
        {
            get
            {
                return this._topMenuItems;
            }
        }

        /// <summary>
        /// Make this MainWindow available to the rest of the application. It may be used as the owner of dialogs and other secondary windows.
        /// </summary>
        private static Window _topWindow = null;
        public static Window TopWindow
        {
            get
            {
                return MainWindow._topWindow;
            }
        }

        /// <summary>
        /// Default constructor for the MainWindow.
        /// </summary>
        public MainWindow()
        {
            // Initialize all of the standard controls in the window
            InitializeComponent();

            // Build our menu
            this.BuildMenu();

            // Set the TopWindow
            MainWindow._topWindow = this;
        }

        /// <summary>
        /// Handle actions once this Window has successfully loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Register the window with global events
            ELM327Connection.ConnectionEstablishedEvent += this.ConnectionEstablished;
            ELM327Connection.ConnectionDestroyedEvent += this.ConnectionDestroyed;
        }

        /// <summary>
        /// Execute actions when a connection is established with an ELM327.
        /// </summary>
        /// <param name="connection">Port on which the ELM327 connection is established.</param>
        private void ConnectionEstablished(SerialPort connection)
        {
            this.lblStatus.Content = "Connected.";
            this.imgConnection.Source = new BitmapImage(new Uri("pack://application:,,,/ObdExpress;component/Ui/Images/connect.png"));
            this.miDisconnect.IsEnabled = true;
        }

        /// <summary>
        /// Execute actions when a connection is disconnected from an ELM327.
        /// </summary>
        private void ConnectionDestroyed()
        {
            this.lblStatus.Content = "Disconnected.";
            this.imgConnection.Source = new BitmapImage(new Uri("pack://application:,,,/ObdExpress;component/Ui/Images/disconnect.png"));
            this.miDisconnect.IsEnabled = false;
        }

        /// <summary>
        /// Handles post-render tasks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuildMenu()
        {
            /*
             * Build the NavigationMenu's data structure
             */
            TopMenu mainMenu = new TopMenu("#topMain", "Main");
            mainMenu.AddMenuItem(new DataStructures.MenuItem("#home", "Home", "pack://application:,,,/ObdExpress;component/UI/Images/home_32.png"));
            mainMenu.AddMenuItem(new DataStructures.MenuItem("#troublecodes", "Trouble Codes", "pack://application:,,,/ObdExpress;component/UI/Images/warning_32.png"));

            TopMenu configMenu = new TopMenu("#topConfig", "Configuration");
            configMenu.AddMenuItem(new DataStructures.MenuItem("#config", "Config", "pack://application:,,,/ObdExpress;component/UI/Images/tools_32.png"));

            this._navMenu.AddTopMenu(mainMenu);
            this._navMenu.AddTopMenu(configMenu);

            /*
             * Attach our event handler
             */
            this._navMenu.AddNavigationMenuEventListener(this.NavigationMenuEventHandler);

            /*
             * Render the NavigationMenu in the GUI
             */
            this.RenderNavigationMenu();
        }

        /// <summary>
        /// Render the NavigationMenu according to the state of the datastructure that backs it.
        /// </summary>
        private void RenderNavigationMenu()
        {
            IEnumerator<TopMenu> tEnum = this._navMenu.GetEnumerator();
            IEnumerator<DataStructures.MenuItem> mEnum;

            this._topMenuItems.Clear();

            while (tEnum.MoveNext())
            {
                // Add the TopMenu
                this._topMenuItems.Add(tEnum.Current);

                if (tEnum.Current.IsSelected)
                {
                    mEnum = tEnum.Current.GetEnumerator();
                    this._menuItems.Clear();

                    while (mEnum.MoveNext())
                    {
                        this._menuItems.Add(mEnum.Current);
                    }
                }
            }

        }

        private void NavigationMenuEventHandler(NavigationMenu menu, NavigationMenuEventType type)
        {
            if (type == NavigationMenuEventType.MENU_MODIFIED || type == NavigationMenuEventType.TOPMENU_SELECTED)
            {
                this.RenderNavigationMenu();
            }
        }

        /// <summary>
        /// Displays the Elm327 Connector dialog window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileMenu_Connect_Click(object sender, RoutedEventArgs e)
        {
            Elm327Connector connectorDialog = new Elm327Connector();
            connectorDialog.Owner = MainWindow.TopWindow;
            connectorDialog.ShowDialog();
        }

        /// <summary>
        /// Closes a connection to an ELM327.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileMenu_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            ELM327Connection.DestroyConnection();
        }

        /// <summary>
        /// Handles the closing of the application as a result of the File Menu's Exit option being selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileMenu_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        /// <summary>
        /// Handles clean-up tasks before closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Alerts any running threads to exit.
            Variables.ShouldClose = true;
        }

    }
}
