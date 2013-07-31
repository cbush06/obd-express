using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ObdExpress.Ui.UserControls
{
    /// <summary>
    /// Interaction logic for RegisteredPanel.xaml
    /// </summary>
    public partial class RegisteredPanel : UserControl, INotifyPropertyChanged
    {
        #region Dependency Properties

        /// <summary>
        /// Sets the brush of the border used by this panel. Defaults to black.
        /// </summary>
        public new Brush BorderBrush
        {
            get 
            {
                return (Brush)GetValue(BorderBrushProperty);
            }
            set
            {
                SetValue(BorderBrushProperty, value);
                this.NotifyPropertyChanged("BorderBrush");
            }
        }
        public new static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(RegisteredPanel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Sets the thickness of the border used by this panel. Defaults to 1px.
        /// </summary>
        public new Thickness BorderThickness
        {
            get
            {
                return (Thickness)GetValue(BorderThicknessProperty); 
            }
            set
            {
                SetValue(BorderThicknessProperty, value);
                this.NotifyPropertyChanged("BorderThickness");
            }
        }
        public new static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(RegisteredPanel), new PropertyMetadata(new Thickness(1.0)));

        /// <summary>
        /// Sets the brush used to paint the background of this panel.
        /// </summary>
        public new Brush Background
        {
            get 
            { 
                return (Brush)GetValue(BackgroundProperty); 
            }
            set 
            { 
                SetValue(BackgroundProperty, value);
                this.NotifyPropertyChanged("Background");
            }
        }
        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(RegisteredPanel), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Sets the thickness of the border around the header of this panel. Defaults to 0px for left, top, and right, and to 1px for bottom.
        /// </summary>
        public Thickness HeaderBorderThickness
        {
            get
            {
                return (Thickness)GetValue(HeaderBorderThicknessProperty);
            }
            set
            {
                SetValue(HeaderBorderThicknessProperty, value);
                this.NotifyPropertyChanged("HeaderBorderThickness");
            }
        }
        public static readonly DependencyProperty HeaderBorderThicknessProperty =
            DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(RegisteredPanel), new PropertyMetadata(new Thickness(0, 0, 0, 1)));

        /// <summary>
        /// Sets the brush used to create the border around the header.
        /// </summary>
        public Brush HeaderBorderBrush
        {
            get
            {
                return (Brush)GetValue(HeaderBorderBrushProperty);
            }
            set
            {
                SetValue(HeaderBorderBrushProperty, value);
                this.NotifyPropertyChanged("HeaderBorderBrush");
            }
        }
        public static readonly DependencyProperty HeaderBorderBrushProperty =
            DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(RegisteredPanel), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Sets the brush used to paint the background of the header. 
        /// </summary>
        public Brush HeaderBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(HeaderBackgroundBrushProperty);
            }
            set
            {
                SetValue(HeaderBackgroundBrushProperty, value);
                this.NotifyPropertyChanged("HeaderBackgroundBrush");
            }
        }
        public static readonly DependencyProperty HeaderBackgroundBrushProperty =
            DependencyProperty.Register("HeaderBackgroundBrush", typeof(Brush), typeof(RegisteredPanel), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Sets the content of the panel's header.
        /// </summary>
        public object Header
        {
            get
            {
                return (object)GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
                this.NotifyPropertyChanged("Header");
            }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(RegisteredPanel), new PropertyMetadata(null));


        /// <summary>
        /// Sets the thickness of the border around the body of this panel. Defaults to 0px for all sides.
        /// </summary>
        public Thickness BodyBorderThickness
        {
            get
            {
                return (Thickness)GetValue(BodyBorderThicknessProperty);
            }
            set
            {
                SetValue(BodyBorderThicknessProperty, value);
                this.NotifyPropertyChanged("BodyBorderThickness");
            }
        }
        public static readonly DependencyProperty BodyBorderThicknessProperty =
            DependencyProperty.Register("BodyBorderThickness", typeof(Thickness), typeof(RegisteredPanel), new PropertyMetadata(new Thickness(0.0)));

        /// <summary>
        /// Sets the brush used to create the border around the body. Defaults to transparent.
        /// </summary>
        public Brush BodyBorderBrush
        {
            get
            {
                return (Brush)GetValue(BodyBorderBrushProperty);
            }
            set
            {
                SetValue(BodyBorderBrushProperty, value);
                this.NotifyPropertyChanged("BodyBorderBrush");
            }
        }
        public static readonly DependencyProperty BodyBorderBrushProperty =
            DependencyProperty.Register("BodyBorderBrush", typeof(Brush), typeof(RegisteredPanel), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Sets the brush used to pain the background of the body.
        /// </summary>
        public Brush BodyBackgroundBrush
        {
            get
            {
                return (Brush)GetValue(BodyBackgroundBrushProperty);
            }
            set
            {
                SetValue(BodyBackgroundBrushProperty, value);
                this.NotifyPropertyChanged("BodyBackgroundBrush");
            }
        }
        public static readonly DependencyProperty BodyBackgroundBrushProperty =
            DependencyProperty.Register("BodyBackgroundBrush", typeof(Brush), typeof(RegisteredPanel), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Sets the content of the panel's body.
        /// </summary>
        public object Body
        {
            get 
            {
                return (object)GetValue(BodyProperty);
            }
            set
            {
                SetValue(BodyProperty, value);
                this.NotifyPropertyChanged("Body");
            }
        }
        public static readonly DependencyProperty BodyProperty =
            DependencyProperty.Register("Body", typeof(object), typeof(RegisteredPanel), new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RegisteredPanel()
        {
            InitializeComponent();
            Console.Out.WriteLine("Registered Panel.");
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
