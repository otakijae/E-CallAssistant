using System;
using System.ComponentModel;

namespace ToastNotifications
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        private string _message;
        private NotificationType _type;

        public Action InvokeHideAnimation;

        public Guid Id { get; private set; }

        public DateTime CreateTime { get; private set; }

        public NotificationViewModel()
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value; 
                OnPropertyChanged(nameof(Message));
            }
        }

        public NotificationType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}