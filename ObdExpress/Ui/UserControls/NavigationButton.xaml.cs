using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ObdExpress.Ui.UserControls
{
    /// <summary>
    /// Interaction logic for NavigationButton.xaml
    /// </summary>
    public partial class NavigationButton : UserControl, INotifyPropertyChanged
    {
        public enum ButtonType { MENU, MENU_ITEM };

        private static LinearGradientBrush _navigationButtonPressedGold = new LinearGradientBrush();
        private static LinearGradientBrush _navigationButtonHighlightedGold = new LinearGradientBrush();
        private static LinearGradientBrush _navigationButtonSelectedGold = new LinearGradientBrush();
        private static LinearGradientBrush _navigationButtonTransparent = new LinearGradientBrush();

        private static LinearGradientBrush _navigationButtonPressedBlack = new LinearGradientBrush();
        private static LinearGradientBrush _navigationButtonSelectedBlack = new LinearGradientBrush();
        private static LinearGradientBrush _navigationButtonTransparentBlack = new LinearGradientBrush();

        private static SolidColorBrush _transparentSolidColorBrush = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
        private static SolidColorBrush _blackSolidColorBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00));
        private static SolidColorBrush _whiteSolidColorBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF));

        private Border _buttonBorder = null;

        /// <summary>
        /// The IsSelected Property is used to indicate if this navgiation button has been selected in the Navigation menu.
        /// </summary>
        public bool IsSelected 
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(NavigationButton), new PropertyMetadata(false, IsSelectedCallback));

        private static void IsSelectedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigationButton)d).NavigationGrid_MouseLeave(d, null);
        }

        /// <summary>
        /// Provides the label used on the button
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(NavigationButton), new PropertyMetadata(""));

        /// <summary>
        /// Sets the path to the Image used on this button
        /// </summary>
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(NavigationButton), new PropertyMetadata());

        /// <summary>
        /// Sets the Width of the Image Shown. Defaults to 32.
        /// </summary>
        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(NavigationButton), new PropertyMetadata(32));


        /// <summary>
        /// Sets the Height of the Image Shown. Defaults to 32.
        /// </summary>
        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(NavigationButton), new PropertyMetadata(32));

        /// <summary>
        /// Sets the Margin of the Image Shown. Defaults with only 5px above the image.
        /// </summary>
        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }

        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(NavigationButton), new PropertyMetadata(new Thickness(0.0, 5.0, 0.0, 0.0)));

        
        /// <summary>
        /// Defines the type of button this will be in a menu
        /// </summary>
        public ButtonType NavButtonType
        {
            get { return (ButtonType)GetValue(TypeProperty); }
            set
            {
                SetValue(TypeProperty, value);
                NavigationGrid_MouseLeave(this, null);
            }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(ButtonType), typeof(NavigationButton), new PropertyMetadata(ButtonType.MENU_ITEM));

        /// <summary>
        /// Sets the background applied to the Grid of the button
        /// </summary>
        private LinearGradientBrush _buttonBackground = NavigationButton._navigationButtonTransparent;
        public LinearGradientBrush ButtonBackground
        {
            get 
            { 
                return _buttonBackground; 
            }
        }

        /// <summary>
        /// Sets the foreground applied to the label of the button.
        /// </summary>
        private SolidColorBrush _buttonForeground = NavigationButton._blackSolidColorBrush;
        public SolidColorBrush ButtonForeground
        {
            get
            {
                return _buttonForeground;
            }
        }

        /// <summary>
        /// Provide an event for listeners to subscribe to so we can notify of the Click event.
        /// </summary>
        public static readonly DependencyProperty ClickProperty =
            DependencyProperty.Register("Click", typeof(MouseButtonEventHandler), typeof(NavigationButton), new PropertyMetadata());

        public MouseButtonEventHandler Click
        {
            get
            {
                return (MouseButtonEventHandler)GetValue(ClickProperty);
            }
            set
            {
                SetValue(ClickProperty, value);
            }
        }

        /// <summary>
        /// Create the gradient backgrounds used for all Navigation Buttons
        /// </summary>
        static NavigationButton()
        {
            _navigationButtonPressedGold.StartPoint = new Point(0.5, 0.0);
            _navigationButtonPressedGold.EndPoint = new Point(0.5, 1.0);
            _navigationButtonPressedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xC5, 0xBC, 0x70), 0.0));
            _navigationButtonPressedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xDF, 0xD5, 0x7E), 0.5));
            _navigationButtonPressedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xF2, 0xE7, 0x89), 1.0));

            _navigationButtonSelectedGold.StartPoint = new Point(0.5, 0.0);
            _navigationButtonSelectedGold.EndPoint = new Point(0.5, 1.0);
            _navigationButtonSelectedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xF3, 0x90), 0.0));
            _navigationButtonSelectedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xE8, 0xDD, 0x83), 0.5));
            _navigationButtonSelectedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0xC5, 0xBC, 0x70), 1.0));

            _navigationButtonHighlightedGold.StartPoint = new Point(0.5, 0.0);
            _navigationButtonHighlightedGold.EndPoint = new Point(0.5, 1.0);
            _navigationButtonHighlightedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0x7F, 0xFF, 0xF3, 0x90), 0.0));
            _navigationButtonHighlightedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0x7F, 0xE8, 0xDD, 0x83), 0.5));
            _navigationButtonHighlightedGold.GradientStops.Add(new GradientStop(Color.FromArgb(0x7F, 0xC5, 0xBC, 0x70), 1.0));

            _navigationButtonTransparent.StartPoint = new Point(0.5, 0.0);
            _navigationButtonTransparent.EndPoint = new Point(0.5, 1.0);
            _navigationButtonTransparent.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, 0x00, 0x00, 0x00), 0.0));
            _navigationButtonTransparent.GradientStops.Add(new GradientStop(Color.FromArgb(0x00, 0x00, 0x00, 0x00), 1.0));

            _navigationButtonPressedBlack.StartPoint = new Point(0.5, 0.0);
            _navigationButtonPressedBlack.EndPoint = new Point(0.5, 1.0);
            _navigationButtonPressedBlack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x4E, 0x4E, 0x4E), 0.0));
            _navigationButtonPressedBlack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x6E, 0x6E, 0x6E), 0.5));
            _navigationButtonPressedBlack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x99, 0x99, 0x99), 1.0));

            _navigationButtonSelectedBlack.StartPoint = new Point(0.5, 0.0);
            _navigationButtonSelectedBlack.EndPoint = new Point(0.5, 1.0);
            _navigationButtonSelectedBlack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x9D, 0x9D, 0x9D), 0.0));
            _navigationButtonSelectedBlack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x58, 0x58, 0x58), 0.5));
            _navigationButtonSelectedBlack.GradientStops.Add(new GradientStop(Color.FromArgb(0xFF, 0x3C, 0x3C, 0x3C), 1.0));
        }

        /// <summary>
        /// Default constructor that creates a Menu Item button.
        /// </summary>
        public NavigationButton() : this(ButtonType.MENU_ITEM) {  }

        /// <summary>
        /// Prepare the Navigation Button and attach the Event Listeners
        /// </summary>
        /// <param name="buttonType">Type of button to create. May either be a menu button (that selects a menu of items to show), or a menu item.</param>
        public NavigationButton( ButtonType buttonType )
        {
            // Store a reference to the template to provide access to its children
            ControlTemplate template;

            // Initialize the components
            InitializeComponent();

            // Must call ApplyTemplate() before we can FindName()
            ApplyTemplate();

            // Get a reference to the border so we can show and hide it
            template = Template;
            _buttonBorder = (Border) template.FindName("ButtonBorder", this);

            // Store the type of button this is so we can apply the correct color scheme and other properties
            NavButtonType = buttonType;
        }

        #region Event Listeners for hover actions and clicking
        private void NavigationGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Set the background to pressed
                _buttonBackground = NavigationButton._navigationButtonPressedGold;

                // Update the display
                NotifyPropertyChanged("ButtonBackground");
            }
        }

        private void NavigationGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                // Set the background to selected/highlighted
                _buttonBackground = NavigationButton._navigationButtonSelectedGold;

                // Update the display
                NotifyPropertyChanged("ButtonBackground");

                // Pass on the event
                if (Click != null)
                {
                    Click(sender, e);
                }
            }
        }

        private void NavigationGrid_MouseEnter(object sender, MouseEventArgs e)
        {

            // If it's a top-level menu button (selects a particular menu to show)
            if (NavButtonType == ButtonType.MENU)
            {
                _buttonBackground = NavigationButton._navigationButtonHighlightedGold;
                _buttonForeground = NavigationButton._blackSolidColorBrush;
            }
            // If it's just a menu-item
            else
            {
                _buttonBackground = NavigationButton._navigationButtonHighlightedGold;
                _buttonBorder.BorderBrush = NavigationButton._blackSolidColorBrush;
            }

            NotifyPropertyChanged("ButtonBackground");
            NotifyPropertyChanged("ButtonForeground");
        }

        private void NavigationGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsSelected)
            {
                // If it's a top-level menu button (selects a particular menu to show)
                if (NavButtonType == ButtonType.MENU)
                {
                    _buttonBackground = NavigationButton._navigationButtonSelectedGold;
                    _buttonForeground = NavigationButton._blackSolidColorBrush;
                }
                // If it's just a menu-item
                else
                {
                    _buttonBackground = NavigationButton._navigationButtonSelectedGold;
                    _buttonBorder.BorderBrush = NavigationButton._blackSolidColorBrush;
                }

                NotifyPropertyChanged("ButtonBackground");
                NotifyPropertyChanged("ButtonForeground");
            }
            else
            {
                // If it's a top-level menu button (selects a particular menu to show)
                if (NavButtonType == ButtonType.MENU)
                {
                    _buttonBackground = NavigationButton._navigationButtonSelectedBlack;
                    _buttonForeground = NavigationButton._whiteSolidColorBrush;
                }
                // If it's just a menu-item
                else
                {
                    _buttonBackground = NavigationButton._navigationButtonTransparent;
                    _buttonBorder.BorderBrush = NavigationButton._transparentSolidColorBrush;
                }


                NotifyPropertyChanged("ButtonBackground");
                NotifyPropertyChanged("ButtonForeground");
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
