using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ace.Base.Sandbox.Sugar
{
    public static class CombinedMatching
    {
        public static void Test()
        {
            TestLuck();
            TestSuperMatching();
            TestTupleCheck();
        }

        private static void TestLuck()
        {
            Assert.AreEqual(LuckGauge(7, 7, 7), "Bingo! You are really lucky!");
            Assert.AreEqual(LuckGauge(3, 3, 3), "Not bad! You are lucky!");
            Assert.AreEqual(LuckGauge(7, 7, 9), "Oh! So close...");
            Assert.AreEqual(LuckGauge(7, 9, 9), "Double hit!");
            Assert.AreEqual(LuckGauge(8, 8, 9), "Double hit!");
            Assert.AreEqual(LuckGauge(5, 8, 5), "Double hit!");
            Assert.AreEqual(LuckGauge(1, 5, 4), "We love you!");
        }

        private static string LuckGauge(int a, int b, int c) =>
            Tuple.Create(a, b, c).Is(out var tp) &&

            tp.Check(7, 7, 7).All(true) ? "Bingo! You are really lucky!" :
            tp.Check(a, a, a).All(true) ? "Not bad! You are lucky!" :

            tp.Check(7, 7, 7).Count(true) == 2 ? "Oh! So close..." :

            tp.Check(a, a, c).All(true) ? "Double hit!" :
            tp.Check(c, b, c).All(true) ? "Double hit!" :
            tp.Check(a, b, b).All(true) ? "Double hit!" :

            tp.Check(a, b, c).All(true) ? "We love you!" :

            "unreachable";

        private static void TestSuperMatching()
        {
            Assert.AreEqual(SuperMatching(null), "OMG! This is null!");
            Assert.AreEqual(SuperMatching(32), "This is a int with value '32'");
            Assert.AreEqual(SuperMatching("abc"), "This is a string with value 'abc'");
            Assert.AreEqual(SuperMatching(new Line()), "This is a line");
            Assert.AreEqual(SuperMatching(new Rectangle {Width = 2, Height = 3}), "This is a rectangle with square 6");
        }

        private static string SuperMatching(this object item) =>
            item.Match(

                (Person p) =>
                    p.ToSwitch(
                        p.FirstName.To(out var name),
                        p.Age.To(out var age)
                    ).Is(out var sw) &&

                    sw.Case("Jack", 32) ? "Piter is 32" :
                    sw.Case("Alice", 16) ? "M is 32" :
                    sw.Case("Piter", age) ? "P age" :
                    sw.Case(name, 12) ? "young" :
                    sw.Case(name, age) ? "any" :

                    "unreachable",

                (Shape shape) =>
                    shape.Match(
                        (Line _) => "This is a line",
                        (Circle c) => $"This is a circle with radius {c.Radius} and square {c.CalculateSquare()}",
                        (Rectangle r) => $"This is a rectangle with square {r.CalculateSquare()}"
                    ),

                (string s) => $"This is a string with value '{s}'",

                (int i) => $"This is a int with value '{i}'",

                (object o) => o?.ToString(),

                () => "OMG! This is null!"
            );
        
        private static void TestTupleCheck()
        {
            Assert.AreEqual(TupleCheck(() => new Person {FirstName = "Keanu", LastName = "Reeves"}), "It is Neo!");
            Assert.AreEqual(TupleCheck(() => new Person {Age = 21}), "It is a person with requried age");
        }

        public static string TupleCheck(Func<Person> getPerson) =>
            getPerson().To(out var p).ToTuple
            (
                p.FirstName.To(out var firstName),
                p.LastName.To(out var lastName),
                p.Age.To(out var age)
            ).Is(out var tp) &&

            tp.Check("Keanu", "Reeves", age).All(true) ? "It is Neo!" :
            tp.Check(firstName, lastName, null).All(true) ? "It is a person with undefined age" :

            tp.Check(firstName, lastName, 21).All(true) ? "It is a person with requried age" :
            tp.Check(firstName, lastName, age).All(true) && age < 21 ? "It is so young person" :
            tp.Check(firstName, lastName, age).All(true) && age > 21 ? "It is so old person" :

            tp.Check(firstName, lastName, age).All(true) ? $"It is {firstName}" :

            "unreachable";
    }
}