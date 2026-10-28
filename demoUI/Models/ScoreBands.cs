namespace demoUI.Models;

public sealed record ScoreBand(string Label, string Color, bool StrikeThrough);

public static class ScoreBands
{
    public static readonly ScoreBand High = new("70% and above", "var(--rz-success, #2e7d32)", false);
    public static readonly ScoreBand Good = new("40% to 69%", "var(--rz-info, #0277bd)", false);
    public static readonly ScoreBand Low = new("30% to 39%", "var(--rz-warning, #ef6c00)", false);
    public static readonly ScoreBand Fail = new("Below 30%", "var(--rz-danger, #c62828)", true);

    public static IReadOnlyList<ScoreBand> All { get; } = new[] { High, Good, Low, Fail };

    public static ScoreBand? For(decimal? overallPercentage)
    {
        if (overallPercentage is not { } value)
            return null;

        if (value >= 70m) return High;
        if (value >= 40m) return Good;
        if (value >= 30m) return Low;
        return Fail;
    }

    public static string Tint(ScoreBand band) =>
        $"color-mix(in srgb, {band.Color} 22%, transparent)";
}