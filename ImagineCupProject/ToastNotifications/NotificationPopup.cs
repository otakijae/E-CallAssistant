using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ToastNotifications
{
    [DefaultProperty("Child")]
    [ContentProperty("Child")]
    public class NotificationPopup : Control
    {
        private NotificationPopupWindow _window;

        static NotificationPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationPopup), new FrameworkPropertyMetadata(typeof(NotificationPopup)));
        }

        public NotificationPopup()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public void UpdateBounds()
        {
            _window?.UpdateBounds();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _window = new NotificationPopupWindow(this);
            _window.Topmost = IsTopmost;
            _window.PopupFlowDirection = PopupFlowDirection;
            _window.PopupContent = Child;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _window?.Close();
            _window = null;
        }

        public static readonly DependencyProperty ChildProperty = DependencyProperty.Register(nameof(Child), typeof(FrameworkElement), typeof(NotificationPopup), new FrameworkPropertyMetadata(default(FrameworkElement), ChildChanged));

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(NotificationPopup), new FrameworkPropertyMetadata(default(bool), IsOpenChanged));

        public static readonly DependencyProperty IsTopmostProperty = DependencyProperty.Register(nameof(IsTopmost), typeof(bool), typeof(NotificationPopup), new FrameworkPropertyMetadata(default(bool), IsTopmostChanged));

        public static readonly DependencyProperty PopupFlowDirectionProperty = DependencyProperty.Register(nameof(PopupFlowDirection), typeof(PopupFlowDirection), typeof(NotificationPopup), new FrameworkPropertyMetadata(default(PopupFlowDirection), PopupFlowDirectionChanged));

        public PopupFlowDirection PopupFlowDirection
        {
            get { return (PopupFlowDirection) GetValue(PopupFlowDirectionProperty); }
            set { SetValue(PopupFlowDirectionProperty, value); }
        }

        public FrameworkElement Child
        {
            get { return (FrameworkElement) GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool) GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private static void IsOpenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var popup = dependencyObject as NotificationPopup;
            if (popup == null)
                return;

            if (eventArgs.NewValue == eventArgs.OldValue)
                return;

            bool isOpen = (bool) eventArgs.NewValue;

            if (popup._window == null)
                return;

            if (isOpen)
                popup._window.Show();
            else
                popup._window.Hide();
        }

        public bool IsTopmost
        {
            get { return (bool)GetValue(IsTopmostProperty); }
            set { SetValue(IsTopmostProperty, value); }
        }

        private static void IsTopmostChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var popup = dependencyObject as NotificationPopup;
            if (popup == null)
                return;
            
            if (eventArgs.NewValue == eventArgs.OldValue)
                return;

            if (popup._window == null)
                return;

            popup._window.Topmost = (bool)eventArgs.NewValue;
        }

        private static void ChildChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var popup = dependencyObject as NotificationPopup;
            if (popup == null)
                return;

            if (eventArgs.NewValue == eventArgs.OldValue)
                return;

            FrameworkElement child = eventArgs.NewValue as FrameworkElement;

            if (child == null)
                return;

            if (popup._window == null)
                return;

            popup._window.PopupContent = child;
        }

        private static void PopupFlowDirectionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var popup = dependencyObject as NotificationPopup;
            if (popup == null)
                return;

            if (eventArgs.NewValue == eventArgs.OldValue)
                return;

            var flowDirection = (PopupFlowDirection)eventArgs.NewValue;

            if (popup._window == null)
                return;

            popup._window.PopupFlowDirection = flowDirection;
        }
    }
}
