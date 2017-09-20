using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AceReplication.Diagnostics
{
    [DataContract]
    public class Role
    {
        [DataMember] public string Name;           
        public string CodePhrase;
        [DataMember] public DateTime LastOnline = DateTime.Now;
            
        [DataMember] public Person Person;
    }

    public class Person : ICloneable
    {
        public string FirstName;
        public string LastName;
        public DateTime Birthday;

        public List<Role> Roles = new List<Role>();

        public object Clone()
        {
            var clone = (Person) MemberwiseClone();
            clone.Roles = new List<Role>();
            return clone;
        }
    }

    public static class DiagnosticsGraph
    {
        public static Person Create()
        {
            var person0 = new Person
            {
                FirstName = "Keanu",
                LastName = "Reeves",
                Birthday = new DateTime(1964, 9 ,2)
            };
                   
            var roleA0 = new Role
            {
                Name = "Neo",
                CodePhrase = "The Matrix has you...",
                LastOnline = DateTime.Now,
                Person = person0
            };
            
            var roleB0 = new Role
            {
                Name = "Thomas Anderson",
                CodePhrase = "Follow the White Rabbit.",
                LastOnline = DateTime.Now,
                Person = person0
            };
            
            person0.Roles.Add(roleA0);
            person0.Roles.Add(roleB0);
            return person0;
        }
    }
}