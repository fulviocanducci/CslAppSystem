using System;

namespace CslAppSystem.Models
{
    public class People
    {
        public People() { }
        public People(string name, DateTime dateBirthday)
        {
            Name = name;
            DateBirthday = dateBirthday;
        }

        public People(int id, string name, DateTime dateBirthday)
        {
            Id = id;
            Name = name;
            DateBirthday = dateBirthday;
        }

        public long Id { get; set; }
        public string Name { get; set; } = null;
        public DateTime DateBirthday { get; set; }

        public static implicit operator bool(People p)
        {
            return p != null;
        }
    }
}
