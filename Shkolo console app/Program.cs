using Shkolo_console_app.Data;
using Shkolo_console_app.Models;

// TEMPORARY test of the JSON storage. Venko0 will replace this with the real menu.
var repo = new JsonRepository();

var students = repo.Load();
Console.WriteLine($"Loaded {students.Count} student(s) from file.");

if (students.Count == 0)
{
    students.Add(new Student { Id = 1, Name = "Ivan", Grades = new List<int> { 5, 6, 4 } });
    students.Add(new Student { Id = 2, Name = "Maria", Grades = new List<int> { 6, 6 } });
    repo.Save(students);
    Console.WriteLine("No data found, saved 2 students. Run the program again to see them loaded.");
}
else
{
    foreach (var s in students)
        Console.WriteLine($"{s.Id}: {s.Name} - grades: {string.Join(", ", s.Grades)}");
}
