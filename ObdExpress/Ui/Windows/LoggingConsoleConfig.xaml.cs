using ObdExpress.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace ObdExpress.Ui.Windows
{
    /// <summary>
    /// Interaction logic for LoggingConsoleConfig.xaml
    /// </summary>
    public partial class LoggingConsoleConfig : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Color Palette to use for color pickers.
        /// </summary>
        private ObservableCollection<ColorItem> _colorPalette = new ObservableCollection<ColorItem>();

        private Color _consoleForeground = Colors.White;
        public Color ConsoleForeground
        {
            get
            {
                return _consoleForeground;
            }
            set
            {
                _consoleForeground = value;
            }
        }

        private Color _consoleBackground = Colors.Black;
        public Color ConsoleBackground
        {
            get
            {
                return _consoleBackground;
            }
            set
            {
                _consoleBackground = value;
            }
        }

        private int _consoleBufferedRows = 100;
        public int ConsoleBufferedRows
        {
            get
            {
                return _consoleBufferedRows;
            }
            set
            {
                _consoleBufferedRows = value;
            }
        }

        private bool _consoleInfiniteBuffer = false;
        public bool ConsoleInfiniteBuffer
        {
            get
            {
                return _consoleInfiniteBuffer;
            }
            set
            {
                if (value)
                {
                    _consoleBufferedRows = -1;
                    updBufferedRows.IsEnabled = false;
                    OnPropertyChanged("ConsoleBufferedRows");
                }
                else
                {
                    _consoleBufferedRows = 100;
                    updBufferedRows.IsEnabled = true;
                    OnPropertyChanged("ConsoleBufferedRows");
                }

                _consoleInfiniteBuffer = value;
            }
        }

        public LoggingConsoleConfig()
        {
            _consoleForeground = (Color)ColorConverter.ConvertFromString((String)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_FOREGROUND]);
            _consoleBackground = (Color)ColorConverter.ConvertFromString((String)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BACKGROUND]);
            _consoleBufferedRows = (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BUFFERED_ROWS];

            InitializeComponent();

            // If buffered rows is infinite, prepare the IntegerUpDown and the checkbox
            if (_consoleBufferedRows < 0)
            {
                _consoleInfiniteBuffer = true;
                OnPropertyChanged("ConsoleInfiniteBuffer");
                updBufferedRows.IsEnabled = false;
            }

            // Build our color palette
            IEnumerator<KeyValuePair<string, Color>> standardColorEnum = Variables.STANDARD_COLORS.GetEnumerator();
            while (standardColorEnum.MoveNext())
            {
                _colorPalette.Add(new ColorItem(standardColorEnum.Current.Value, standardColorEnum.Current.Key));
            }

            // Set the color palette as the StandardColors for the two color pickers
            cpForeground.StandardColors = _colorPalette;
            cpBackground.StandardColors = _colorPalette;
        }

        /// <summary>
        /// Handle the Done button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            // Update the Properties file
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_FOREGROUND] = _consoleForeground.ToString();
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BACKGROUND] = _consoleBackground.ToString();
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BUFFERED_ROWS] = _consoleBufferedRows;

            // Save the Properties Changes
            Properties.ApplicationSettings.Default.Save();

            // Close this Window
            Close();
        }

        /// <summary>
        /// Handle if this window closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Owner.Focus();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
