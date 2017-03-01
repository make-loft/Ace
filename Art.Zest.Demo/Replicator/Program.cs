using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Art.Replication;
using Art.Wiz;

namespace ConsoleApplication1
{
    public class CustomList : List<object>
    {
        public string Name { get; set; }
    }

    [DataContract]
    public class Person
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public int Age { get; set; }

        //[DataMember]
        public DateTime Timestamp { get; set; }

        //[DataMember]
        public Address Address { get; set; }

        //[DataMember]
        public Person Subperson { get; set; }

        public object[] ObjArray { get; set; }

        public List<object> ObjList { get; set; }

        public CustomList Custom { get; set; }
    }

    [DataContract]
    public class Address
    {
        [DataMember]
        public string Street { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var personMaster = new Person
            {
                FirstName = "Vladimir",
                LastName = "Makarevich",
                Timestamp = DateTime.Now,
                Age = 27,
                Address = new Address
                {
                    Street = "Hello"
                },
                ObjArray = new object[] {"Hi", 1, 3},
                ObjList = new List<object> {"Ow", 4, 7, null},
                Custom = new CustomList {Name = "ABC"}
            };
            personMaster.Subperson = personMaster;
            personMaster.Custom.AddRange(new List<object> {"3", 5, "77"});

            var contractProfile = new Replicator();
            var personSnapshot = contractProfile.TranscribeSnapshotFrom(personMaster);

            personMaster.FirstName = "123";
            personMaster.LastName = "321";
            personMaster.ObjList[0] = null;
            var personReplica = contractProfile.TranslateReplicaFrom(personSnapshot);
            personReplica = personReplica;

            var profile = KeepProfile.GetFormatted();
            var t = new System.Text.StringBuilder().Append(personSnapshot, profile).ToString();
            int i = 0;
            var x = t.Capture(profile, ref i);

            x = x;

        }
    }
}
