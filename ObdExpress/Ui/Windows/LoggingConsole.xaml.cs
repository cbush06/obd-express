using Microsoft.Win32;
using ObdExpress.Global;
using System;
using System.ComponentModel;
using System.Windows;
using log4net;
using System.IO.Pipes;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text;

namespace ObdExpress.Ui.Windows
{
    /// <summary>
    /// Interaction logic for LoggingConsole.xaml
    /// </summary>
    public partial class LoggingConsole : Window, INotifyPropertyChanged
    {
        private int _consoleBufferedRows = 100;

        private bool _shouldMonitoringStop = true;
        private bool _scrollLock = false;
        private LogTapAppender _log4NetAppender = null;
        private Semaphore _pipeStreamSemaphore = new Semaphore(1, 1);
        private byte[] _pipeBuffer = new byte[4096];

        private BitmapImage _imgStart = new BitmapImage(new Uri("/Ui/Images/Gnome-Media-Playback-Start-32.png", UriKind.Relative));
        private BitmapImage _imgStop = new BitmapImage(new Uri("/Ui/Images/Gnome-Media-Playback-Stop-32.png", UriKind.Relative));

        private StringBuilder _loggingBuffer = new StringBuilder();

        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LoggingConsole()
        {
            InitializeComponent();
            this.UpdateSettings();
        }

        ~LoggingConsole()
        {
            if (_log4NetAppender != null)
            {
                _log4NetAppender.MessageLogging -= LogIncomingMessage;
            }
        }

        /// <summary>
        /// Start Logging to the Console.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (_shouldMonitoringStop)
            {
                txtStartStopAccessText.Text = "Stop";
                imgStartStopIcon.Source = _imgStop;
                _shouldMonitoringStop = false;
                StartMonitoringLoggingOutput();
            }
            else
            {
                txtStartStopAccessText.Text = "Start";
                imgStartStopIcon.Source = _imgStart;
                _shouldMonitoringStop = true;
                StopMonitoringLoggingOutput();
            }
        }

