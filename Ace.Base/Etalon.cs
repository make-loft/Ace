using System.Collections.Generic;
using System.Linq;
using Ace.Replication;

namespace Ace
{
	public class Juxtaposition
	{
		public string Path { get; set; }
		public object Etalon { get; set; }
		public object Sample { get; set; }
		public Etalon.State State { get; set; }

		public override string ToString() => $"<{State}> • [{Path}] {Print(Etalon)} {Print(Sample)}";

		private static string Print(object item) =>
			item is Map || item is Set ? "<instance>"
			: item is null ? "<null>" : "{" + item + "}";
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

		public static IEnumerable<Juxtaposition> JuxtaposeWithEtalon(this Snapshot sample,
			Snapshot etalon, string path = "this", bool reordering = false) =>
			sample.MasterState.Juxtapose(etalon.MasterState, path, reordering);

		public static IEnumerable<Juxtaposition> JuxtaposeLikeEtalon(this Snapshot etalon,
			Snapshot sample, string path = "this", bool reordering = false) =>
			sample.MasterState.Juxtapose(etalon.MasterState, path, reordering);

		private static IEnumerable<Juxtaposition> Juxtapose(this object sample,
			object etalon, string path, bool reordering) => sample.Match(

			(Map samples) => etalon.Is(out Map etalons)
				? Juxtapose(samples, etalons, path, reordering)
				: Juxtapose(sample, etalon, path, State.Different),

			(Set samples) => etalon.Is(out Set etalons)
				? Juxtapose(samples, etalons, path, reordering)
				: Juxtapose(sample, etalon, path, State.Different),

			(object _) => Juxtapose(sample, etalon, path),
			() => Juxtapose(null, etalon, path)
		);

		private static IEnumerable<Juxtaposition> Juxtapose(object sample, object etalon, string path,
			State state = State.Identical) => new Juxtaposition
		{
			Path = path,
			Etalon = etalon,
			Sample = sample,
			State = state != State.Identical || Equals(etalon, sample) ? state : State.Different
		}.ToEnumerable();

		private static IEnumerable<Juxtaposition> Juxtapose(Map samples, Map etalons, string path, bool reordering)
		{
			foreach (var key in samples.Keys.Union(etalons.Keys))
			{
				var hasSample = samples.TryGetValue(key, out var sample);
				var hasEtalon = etalons.TryGetValue(key, out var etalon);
				var juxtapositions = hasSample && hasEtalon
					? Juxtapose(sample, etalon, $"{path}.{key}", reordering)
					: Juxtapose(samples, etalons, $"{path}.{key}", hasSample ? State.Appended : State.Missed);

				foreach (var juxtaposition in juxtapositions)
				{
					yield return juxtaposition;
				}
			}
		}

		private static IEnumerable<Juxtaposition> Juxtapose(Set samples, Set etalons, string path, bool reordering)
		{
			samples = reordering ? new Set(samples.OrderBy(i => i)) : samples;
			etalons = reordering ? new Set(etalons.OrderBy(i => i)) : etalons;

			for (var index = 0; index < etalons.Count; index++)
			{
				var sample = samples[index];
				var etalon = etalons[index];
				var juxtapositions = Juxtapose(sample, etalon, $"{path}[{index}]", reordering);

				foreach (var juxtaposition in juxtapositions)
				{
					yield return juxtaposition;
				}
			}
		}
	}
}
