using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ace.Base.MSTest
{
	public class Point
	{
		public int? X { get; set; }
		public int? Y { get; set; }
	}

	public class Person
	{
		public string FirstName;
		public string LastName;
		public int? Age;
		public DateTime Birthday;
		
		public List<Role> Roles = new List<Role>();
	}
	
	[DataContract]
	public class Role
	{
		[DataMember] public string Name;           
		[DataMember] public DateTime LastOnline = DateTime.Now;      
		[DataMember] public Person Person;
		public string CodePhrase;
	}
	
	public interface IModel {}
	public class ModelA : IModel {}
	public class ModelB : IModel {}
	public class ModelC : IModel {}
	
	public class Shape {}
	public class Line : Shape { public double Length; }
	public class Circle : Shape { public double Radius; }
	public class Triangle : Shape {}
	public class Rectangle : Shape { public double Width, Height; }
	
	public static class DiagnosticsGraph
	{
		public static Person Create(DateTime timestamp)
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
				LastOnline = timestamp,
				Person = person0
			};
            
			var roleB0 = new Role
			{
				Name = "Thomas Anderson",
				CodePhrase = "Follow the White Rabbit.",
				LastOnline = timestamp,
				Person = person0
			};
            
			person0.Roles.Add(roleA0);
			person0.Roles.Add(roleB0);
			return person0;
		}
	}
}

