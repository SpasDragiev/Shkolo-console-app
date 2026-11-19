using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shkolo
{
    public class Grade
    {
        // Fields
        private int id;
        private double value;
        private Subject subject;

        // Properties
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public double Value
        {
            get { return value; }
            set { value = value; }
        }

        public Subject Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        // Constructor
        public Grade(int id, double value, Subject subject)
        {
            this.id = id;
            this.value = value;
            this.subject = subject;
        }

        // Methods
        public override string ToString()
        {
            return $"{subject.Name}: {value}";
        }
    }
}

