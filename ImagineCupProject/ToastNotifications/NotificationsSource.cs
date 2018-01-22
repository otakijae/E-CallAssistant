using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace ToastNotifications
{
    public class NotificationsSource : INotifyPropertyChanged
    {
        public const int UnlimitedNotifications = -1;

        public static readonly TimeSpan NeverEndingNotification = TimeSpan.MaxValue;

        private readonly DispatcherTimer _timer;

        private bool _isOpen;

        private bool _isTopmost;

        public NotificationsSource()
            :this(Dispatcher.CurrentDispatcher)
        {

        }

        public NotificationsSource(Dispatcher dispatcher)
        {
            NotificationMessages = new ObservableCollection<NotificationViewModel>();

            MaximumNotificationCount = 2;
            NotificationLifeTime = TimeSpan.FromSeconds(6);

            _timer = new DispatcherTimer(DispatcherPriority.Normal, dispatcher);
            _timer.Interval = TimeSpan.FromMilliseconds(200);
        }

        public ObservableCollection<NotificationViewModel> NotificationMessages { get; private set; }

        public long MaximumNotificationCount { get; set; }

        public TimeSpan NotificationLifeTime { get; set; }

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        public bool IsTopmost
        {
            get { return _isTopmost; }
            set
            {
                _isTopmost = value;
                OnPropertyChanged(nameof(IsTopmost));
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (NotificationLifeTime == NeverEndingNotification)
                return;

            var currentTime = DateTime.Now;
            var itemsToRemove = NotificationMessages.Where(x => (currentTime - x.CreateTime) >= NotificationLifeTime)
                                                    .Select(x => x.Id)
                                                    .ToList();

            foreach (var id in itemsToRemove)
            {
                Hide(id);
            }
        }

        public void Show(string message, NotificationType type)
        {
            if (NotificationMessages.Any() == false)
            {
                InternalStartTimer();
                IsOpen = true;
            }

            if (MaximumNotificationCount != UnlimitedNotifications)
            {
                if (NotificationMessages.Count >= MaximumNotificationCount)
                {
                    int removeCount = (int)(NotificationMessages.Count - MaximumNotificationCount) + 1;

                    var itemsToRemove = NotificationMessages.OrderBy(x => x.CreateTime)
                                                            .Take(removeCount)
                                                            .Select(x => x.Id)
                                                            .ToList();
                    foreach (var id in itemsToRemove)
                        Hide(id);
                }
            }

            NotificationMessages.Add(new NotificationViewModel
                                     {
                                         Message = message,
                                         Type = type
                                     });
        }

        public void Hide(Guid id)
        {
            var n = NotificationMessages.SingleOrDefault(x => x.Id == id);
            if (n?.InvokeHideAnimation == null)
                return;

            n.InvokeHideAnimation();

            var timer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromMilliseconds(200),
                            Tag = n
                        };
            timer.Tick += RemoveNotificationsTimer_OnTick;
            timer.Start();
        }

        private void RemoveNotificationsTimer_OnTick(object sender, EventArgs eventArgs)
        {
            var timer = sender as DispatcherTimer;
            if (timer == null) return;

            // Stop the timer and cleanup for GC
            timer.Tick += RemoveNotificationsTimer_OnTick;
            timer.Stop();

            var n = timer.Tag as NotificationViewModel;
            if (n == null) return;

            NotificationMessages.Remove(n);

            if (NotificationMessages.Any()) return;

            InternalStopTimer();
            IsOpen = false;
        }

        private void InternalStartTimer()
        {
            _timer.Tick += TimerOnTick;
            _timer.Start();
        }

        private void InternalStopTimer()
        {
            _timer.Stop();
            _timer.Tick -= TimerOnTick;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}