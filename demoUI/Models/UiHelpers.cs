using Radzen;

namespace demoUI.Models;

public static class UiHelpers
{
    public static BadgeStyle StatusBadge(EnrollmentStatus status) => status switch
    {
        EnrollmentStatus.Active => BadgeStyle.Success,
        EnrollmentStatus.Completed => BadgeStyle.Primary,
        _ => BadgeStyle.Secondary
    };

    public static BadgeStyle GradeBadge(string grade) => grade switch
    {
        "A+" or "A" => BadgeStyle.Success,
        "B" or "C" => BadgeStyle.Info,
        "D" => BadgeStyle.Warning,
        "F" => BadgeStyle.Danger,
        _ => BadgeStyle.Secondary
    };
}