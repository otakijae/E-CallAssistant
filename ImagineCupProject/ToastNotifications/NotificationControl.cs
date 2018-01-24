using System.Windows;
using System.Windows.Controls;

namespace ToastNotifications
{
    public class NotificationControl : Control
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(FrameworkElement), typeof(NotificationControl), new PropertyMetadata(default(FrameworkElement)));
        public static readonly DependencyProperty NotificationProperty = DependencyProperty.Register("Notification", typeof(NotificationViewModel), typeof(NotificationControl), new PropertyMetadata(default(NotificationViewModel), NotificationChanged));

        public static readonly RoutedEvent NotificationClosingEvent = EventManager.RegisterRoutedEvent("NotificationClosing", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(NotificationControl));
        public static readonly RoutedEvent NotificationClosedEvent = EventManager.RegisterRoutedEvent("NotificationClosed", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(NotificationControl));

        private bool _isClosing;
        private bool _isClosed;

        public Canvas Icon
        {
            get { return (Canvas)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public NotificationViewModel Notification
        {
            get { return (NotificationViewModel) GetValue(NotificationProperty); }
            set { SetValue(NotificationProperty, value); }
        }

        public event RoutedEventHandler NotificationClosed
        {
            add { AddHandler(NotificationClosedEvent, value); }
            remove { RemoveHandler(NotificationClosedEvent, value); }
        }

        public event RoutedEventHandler NotificationClosing
        {
            add { AddHandler(NotificationClosingEvent, value); }
            remove { RemoveHandler(NotificationClosingEvent, value); }
        }

        static NotificationControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationControl), new FrameworkPropertyMetadata(typeof(NotificationControl)));
        }

        public NotificationControl()
        {
            _isClosed = false;
            _isClosing = false;

            Unloaded += OnUnloaded;
        }


        public void InvokeHideAnimation()
        {
            if (_isClosing || _isClosed)
                return;

            _isClosing = true;

            RaiseEvent(new RoutedEventArgs(NotificationClosingEvent));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButton = GetTemplateChild("CloseButton") as Button;
            if (closeButton != null)
                closeButton.Click += CloseButtonClicked;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var closeButton = GetTemplateChild("CloseButton") as Button;

            if (closeButton != null)
                closeButton.Click -= CloseButtonClicked;
        }

        private static void NotificationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = dependencyObject as NotificationControl;
            var notification = eventArgs.NewValue as NotificationViewModel;

            if (control == null || notification == null)
                return;

            notification.InvokeHideAnimation = control.InvokeHideAnimation;
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_isClosing || _isClosed)
                return;

            _isClosed = true;

            RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));
        }
    }
}
