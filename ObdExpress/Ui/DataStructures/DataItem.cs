using ELM327API.Processing.Interfaces;
using System;
using System.ComponentModel;

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
            _handlerType = handlerType;
            _measure = handler.Name;
            _unit = handler.Unit;
        }

        /// <summary>
        /// The Type of the ELM327 handler that should be updating this DashboardItem.
        /// </summary>
        private Type _handlerType;
        public Type HandlerType
        {
            get
            {
                return _handlerType;
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
                return _position;
            }
            set
            {
                _position = value;
                NotifyPropertyChanged("Position");
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
                return _measure;
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
                return _unit;
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
                return _value;
            }
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
