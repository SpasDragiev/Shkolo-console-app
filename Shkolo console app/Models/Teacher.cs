using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shkolo
{
    public class Teacher
    {
        // Fields
        private int id;
        private string firstName;
        private string lastName;
        private Subject subject;

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

        public Subject Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        // Constructor
        public Teacher(int id, string firstName, string lastName, Subject subject)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.subject = subject;
        }

        // Methods
        public override string ToString()
        {
            return $"{id} - {firstName} {lastName} - {subject.Name}";
        }
    }
}

