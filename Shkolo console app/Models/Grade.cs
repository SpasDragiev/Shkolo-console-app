using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shkolo_console_app.Models
{
    public class Grade
    {
        // Fields
        private int id;
        private double value;
        private Subject subject = null!;

        // Properties
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public Subject Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        // Empty constructor (needed so JSON can load the data)
        public Grade() { }

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

