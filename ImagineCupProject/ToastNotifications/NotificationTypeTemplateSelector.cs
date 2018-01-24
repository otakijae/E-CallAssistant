using System.Windows;
using System.Windows.Controls;

namespace ToastNotifications
{
    class NotificationTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InformationTemplate { get; set; }
        public DataTemplate SuccessTemplate { get; set; }
        public DataTemplate WarningTemplate { get; set; }
        public DataTemplate ErrorTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var n = item as NotificationViewModel;
            if (n == null)
                return null;

            switch (n.Type)
            {
                case NotificationType.Information:
                    return InformationTemplate;
                case NotificationType.Success:
                    return SuccessTemplate;
                case NotificationType.Warning:
                    return WarningTemplate;
                case NotificationType.Error:
                    return ErrorTemplate;
                default:
                    return null;
            }
        }
    }
}