using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shkolo_console_app.Data;

// Saves and loads ANY data (Student, Teacher, List<Student>, a whole "School" class...)
// to/from a JSON file. It does not know about specific classes, so when the real
// classes are finished nothing here has to change.
//
// Requirements for the classes that get saved:
//  - public properties ({ get; set; }) or public fields (both are saved)
//  - a public parameterless constructor (the default one is fine)
public class JsonRepository
{
    private readonly string _path;

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,                 // human-readable file
        IncludeFields = true,                 // also save public fields
        PropertyNameCaseInsensitive = true,   // "name" and "Name" both load
        Converters = { new JsonStringEnumConverter() } // enums saved as text, not numbers
    };

    public JsonRepository(string path = "Data/app.settings.json")
    {
        // Relative paths are placed in the project folder (where the .csproj is, and the
        // app.settings.json you edit in Visual Studio live), so the data is saved in that file.
        _path = Path.GetFullPath(path, FindProjectFolder());
    }

    // Walks up from bin/Debug/net10.0 until it finds the .csproj.
    // Falls back to the .exe folder if the source folder isn't there (e.g. published app).
    private static string FindProjectFolder()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (dir.GetFiles("*.csproj").Length > 0)
                return dir.FullName;
            dir = dir.Parent;
        }
        return AppContext.BaseDirectory;
    }

    // Objects -> text -> file
    public void Save<T>(T data)
    {
        string json = JsonSerializer.Serialize(data, _options);

        // Write to a temp file first so a crash can't leave a half-written data file
        string temp = _path + ".tmp";
        File.WriteAllText(temp, json);
        File.Move(temp, _path, overwrite: true);
    }

    // File -> text -> objects. Returns a new empty T if the file is missing or broken.
    public T Load<T>() where T : new()
    {
        // First run: the file doesn't exist yet
        if (!File.Exists(_path))
            return new T();

        try
        {
            string json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<T>(json, _options) ?? new T();
        }
        catch (JsonException)
        {
            // File is empty or corrupted
            return new T();
        }
    }
}
