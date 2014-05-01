using log4net;
using ObdExpress.Global;
using ObdExpress.Ui.DataStructures;
using ObdExpress.Ui.UserControls.Interfaces;
using ObdExpress.Ui.UserControls.PanelCollections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ObdExpress.Ui.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// The navigation menu on the left of the MainWindow.
        /// </summary>
        private NavigationMenu _navMenu = new NavigationMenu();

        /// <summary>
        /// Get the logger.
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The collection of IRegisteredPanel objects selected for display.
        /// </summary>
        private IPanelCollection _selectedPanelCollection;

        /// <summary>
        /// Stores the already created Panel Collections.
        /// </summary>
        private Dictionary<String, IPanelCollection> _panelCollections = new Dictionary<string, IPanelCollection>();

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
        /// Property backing the title shown above all the panels.
        /// </summary>
        private String _selectedMenuItemLabel = "";
        public String SelectedMenuItemLabel
        {
            get
            {
                return this._selectedMenuItemLabel;
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
        /// Store a reference to the one and only Logging Console.
        /// </summary>
        private LoggingConsole _loggingConsole = null;

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
            this.miConnect.IsEnabled = false;
            this.miDisconnect.IsEnabled = true;
            this.tbBtnConnect.IsEnabled = false;
            this.tbBtnDisconnect.IsEnabled = true;
        }

        /// <summary>
        /// Execute actions when a connection is disconnected from an ELM327.
        /// </summary>
        private void ConnectionDestroyed()
        {
            this.lblStatus.Content = "Disconnected.";
            this.imgConnection.Source = new BitmapImage(new Uri("pack://application:,,,/ObdExpress;component/Ui/Images/disconnect.png"));
            this.miConnect.IsEnabled = true;
            this.miDisconnect.IsEnabled = false;
            this.tbBtnConnect.IsEnabled = true;
            this.tbBtnDisconnect.IsEnabled = false;
        }

        /// <summary>
        /// Handles post-render tasks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuildMenu()
        {
            DataStructures.MenuItem newMenuItem;

            /*
             * Build the NavigationMenu's data structure
             */
            TopMenu mainMenu = new TopMenu(Variables.TOP_MENU_ITEM_ID_MAIN, "Main");
            mainMenu.AddMenuItem(new DataStructures.MenuItem(Variables.MENU_ITEM_ID_HOME, "Home", "pack://application:,,,/ObdExpress;component/UI/Images/home_32.png"));
            mainMenu.AddMenuItem(new DataStructures.MenuItem(Variables.MENU_ITEM_ID_TROUBLE_CODES, "Troubleshooting", "pack://application:,,,/ObdExpress;component/UI/Images/warning_32.png"));

            TopMenu configMenu = new TopMenu(Variables.TOP_MENU_ITEM_ID_CONFIGURATION, "Configuration");
            configMenu.AddMenuItem(new DataStructures.MenuItem(Variables.MENU_ITEM_ID_CONFIGURATION, "Connection", "pack://application:,,,/ObdExpress;component/Ui/Images/connect.png"));

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

            // Now that the menu is built and ready, notify this window to display the content
            // corresponding to the selected Menu Item (in this case, the Home page).
            NavigationMenuEventHandler(this._navMenu, NavigationMenuEventType.MENUITEM_SELECTED);
        }

        /// <summary>
        /// When a menu item is selected, a top-level menu is selected, or the menu is modified, handle this event.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="type"></param>
        private void NavigationMenuEventHandler(NavigationMenu menu, NavigationMenuEventType type)
        {
            if (type == NavigationMenuEventType.MENU_MODIFIED || type == NavigationMenuEventType.TOPMENU_SELECTED)
            {
                this.RenderNavigationMenu();
            }

            if (type == NavigationMenuEventType.MENUITEM_SELECTED || type == NavigationMenuEventType.TOPMENU_SELECTED)
            {
                if (!(this._selectedMenuItemLabel.Equals(menu.SelectedMenuItem.Label)))
                {
                    // Change the Main Title
                    this._selectedMenuItemLabel = menu.SelectedMenuItem.Label;
                    OnPropertyChanged("SelectedMenuItemLabel");

                    // Change the Panel Collection
                    UpdatePanelsFromCollection(GetPanelCollectionForMenuItemId(menu.SelectedMenuItem.Id));
                }
            }
        }

        /// <summary>
        /// Retrieve the IPanelCollection associated with the menu ID if it is already created; otherwise, create it and then return it.
        /// </summary>
        /// <param name="menuItemId">ID of the selected menu item.</param>
        /// <returns>IPanelCollection for the selected menu item.</returns>
        private IPanelCollection GetPanelCollectionForMenuItemId(String menuItemId)
        {
            IPanelCollection returnValue = null;
            Type panelCollectionType = null;

            // Translate the MenuItem ID into an IPanelCollection Type
            if (menuItemId.Equals(Variables.MENU_ITEM_ID_HOME))
            {
                panelCollectionType = typeof(HomePanelCollection);
            }
            else if (menuItemId.Equals(Variables.MENU_ITEM_ID_TROUBLE_CODES))
            {
                panelCollectionType = typeof(TroubleCodePanelCollection);
            }
            else if (menuItemId.Equals(Variables.MENU_ITEM_ID_CONFIGURATION))
            {
                panelCollectionType = typeof(ConfigurationPanelCollection);
            }

            // If this type does not already exist in the panel collections Dictionary,
            // create a new instance and add it. If it does, return that instance.
            if (!(this._panelCollections.ContainsKey(menuItemId)))
            {
                returnValue = (IPanelCollection)Activator.CreateInstance(panelCollectionType);
                this._panelCollections.Add(menuItemId, returnValue);
            }
            else
            {
                this._panelCollections.TryGetValue(menuItemId, out returnValue);
            }

            return returnValue;
        }

        /// <summary>
        /// Re-render the panels displayed from the selected panel collection.
        /// </summary>
        private void UpdatePanelsFromCollection(IPanelCollection panelCollection)
        {
            GridSplitter gridSplitter;
            System.Windows.Controls.MenuItem menuItem;

            // Prepare the Panels for Removal
            if (this._selectedPanelCollection != null)
            {
                foreach (IRegisteredPanel nextPanel in this._selectedPanelCollection.Panels)
                {
                    // Notify them to Pause Monitoring
                    nextPanel.PauseMonitoring();

                    // Unregister this MainWindow's event handlers from the Panel's events
                    nextPanel.UnRegisterEventHandler(Variables.REGISTERED_EVENT_TYPE_HIDE_PANEL, RemovePanelFromDisplay);
                    nextPanel.UnRegisterEventHandler(Variables.REGISTERED_EVENT_TYPE_SHOW_PANEL, AddPanelToDisplay);
                }
            }

            // Change Collections
            this._selectedPanelCollection = panelCollection;

            // Clear out the Content Area
            this.ContentAreaGrid.Children.Clear();
            this.ContentAreaGrid.RowDefinitions.Clear();

            // Remove all menu items
            this.menAvailablePanels.Items.Clear();

            // Build the Row Definitions for the Home Panels and Add the panels
            foreach (IRegisteredPanel nextPanel in this._selectedPanelCollection.Panels)
            {
                // Add the panel to available panels menu
                menuItem = new System.Windows.Controls.MenuItem();
                menuItem.Header = new Label() { Content = nextPanel.Title, Padding = new Thickness(0.0d) };
                menuItem.IsCheckable = true;
                menuItem.IsChecked = nextPanel.IsShown;

                // Register the panel's event handlers to the MenuItem's events
                menuItem.Checked += nextPanel.ShowPanel;
                menuItem.Unchecked += nextPanel.HidePanel;

                // Register this MainWindow's event handlers to the Panel's events
                nextPanel.RegisterEventHandler(Variables.REGISTERED_EVENT_TYPE_HIDE_PANEL, RemovePanelFromDisplay);
                nextPanel.RegisterEventHandler(Variables.REGISTERED_EVENT_TYPE_SHOW_PANEL, AddPanelToDisplay);

                this.menAvailablePanels.Items.Add(menuItem);

                // Add the Panel to be displayed
                if (nextPanel.IsShown)
                {
                    // Add the next panel
                    ((UserControl)nextPanel).SetValue(Grid.RowProperty, this.ContentAreaGrid.RowDefinitions.Count);
                    ((UserControl)nextPanel).SetValue(Grid.ColumnProperty, 0);
                    this.ContentAreaGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    this.ContentAreaGrid.Children.Add((UserControl)nextPanel);

                    // Unpause the next panel
                    if (nextPanel.IsPaused)
                    {
                        nextPanel.UnPauseMonitoring();
                    }
                    else
                    {
                        nextPanel.StartMonitoring(ELM327Connection.Connection);
                    }

                    // Add a GridSplitter
                    gridSplitter = new GridSplitter() { Height = 4.0d, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 3, 0, 3), Background = new SolidColorBrush(Colors.Transparent) };
                    gridSplitter.SetValue(Grid.RowProperty, this.ContentAreaGrid.RowDefinitions.Count);
                    gridSplitter.SetValue(Grid.ColumnProperty, 0);
                    this.ContentAreaGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    this.ContentAreaGrid.Children.Add(gridSplitter);
                }
            }
        }

        /// <summary>
        /// Remove a panel from the display grid.
        /// </summary>
        private void RemovePanelFromDisplay(object sender, RoutedEventArgs args)
        {
            // Removed Panel
            IRegisteredPanel removedPanel = (IRegisteredPanel)sender;

            // Remove the Panel
            for (int i = 0; i < this.ContentAreaGrid.Children.Count; i++)
            {
                if (this.ContentAreaGrid.Children[i] == sender)
                {
                    // Remove the Panel and the GridSplitter after it
                    this.ContentAreaGrid.Children.RemoveAt(i);
                    this.ContentAreaGrid.Children.RemoveAt(i);

                    // Remove two Row Definitions
                    this.ContentAreaGrid.RowDefinitions.RemoveAt(i);
                    this.ContentAreaGrid.RowDefinitions.RemoveAt(i);

                    // Re-index the Children's Row Indexes
                    for (int j = 0; j < this.ContentAreaGrid.Children.Count; j++)
                    {
                        this.ContentAreaGrid.Children[j].SetValue(Grid.RowProperty, j);
                    }

                    break;
                }
            }

            // Ensure the menu item is unchecked
            for (int i = 0; i < this.menAvailablePanels.Items.Count; i++)
            {
                System.Windows.Controls.MenuItem menuItem = (System.Windows.Controls.MenuItem)this.menAvailablePanels.Items[i];
                string menuItemLabel = (string)(((Label)(menuItem.Header)).Content);

                if (menuItemLabel.Equals(removedPanel.Title) && menuItem.IsChecked)
                {
                    menuItem.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// Add a panel to the display grid.
        /// </summary>
        private void AddPanelToDisplay(object sender, RoutedEventArgs args)
        {
            GridSplitter gridSplitter;
            IRegisteredPanel nextPanel = (IRegisteredPanel)sender;
            
            // Add the next panel
            ((UserControl)nextPanel).SetValue(Grid.RowProperty, this.ContentAreaGrid.RowDefinitions.Count);
            this.ContentAreaGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            this.ContentAreaGrid.Children.Add((UserControl)nextPanel);

            // Add a GridSplitter
            gridSplitter = new GridSplitter() { Height = 4.0d, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 3, 0, 3), Background = new SolidColorBrush(Colors.Transparent) };
            gridSplitter.SetValue(Grid.RowProperty, this.ContentAreaGrid.RowDefinitions.Count);
            gridSplitter.SetValue(Grid.ColumnProperty, 0);
            this.ContentAreaGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            this.ContentAreaGrid.Children.Add(gridSplitter);
        }

        /// <summary>
        /// Displays the Elm327 Connector dialog window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileMenu_Connect_Click(object sender, RoutedEventArgs e)
        {
            ELM327Connector connectorDialog = new ELM327Connector();
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

        private void ToolsMenu_Plugins_Click(object sender, RoutedEventArgs args)
        {
            PluginManager pluginsWindow = new PluginManager();
            pluginsWindow.Owner = MainWindow._topWindow;
            pluginsWindow.ShowDialog();
        }

        /// <summary>
        /// Open the Logging Console when the Console icon is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Toolbar_Console_Click(object sender, RoutedEventArgs e)
        {
            if (this._loggingConsole == null)
            {
                this._loggingConsole = new LoggingConsole();
                this._loggingConsole.Closed += Console_Closed;
                this._loggingConsole.Owner = MainWindow._topWindow;
                this._loggingConsole.Show();
            }
        }

        /// <summary>
        /// Set our reference to the Logging Console to null if it is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Console_Closed(object sender, EventArgs e)
        {
            this._loggingConsole = null;
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
            ELM327Connection.DestroyConnection();
        }


        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
