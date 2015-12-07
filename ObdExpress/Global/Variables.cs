using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace ObdExpress.Global
{
    public class Variables
    {
        /// <summary>
        /// Set by main window on close to alert any running threads to exit.
        /// </summary>
        public static bool ShouldClose = false;

        /// <summary>
        /// Used to separate setting values
        /// </summary>
        public static char SETTINGS_SEPARATOR = ';';

        /// <summary>
        /// Constants used to allow communication between the parent window and Registered Panels.
        /// </summary>
        public const short REGISTERED_EVENT_TYPE_HIDE_PANEL                 = 0x0000;
        public const short REGISTERED_EVENT_TYPE_SHOW_PANEL                 = 0x0001;
        public const short REGISTERED_EVENT_TYPE_PANEL_COLLECTIONS_CHANGING = 0x0002;

        /*
         * Logging Pipe's Name.
         */
        public const string MAP_KEY_LOG4NET_LOG_TAP_INSTANCE = "log4net_log_tap";

        /*
         * IDs used by menu items.
         */
        public const String TOP_MENU_ITEM_ID_MAIN = "#topmainmenu";
        public const String TOP_MENU_ITEM_ID_CONFIGURATION = "#topconfigurationmenu";
        public const String MENU_ITEM_ID_HOME = "#home";
        public const String MENU_ITEM_ID_TROUBLE_CODES = "#troublecodes";
        public const String MENU_ITEM_ID_CONFIGURATION = "#configuration";

        /*
         * Settings keys.
         */
        public const String SETTINGS_DASHBOARD_HANDLERS = "settings_dashboard_handlers";
        public const String SETTINGS_DASHBOARD_COLUMNS = "settings_dashboard_columns";
        public const String SETTINGS_CONSOLE_FOREGROUND = "settings_console_foreground";
        public const String SETTINGS_CONSOLE_BACKGROUND = "settings_console_background";
        public const String SETTINGS_CONSOLE_BUFFERED_ROWS = "settings_console_buffered_rows";
        public const String SETTINGS_APPLICATION_PLUGINS = "settings_application_plugins";
        public const String SETTINGS_CONNECTION_BAUDRATE = "settings_connection_baudrate";
        public const String SETTINGS_CONNECTION_DATABITS = "settings_connection_databits";
        public const String SETTINGS_CONNECTION_PARITY = "settings_connection_parity";
        public const String SETTINGS_CONNECTION_STOPBITS = "settings_connection_stopbits";
        public const String SETTINGS_CONNECTION_READ_TIMEOUT = "settings_connection_read_timeout";
        public const String SETTINGS_CONNECTION_WRITE_TIMEOUT = "settings_connection_write_timeout";
        public const String SETTINGS_CONNECTION_DEVICEDESCRIPTION = "settings_connection_devicedescription";
        public const String SETTINGS_VEHICLEINFORMATION_HANDLERS = "settings_vehicleinformation_handlers";

        /*
         * Standard Colors
         */
        public static ReadOnlyDictionary<String, Color> STANDARD_COLORS
        {
            get
            {
                return _standardColors;
            }
        }

        private static ReadOnlyDictionary<String, Color> _standardColors = new ReadOnlyDictionary<string, Color>(
            new Dictionary<string, Color>
            {
                { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) },
                { "Green", Color.FromArgb(0xFF, 0x00, 0xFF, 0x00) },
                { "Blue", Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) },
                { "Yellow", Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00) },
                { "Orange", Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00) },
                { "Gold", Color.FromArgb(0xFF, 0xFF, 0xD7, 0x00) },
                { "Dark Red", Color.FromArgb(0xFF, 0x8B, 0x00, 0x00) },
                { "Dark Green", Color.FromArgb(0xFF, 0x00, 0x64, 0x00) },
                { "Dark Blue", Color.FromArgb(0xFF, 0x00, 0x00, 0x8B) },
                { "Dark Magenta", Color.FromArgb(0xFF, 0x8B, 0x00, 0x8B) },
                { "Dark Orange", Color.FromArgb(0xFF, 0xFF, 0x8C, 0x00) },
                { "White", Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) },
                { "Black", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) },
                { "Silver", Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0) }
            }
        );
    }
}
