using ELM327API.Processing.DataStructures;
using System.ComponentModel;

namespace ObdExpress.Ui.DataStructures
{
    public class PanelPropertyOption : INotifyPropertyChanged
    {
        public HandlerWrapper HandlerWrapper 
        {
            get { return _handlerWrapper; }
            set
            {
                _handlerWrapper = value;
                OnPropertyChanged("HandlerWrapper");
            }
        }
        private HandlerWrapper _handlerWrapper = null;

        public bool IsChecked 
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
        private bool _isChecked = false;

        public int Position 
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }
        private int _position = 0;

        public PanelPropertyOption(HandlerWrapper wrapper, int position)
        {
            _handlerWrapper = wrapper;
            _isChecked = false;
            _position = position;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
