using Shkolo_console_app.Models;
using Spectre.Console;

namespace Shkolo_console_app.UI;

// Small helpers shared by the Razor pages.
public static class Helpers
{
    public static int NextId(IEnumerable<int> existing) => existing.DefaultIfEmpty(0).Max() + 1;

    // The Select component works with text, so entities are shown as "<id> - <description>".
    // The id is always the first word, so it can be read back from the chosen text.
    public static int ParseId(string? label)
    {
        if (string.IsNullOrWhiteSpace(label)) return -1;
        return int.TryParse(label.Split(' ')[0], out int id) ? id : -1;
    }

    public static string Label(Student s) => $"{s.Id} - {s.FirstName} {s.LastName} ({s.ClassName})";
    public static string Label(Subject s) => $"{s.Id} - {s.Name}";

    public static bool TryParseGrade(string? text, out double value)
    {
        string fixedText = (text ?? "").Trim().Replace(',', '.');
        return double.TryParse(fixedText, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out value);
    }

    // Bulgarian scale: 2-3 bad, 4 ok, 5-6 good
    public static Color GradeColor(double grade)
    {
        if (grade <= 0) return Color.Grey;
        if (grade < 3.5) return Color.Red;
        if (grade < 4.5) return Color.Yellow;
        if (grade < 5.5) return Color.Green;
        return Color.Lime;
    }
}
