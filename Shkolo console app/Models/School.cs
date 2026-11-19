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

        public void RemoveStudent(int id)
        {
            Student? student = FindStudent(id);

            if (student != null)
            {
                students.Remove(student);
            }
        }

        public Student? FindStudent(int id)
        {
            foreach (Student student in students)
            {
                if (student.Id == id)
                {
                    return student;
                }
            }

            return null;
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

