using System.Collections;
using System.Collections.Generic;
using Art.Replication;

namespace Art
{
    public enum CompareState
    {
        Equals, Different, SourceMissed, TargetMissed
    }

    public class CompareResult
    {
        public object Source { get; set; }
        public object Target { get; set; }
        public string Path { get; set; }

        public CompareState State { get; set; }

        public override string ToString()
        {
            return State + " # " + Path + " # " + Source + " <> " + Target;
        }
    }

    public static class Comparizer
    {
        public static IEnumerable<CompareResult> GetResults(this object o, object value, string path)
        {
            switch (value)
            {
                case Map map:
                    foreach (var result in GetResults(o, map, path))
                    {
                        yield return result;
                    }
                    yield break;
                case Set set:
                    foreach (var result in GetResults(o, set, path))
                    {
                        yield return result;
                    }
                    yield break;
                default:
                    yield return new CompareResult
                        {
                            Path = path,
                            State = Equals(o, value) ? CompareState.Equals : CompareState.Different,
                            Source = o,
                            Target = value
                        };
                    yield break;
            }
        }

        public static IEnumerable<CompareResult> GetResults(this object o, ICollection items, string path)
        {
            if (o is Map mA && items is Map mB)
            {
                foreach (var pair in mA)
                {
                    var a = pair.Value;
                    var b = mB[pair.Key];
                    foreach (var result in GetResults(a, b, path + "." + pair.Key))
                    {
                        yield return result;
                    }
                }
            }
            else yield return new CompareResult
            {
                Path = path,
                State = CompareState.Different,
                Source = o,
                Target = items
            };
        }
    }
}
