using Shkolo_console_app.Models;

namespace Shkolo_console_app.Data;

// Loads / saves the whole School through JsonRepository.
// In the JSON file every Grade and Teacher contains a copy of its Subject. After loading,
// those copies are replaced with the real objects from School.Subjects, so a Subject is
// one shared object again (renaming a subject then changes it everywhere).
public class SchoolStorage
{
    private readonly JsonRepository _repo;

    public SchoolStorage(JsonRepository repo)
    {
        _repo = repo;
    }

    public School Load()
    {
        School school = _repo.Load<School>();
        Relink(school);
        return school;
    }

    public void Save(School school)
    {
        _repo.Save(school);
    }

    private static void Relink(School school)
    {
        var subjects = school.Subjects.ToDictionary(s => s.Id);

        foreach (Teacher teacher in school.Teachers)
        {
            if (teacher.Subject != null && subjects.TryGetValue(teacher.Subject.Id, out var s))
                teacher.Subject = s;
        }

        foreach (Student student in school.Students)
            foreach (Grade grade in student.Grades)
            {
                if (grade.Subject != null && subjects.TryGetValue(grade.Subject.Id, out var s))
                    grade.Subject = s;
            }
    }
}
