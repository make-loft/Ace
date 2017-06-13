using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Art.Replication;

namespace Art
{
    public class Juxtaposition
    {    
        public object Etalon { get; set; }
        public object Sample { get; set; }
        public string Path { get; set; }
        public Etalon.State State { get; set; }
        
        public override string ToString() =>
            "<" + State + "> • [" + Path + "] " + Print(Etalon) + " " + Print(Sample);

        private static string Print(object item) => item is Map || item is Set
            ? "<instance>"
            : item == null
                ? "<null>"
                : "{" + item + "}";
    }
    
    public static class Etalon
    {
        public enum State
        {
            Identical,
            Different,
            Appended,
            Missed
        }
   
        public static IEnumerable<Juxtaposition> Juxtapose(this Snapshot etalon, Snapshot sample, 
            string path = "this") => etalon.MasterState.Juxtapose(sample.MasterState, path);

        private static IEnumerable<Juxtaposition> Juxtapose(this object etalon, object sample, string path = "this")
        {
            switch (sample)
            {
                case Map map:
                    foreach (var result in Juxtapose(etalon, map, path))
                    {
                        yield return result;
                    }
                    yield break;
                case Set set:
                    foreach (var result in Juxtapose(etalon, set, path))
                    {
                        yield return result;
                    }
                    yield break;
                default:
                    yield return new Juxtaposition
                    {
                        Path = path,
                        State = Equals(etalon, sample) ? State.Identical : State.Different,
                        Etalon = etalon,
                        Sample = sample
                    };
                    yield break;
            }
        }

        private static IEnumerable<Juxtaposition> Juxtapose(this object etalon, ICollection items, string path)
        {
            if (etalon is Map mA && items is Map mB)
            {
                var keys = mA.Keys.Union(mB.Keys);
                foreach (var key in keys)
                {
                    var hasA = mA.TryGetValue(key, out var a);
                    var hasB = mB.TryGetValue(key, out var b);
                    if (!hasA || !hasB)
                    {
                        yield return new Juxtaposition
                        {
                            Etalon = etalon,
                            Sample = items,
                            Path = path + "." + key,
                            State = hasA ? State.Missed : State.Appended
                        };
                    }
                    else
                    {
                        foreach (var result in Juxtapose(a, b, path + "." + key))
                        {
                            yield return result;
                        }
                    }
                }
            }
            else if (etalon is Set sA && items is Set sB)
            {
                for (var index = 0; index < sB.Count; index++)
                {
                    var a = sA[index];
                    var b = sB[index];
                    foreach (var result in Juxtapose(a, b, path + "[" + index + "]"))
                    {
                        yield return result;
                    }
                }
            }
            else
            {
                yield return new Juxtaposition
                {
                    Path = path,
                    State = State.Different,
                    Etalon = etalon,
                    Sample = items
                };
            }
        }
    }
}
