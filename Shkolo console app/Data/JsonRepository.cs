using System.Text.Json;
using Shkolo_console_app.Models;

namespace Shkolo_console_app.Data;

// Saves and loads the app's data to/from a JSON file (our small "database").
public class JsonRepository
{
    private readonly string _path;

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true // makes the file human-readable
    };

    public JsonRepository(string path = "data.json")
    {
        _path = path;
    }

    // Objects -> text -> file
    public void Save(List<Student> students)
    {
        string json = JsonSerializer.Serialize(students, _options);
        File.WriteAllText(_path, json);
    }

    // File -> text -> objects
    public List<Student> Load()
    {
        // First run: the file doesn't exist yet
        if (!File.Exists(_path))
            return new List<Student>();

        try
        {
            string json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
        }
        catch (JsonException)
        {
            // File is empty or corrupted
            return new List<Student>();
        }
    }
}