        /// <summary>
        /// Select all the content in the console.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            this.txtLoggingConsole.Focus();
            this.txtLoggingConsole.SelectAll();
        }

        /// <summary>
        /// Copy the selected content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBtnCopy_Click(object sender, RoutedEventArgs e)
        {
            this.txtLoggingConsole.Focus();
            this.txtLoggingConsole.Copy();
        }

        /// <summary>
        /// Clear the console.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBtnClear_Click(object sender, RoutedEventArgs e)
        {
            _loggingBuffer.Clear();
            this.txtLoggingConsole.Focus();
            this.txtLoggingConsole.Clear();
        }

        /// <summary>
        /// Save the content of the Console.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool? saveDialogResult = null;
            SaveFileDialog fileDialog = new SaveFileDialog();

            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            fileDialog.Title = "Save Logging Data...";
            fileDialog.Filter = "All Files|*.*";
            fileDialog.DefaultExt = "*.*";
            fileDialog.FileName = "OBDExpress_Logging_Dump_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

            saveDialogResult = fileDialog.ShowDialog(this);

            if (saveDialogResult != null && saveDialogResult == true)
            {
                try
                {
                    StreamWriter newFile = new StreamWriter(File.OpenWrite(fileDialog.FileName));
                    newFile.Write(_loggingBuffer.ToString());
                    newFile.Close();
                }
                catch (IOException ioe)
                {
                    LoggingConsole.log.Error("Error occurred while attempting to save Console output to file.", ioe);
                    MessageBox.Show(this, "Error occurred while attempting to save Console output to file.", "IOException", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                LoggingConsole.log.Info("Logging Console successfully saved to file: " + fileDialog.FileName);
            }
        }

        /// <summary>
        /// Configure the console settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBtnConfigure_Click(object sender, RoutedEventArgs e)
        {
            LoggingConsoleConfig consoleConfig = new LoggingConsoleConfig();
            consoleConfig.Owner = this;
            consoleConfig.ShowDialog();
            UpdateSettings();
        }

        /// <summary>
        /// Close this window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Update settings that affect the Console.
        /// </summary>
        private void UpdateSettings()
        {
            this._consoleBufferedRows = (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BUFFERED_ROWS];
        }

        /// <summary>
        /// Handle the window closed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            StopMonitoringLoggingOutput();
            this.Owner.Focus();
        }

        /// <summary>
        /// Start logging data to the console.
        /// </summary>
        private void StartMonitoringLoggingOutput()
        {
            try
            {
                if (_log4NetAppender == null)
                {
                    _log4NetAppender = (LogTapAppender)GlobalContext.Properties[Variables.MAP_KEY_LOG4NET_LOG_TAP_INSTANCE];
                }

                _log4NetAppender.MessageLogging += LogIncomingMessage;
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "An error occurred while trying to start logging.", "IOException", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggingConsole.log.Error("Error occrred while attempting to begin logging to the Logging Console.", e);
            }
        }

        /// <summary>
        /// Stop logging data to the console.
        /// </summary>
        private void StopMonitoringLoggingOutput()
        {
            try
            {
                if (_log4NetAppender != null)
                {
                    _log4NetAppender.MessageLogging -= LogIncomingMessage;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "An error occurred while trying to stop logging.", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                LoggingConsole.log.Error("Error occurred while attempting to stop logging to the Logging Console.", e);
            }
        }

        /// <summary>
        /// Event Handler that handles incoming messages from the Log4Net LogTapAppender.
        /// </summary>
        /// <param name="message">Incoming logging message.</param>
        private void LogIncomingMessage(string message)
        {
            // Add the new text
            Dispatcher.Invoke(
                new Action(() =>
                    {
                        try
                        {
                            int bufferedRows = (int)Properties.ApplicationSettings.Default[Variables.SETTINGS_CONSOLE_BUFFERED_ROWS];
                            if ((bufferedRows != -1) && (bufferedRows <= txtLoggingConsole.LineCount))
                            {
                                for (int i = 0; i < ((txtLoggingConsole.LineCount - bufferedRows) + 1); i++)
                                {
                                    int j = 0;
                                    for (j = 0; j < _loggingBuffer.Length && _loggingBuffer[j] != Environment.NewLine[0]; j++) ;

                                    _loggingBuffer.Remove(0, (j + 1));
                                }

                                _loggingBuffer.Append(message);
                                txtLoggingConsole.Text = _loggingBuffer.ToString();
                            }
                            else if (_loggingBuffer.Length > 102400)
                            {
                                while (_loggingBuffer.Length > 102400)
                                {
                                    int j = 0;
                                    for (j = 0; j < _loggingBuffer.Length && _loggingBuffer[j] != Environment.NewLine[0]; j++) ;

                                    _loggingBuffer.Remove(0, (j + 1));
                                }

                                _loggingBuffer.Append(message);
                                txtLoggingConsole.Text = _loggingBuffer.ToString();
                            }
                            else
                            {
                                _loggingBuffer.Append(message);
                                txtLoggingConsole.AppendText(message);
                            }

                            if (!(_scrollLock))
                            {
                                txtLoggingConsole.ScrollToEnd();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.Out.WriteLine(e.Message + e.StackTrace);
                        }
                    }
                )
            );
        }

        /// <summary>
        /// Event handler for the toolbar Scroll Lock button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbBtnScrollLock_Click(object sender, RoutedEventArgs e)
        {
            ToggleScrollLock();
        }

        /// <summary>
        /// Toggle ScrollLock on and off.
        /// </summary>
        private void ToggleScrollLock()
        {
            _scrollLock = !(_scrollLock);
            tbBtnScrollLock.IsChecked = _scrollLock;
        }

        /// <summary>
        /// Check for the ScrollLock key to turn on/off ScrollToEnd() for the console.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Scroll)
            {
                ToggleScrollLock();
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
