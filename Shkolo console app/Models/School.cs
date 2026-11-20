using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shkolo_console_app.Models
{
    public class School
    {
        // Fields
        private List<Student> students = new List<Student>();
        private List<Teacher> teachers = new List<Teacher>();
        private List<Subject> subjects = new List<Subject>();

        // Properties
        public List<Student> Students
        {
            get { return students; }
            set { students = value; }
        }

        public List<Teacher> Teachers
        {
            get { return teachers; }
            set { teachers = value; }
        }

        public List<Subject> Subjects
        {
            get { return subjects; }
            set { subjects = value; }
        }

        // Constructor
        public School()
        {
            students = new List<Student>();
            teachers = new List<Teacher>();
            subjects = new List<Subject>();
        }

        // Methods
        public void AddStudent(Student student)
        {
            students.Add(student);
        }

        // A student's Id is his number inside the class (1, 2, 3...), so it is only
        // unique together with the class name (10A no. 1 and 10B no. 1 both exist).
        public void RemoveStudent(string className, int number)
        {
            Student? student = FindStudent(className, number);

            if (student != null)
            {
                students.Remove(student);
            }
        }

        public Student? FindStudent(string className, int number)
        {
            foreach (Student student in students)
            {
                if (student.Id == number &&
                    string.Equals(student.ClassName, className, StringComparison.OrdinalIgnoreCase))
                {
                    return student;
                }
            }

            return null;
        }

        // Next free number in the given class (starts from 1 for every class).
        public int NextStudentNumber(string className)
        {
            return students
                .Where(s => string.Equals(s.ClassName, className, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Id)
                .DefaultIfEmpty(0)
                .Max() + 1;
        }

        public void AddTeacher(Teacher teacher)
        {
            teachers.Add(teacher);
        }

        public void AddSubject(Subject subject)
        {
            subjects.Add(subject);
        }
    }
}

