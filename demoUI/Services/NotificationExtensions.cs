using Radzen;

namespace demoUI.Services;

public static class NotificationExtensions
{
    public static void Success(this NotificationService notifications, string detail, string summary = "Done") =>
        notifications.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = summary,
            Detail = detail,
            Duration = 3500
        });

    public static void Error(this NotificationService notifications, string detail, string summary = "Error") =>
        notifications.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Error,
            Summary = summary,
            Detail = detail,
            Duration = 7000
        });

    public static void Warning(this NotificationService notifications, string detail, string summary = "Check this") =>
        notifications.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Warning,
            Summary = summary,
            Detail = detail,
            Duration = 5000
        });
}
