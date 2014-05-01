using ObdExpress.Global;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
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
                return this._consoleForeground;
            }
            set
            {
                this._consoleForeground = value;
            }
        }

        private Color _consoleBackground = Colors.Black;
        public Color ConsoleBackground
        {
            get
            {
                return this._consoleBackground;
            }
            set
            {
                this._consoleBackground = value;
            }
        }

        private int _consoleBufferedRows = 100;
        public int ConsoleBufferedRows
        {
            get
            {
                return this._consoleBufferedRows;
            }
            set
            {
                this._consoleBufferedRows = value;
            }
        }

        private bool _consoleInfiniteBuffer = false;
        public bool ConsoleInfiniteBuffer
        {
            get
            {
                return this._consoleInfiniteBuffer;
            }
            set
            {
                if (value)
                {
                    this._consoleBufferedRows = -1;
                    this.updBufferedRows.IsEnabled = false;
                    OnPropertyChanged("ConsoleBufferedRows");
                }
                else
                {
                    this._consoleBufferedRows = 100;
                    this.updBufferedRows.IsEnabled = true;
                    OnPropertyChanged("ConsoleBufferedRows");
                }

                this._consoleInfiniteBuffer = value;
            }
        }

        public LoggingConsoleConfig()
        {
            this._consoleForeground = (Color)ColorConverter.ConvertFromString((String)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_FOREGROUND]);
            this._consoleBackground = (Color)ColorConverter.ConvertFromString((String)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BACKGROUND]);
            this._consoleBufferedRows = (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BUFFERED_ROWS];

            InitializeComponent();

            // If buffered rows is infinite, prepare the IntegerUpDown and the checkbox
            if (this._consoleBufferedRows < 0)
            {
                this._consoleInfiniteBuffer = true;
                OnPropertyChanged("ConsoleInfiniteBuffer");
                this.updBufferedRows.IsEnabled = false;
            }

            // Build our color palette
            IEnumerator<KeyValuePair<string, Color>> standardColorEnum = Variables.STANDARD_COLORS.GetEnumerator();
            while (standardColorEnum.MoveNext())
            {
                this._colorPalette.Add(new ColorItem(standardColorEnum.Current.Value, standardColorEnum.Current.Key));
            }

            // Set the color palette as the StandardColors for the two color pickers
            this.cpForeground.StandardColors = this._colorPalette;
            this.cpBackground.StandardColors = this._colorPalette;
        }

        /// <summary>
        /// Handle the Done button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            // Update the Properties file
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_FOREGROUND] = this._consoleForeground.ToString();
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BACKGROUND] = this._consoleBackground.ToString();
            Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BUFFERED_ROWS] = this._consoleBufferedRows;

            // Save the Properties Changes
            Properties.ApplicationSettings.Default.Save();

            // Close this Window
            this.Close();
        }

        /// <summary>
        /// Handle if this window closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            this.Owner.Focus();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
