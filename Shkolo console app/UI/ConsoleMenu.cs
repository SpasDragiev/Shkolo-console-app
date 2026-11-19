using Shkolo_console_app.Data;
using Shkolo_console_app.Models;

namespace Shkolo_console_app.UI;

// The console user interface: shows menus, reads input, calls School methods.
// Every change is saved to the JSON file right away.
public class ConsoleMenu
{
    private readonly School _school;
    private readonly SchoolStorage _storage;

    public ConsoleMenu(School school, SchoolStorage storage)
    {
        _school = school;
        _storage = storage;
    }

    public void Run()
    {
        while (true)
        {
            ClearScreen();
            Console.WriteLine("=== SHKOLO ===");
            Console.WriteLine("1. Students");
            Console.WriteLine("2. Teachers");
            Console.WriteLine("3. Subjects");
            Console.WriteLine("0. Exit");

            switch (ReadText("Choose"))
            {
                case "1": StudentsMenu(); break;
                case "2": TeachersMenu(); break;
                case "3": SubjectsMenu(); break;
                case "0": return;
                default: Pause("Unknown option."); break;
            }
        }
    }

    // ---------- Students ----------

    private void StudentsMenu()
    {
        while (true)
        {
            ClearScreen();
            Console.WriteLine("=== STUDENTS ===");
            Console.WriteLine("1. List students");
            Console.WriteLine("2. Add student");
            Console.WriteLine("3. Remove student");
            Console.WriteLine("4. Add grade to student");
            Console.WriteLine("5. Show student's grades");
            Console.WriteLine("0. Back");

            switch (ReadText("Choose"))
            {
                case "1": ListStudents(); break;
                case "2": AddStudent(); break;
                case "3": RemoveStudent(); break;
                case "4": AddGrade(); break;
                case "5": ShowGrades(); break;
                case "0": return;
                default: Pause("Unknown option."); break;
            }
        }
    }

    private void ListStudents()
    {
        if (_school.Students.Count == 0) { Pause("No students yet."); return; }
        foreach (Student s in _school.Students) Console.WriteLine(s);
        Pause();
    }

    private void AddStudent()
    {
        int id = NextId(_school.Students.Select(s => s.Id));
        string first = ReadText("First name");
        string last = ReadText("Last name");
        string className = ReadText("Class (e.g. 10A)");
        if (first == "" || last == "") { Pause("Names cannot be empty."); return; }

        _school.AddStudent(new Student(id, first, last, className));
        _storage.Save(_school);
        Pause($"Student added with id {id}.");
    }

    private void RemoveStudent()
    {
        int id = ReadInt("Student id");
        if (_school.FindStudent(id) == null) { Pause("No student with that id."); return; }
        _school.RemoveStudent(id);
        _storage.Save(_school);
        Pause("Student removed.");
    }

    private void AddGrade()
    {
        Student? student = _school.FindStudent(ReadInt("Student id"));
        if (student == null) { Pause("No student with that id."); return; }

        Subject? subject = PickSubject();
        if (subject == null) return;

        double value = ReadDouble("Grade (2-6)");
        if (value < 2 || value > 6) { Pause("Grade must be between 2 and 6."); return; }

        int id = NextId(_school.Students.SelectMany(s => s.Grades).Select(g => g.Id));
        student.AddGrade(new Grade(id, value, subject));
        _storage.Save(_school);
        Pause("Grade added.");
    }

    private void ShowGrades()
    {
        Student? student = _school.FindStudent(ReadInt("Student id"));
        if (student == null) { Pause("No student with that id."); return; }

        Console.WriteLine(student);
        foreach (Grade g in student.Grades) Console.WriteLine("  " + g);
        Pause();
    }

    // ---------- Teachers ----------

    private void TeachersMenu()
    {
        while (true)
        {
            ClearScreen();
            Console.WriteLine("=== TEACHERS ===");
            Console.WriteLine("1. List teachers");
            Console.WriteLine("2. Add teacher");
            Console.WriteLine("0. Back");

            switch (ReadText("Choose"))
            {
                case "1":
                    if (_school.Teachers.Count == 0) { Pause("No teachers yet."); break; }
                    foreach (Teacher t in _school.Teachers) Console.WriteLine(t);
                    Pause();
                    break;
                case "2": AddTeacher(); break;
                case "0": return;
                default: Pause("Unknown option."); break;
            }
        }
    }

    private void AddTeacher()
    {
        Subject? subject = PickSubject();
        if (subject == null) return;

        int id = NextId(_school.Teachers.Select(t => t.Id));
        string first = ReadText("First name");
        string last = ReadText("Last name");
        if (first == "" || last == "") { Pause("Names cannot be empty."); return; }

        _school.AddTeacher(new Teacher(id, first, last, subject));
        _storage.Save(_school);
        Pause($"Teacher added with id {id}.");
    }

    // ---------- Subjects ----------

    private void SubjectsMenu()
    {
        while (true)
        {
            ClearScreen();
            Console.WriteLine("=== SUBJECTS ===");
            Console.WriteLine("1. List subjects");
            Console.WriteLine("2. Add subject");
            Console.WriteLine("0. Back");

            switch (ReadText("Choose"))
            {
                case "1":
                    if (_school.Subjects.Count == 0) { Pause("No subjects yet."); break; }
                    foreach (Subject s in _school.Subjects) Console.WriteLine(s);
                    Pause();
                    break;
                case "2":
                    string name = ReadText("Subject name");
                    if (name == "") { Pause("Name cannot be empty."); break; }
                    int id = NextId(_school.Subjects.Select(s => s.Id));
                    _school.AddSubject(new Subject(id, name));
                    _storage.Save(_school);
                    Pause($"Subject added with id {id}.");
                    break;
                case "0": return;
                default: Pause("Unknown option."); break;
            }
        }
    }

    private Subject? PickSubject()
    {
        if (_school.Subjects.Count == 0) { Pause("Add a subject first."); return null; }
        foreach (Subject s in _school.Subjects) Console.WriteLine(s);

        Subject? subject = _school.Subjects.FirstOrDefault(s => s.Id == ReadInt("Subject id"));
        if (subject == null) Pause("No subject with that id.");
        return subject;
    }

    // ---------- Input helpers ----------

    private static void ClearScreen()
    {
        try { Console.Clear(); } catch (IOException) { } // no real console (redirected input)
    }

    private static int NextId(IEnumerable<int> existing) => existing.DefaultIfEmpty(0).Max() + 1;

    private static string ReadText(string label)
    {
        Console.Write($"{label}: ");
        return (Console.ReadLine() ?? "").Trim();
    }

    private static int ReadInt(string label)
    {
        while (true)
        {
            if (int.TryParse(ReadText(label), out int n)) return n;
            Console.WriteLine("Please enter a whole number.");
        }
    }

    private static double ReadDouble(string label)
    {
        while (true)
        {
            string text = ReadText(label).Replace(',', '.');
            if (double.TryParse(text, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double n)) return n;
            Console.WriteLine("Please enter a number.");
        }
    }

    private static void Pause(string? message = null)
    {
        if (message != null) Console.WriteLine(message);
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}
