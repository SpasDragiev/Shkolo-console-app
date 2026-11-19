using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shkolo
{
    public class Subject
    {
        // Fields
        private int id;
        private string name;

        // Properties
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        // Constructor
        public Subject(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        // Methods
        public override string ToString()
        {
            return $"{id} - {name}";
        }
    }
}

