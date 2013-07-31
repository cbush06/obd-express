using System;
using System.Collections.Generic;
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
    /// Interaction logic for SideNavigationBar.xaml
    /// </summary>
    public partial class SideNavigationBar : UserControl
    {
        /// <summary>
        /// Sets or returns the text displayed as the header for this panel.
        /// </summary>
        public string SectionHeader
        {
            get
            {
                return (string)GetValue(SectionHeaderProperty);
            }
            set
            {
                SetValue(SectionHeaderProperty, value);
            }
        }
        /// <summary>
        /// Using a DependencyProperty as the backing store for SectionHeader.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SectionHeaderProperty =
            DependencyProperty.Register("SectionHeader", typeof(string), typeof(SideNavigationBar), new PropertyMetadata(""));

        /// <summary>
        /// The ScrollViewer used to display the selected TopMenu.
        /// </summary>
        private ItemsControl _navigationViewer = null;
        public ItemsControl NavigationViewer
        {
            get
            {
                return this._navigationViewer;
            }
        }

        /// <summary>
        /// The StackPanel used to display the buttons that select which TopMenu is shown.
        /// </summary>
        private ItemsControl _topMenuViewer = null;
        public ItemsControl TopMenuViewer
        {
            get
            {
                return this._topMenuViewer;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SideNavigationBar()
        {
            // Initialize the components
            InitializeComponent();

            // Obtain references to the two main components in this UserControl
            this._navigationViewer = navigationViewer;
            this._topMenuViewer = menuSelectViewer;
        }
    }
}
