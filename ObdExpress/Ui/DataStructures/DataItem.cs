using ELM327API.Processing.Interfaces;
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
    public class DataItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor that accepts the Type of the ELM327 handler that updates this Dashboard Item.
        /// </summary>
        /// <param name="handlerType"></param>
        public DataItem(Type handlerType)
        {
            IHandler handler = (IHandler)Activator.CreateInstance(handlerType);
            this._handlerType = handlerType;
            this._measure = handler.Name;
            this._unit = handler.Unit;
        }

        /// <summary>
        /// The Type of the ELM327 handler that should be updating this DashboardItem.
        /// </summary>
        private Type _handlerType;
        public Type HandlerType
        {
            get
            {
                return this._handlerType;
            }
        }

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
                this.NotifyPropertyChanged("Position");
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
