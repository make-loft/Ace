using System;
using Ace.Base.Sandbox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.MSTest.Sugar
{
    public static class WithLikeExtensions
    {
        public static Person GetPerson() => new Person().To(out var p).With
        (
            p.FirstName = "Abc",
            p.LastName = "Xyz",
            p.Age = 18
        );
        
        public static Point GetPoint() => new Point().To(out var p).With
        (
            p.X = 12,
            p.Y = 21
        );

        public static void Test()
        {
            Test(GetPerson);
            Test(() => null);
            
            Test(GetPerson());
            Test(GetPoint());
        }

        public static void Test(Func<Person> getPerson)
        {
            if (getPerson().Is(out var p) && p.All
                (
                    p.FirstName.Is("Abc"),
                    p.LastName.Is(out var lastName),
                    p.Age > 9
                ))
            {
                Assert.AreEqual("Xyz", lastName);
            }
            else
            {
                Assert.IsNull(p);
            }
        }

        public static void Test(object o)
        {
            switch (o)
            {
                case Person p when p.Any(
                    p.FirstName.Is("Aaaa"),
                    p.LastName.Is("Bbbb"),
                    p.Age > 5):
                        
                    Assert.IsTrue(p.FirstName == "Aaaa" || p.LastName == "Bbbb" || p.Age > 5);
                    break;
                    
                case Point p when p.All(
                    p.X > 9 && p.X < 16,
                    p.Y < 28):
                    
                    Assert.IsTrue(p.X > 9 && p.X < 16 && p.Y < 28);
                    break;
                
                default:
                    throw new Exception();
            }
        }
    }
}