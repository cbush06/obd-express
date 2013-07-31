using ObdExpress.Ui.DataStructures;
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

namespace ObdExpress.Ui.UserControls.HomePanels
{
    /// <summary>
    /// Interaction logic for DashboardPanelProperties.xaml
    /// </summary>
    public partial class DashboardPanelProperties : Window, INotifyPropertyChanged
    {
        private DashboardPanel _parent = null;

        /// <summary>
        /// Provides access to the DashboardPanel's columns setting.
        /// </summary>
        public int Columns
        {
            get
            {
                return this._parent.Columns;
            }
            set
            {
                this._parent.Columns = value;
                this.OnPropertyChanged("Columns");
            }
        }

        /// <summary>
        /// Datastructure backing the DataGrid.
        /// </summary>
        public ObservableCollection<DashboardItem> DashboardItems
        {
            get
            {
                return this._parent.DashboardItems;
            }
        }

        /// <summary>
        /// Overloaded constructor that accepts a reference to a collection of dashboad items. Presumably, this is the collection used by the Dashboard Panel.
        /// </summary>
        /// <param name="dashboardItems">Collection of dashboard items to edit.</param>
        public DashboardPanelProperties(DashboardPanel parent)
        {
            this._parent = parent;
            InitializeComponent();
            this.OnPropertyChanged("DashboardItems");
            this.OnPropertyChanged("Columns");
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DashboardPanelProperties()
        {
            InitializeComponent();
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if(this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
