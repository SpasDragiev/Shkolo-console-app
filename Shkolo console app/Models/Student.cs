using System;
using System.Collections.Generic;
using System.Linq;

namespace shkolo
{
    public class Student
    {
        // Fields
        private int id;
        private string firstName;
        private string lastName;
        private string className;
        private List<Grade> grades;

        // Properties
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string ClassName
        {
            get { return className; }
            set { className = value; }
        }

        public List<Grade> Grades
        {
            get { return grades; }
            set { grades = value; }
        }

        // Constructor
        public Student(int id, string firstName, string lastName, string className)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.className = className;
            grades = new List<Grade>();
        }

        // Methods
        public void AddGrade(Grade grade)
        {
            grades.Add(grade);
        }

        public double GetAverageGrade()
        {
            if (grades.Count == 0)
            {
                return 0;
            }

            return grades.Average(g => g.Value);
        }

        public List<Grade> GetGradesBySubject(Subject subject)
        {
            return grades.Where(g => g.Subject.Id == subject.Id).ToList();
        }

        public override string ToString()
        {
            return $"{id} - {firstName} {lastName} - {className} - Average: {GetAverageGrade():F2}";
        }
    }
}

