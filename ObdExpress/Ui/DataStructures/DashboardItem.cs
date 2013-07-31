using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObdExpress.Ui.DataStructures
{
    /// <summary>
    /// This is a data structure used to back the items shown in the dashboard of the home screen. It should, most likely, be used in
    /// an ObservableCollection to populate the DashboardPanel of the home screen.
    /// </summary>
    public class DashboardItem : INotifyPropertyChanged
    {
        /// <summary>
        /// The position of this item.
        /// </summary>
        private int _position;
        public int Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
            }
        }

        /// <summary>
        /// The name of the measure this records.
        /// </summary>
        private string _measure;
        public string Measure
        {
            get
            {
                return this._measure;
            }
            set
            {
                this._measure = value;
            }
        }

        /// <summary>
        /// The unit of measure this item is classified by.
        /// </summary>
        private string _unit;
        public string Unit
        {
            get
            {
                return this._unit;
            }
            set
            {
                this._unit = value;
                this.NotifyPropertyChanged("Unit");
            }
        }

        /// <summary>
        /// The value this item currently holds.
        /// </summary>
        private string _value;
        public string Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                this.NotifyPropertyChanged("Value");
            }
        }

        /// <summary>
        /// Determines if this item is displayed on the dashboard.
        /// </summary>
        private bool _isShown = true;
        public bool IsShown
        {
            get
            {
                return this._isShown;
            }
            set
            {
                this._isShown = value;
                this.NotifyPropertyChanged("IsShown");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if(this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
