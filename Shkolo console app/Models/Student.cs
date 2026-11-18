namespace Shkolo_console_app.Models;

// TEMPORARY class so the JSON storage can be tested.
// Borislava will replace this with the real Student class.
// IMPORTANT: use properties ({ get; set; }), not plain fields, or JSON will not save them.
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<int> Grades { get; set; } = new();
}
