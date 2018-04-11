using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.Sugar
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
            if (getPerson().Is(out var p) && p.Check
                (
                    p.FirstName.Is("Abc"),
                    p.LastName.Is(out var lastName),
                    p.Age > 9
                ).All(true))
            {
                Assert.AreEqual("Xyz", lastName);

                if (
                    p.Check(
                        p.FirstName.To(out var name).Is("a"),
                        p.Age.To(out var age) > 5
                    ).Any(true))
                {
                    Assert.IsTrue(name.IsNot("a"));
                    Assert.IsTrue(age > 5);
                }
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
                case Person p when p.Check(
                    p.FirstName.Is("Abc"),
                    p.LastName.Is("Xyz"),
                    p.Age > 5)
                    .All(true):
                        
                    Assert.IsTrue(p.FirstName == "Abc" || p.LastName == "Xyz" || p.Age > 5);
                    break;
                    
                case Point p when p.Check(
                    p.X > 9 && p.X < 16,
                    p.Y < 28)
                    .All(true):
                    
                    Assert.IsTrue(p.X > 9 && p.X < 16 && p.Y < 28);
                    break;
                
                default:
                    throw new Exception();
            }
        }
    }
}