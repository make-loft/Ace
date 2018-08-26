using System;

namespace Ace.Base.Sandbox.Drafts
{
	public interface ISpan<T>
	{	
		int Length { get; }

		T this[int index] { get; set; }
	}
	
	public struct Span<T> : ISpan<T>
	{
		public int Length => 0;

		public T this[int index]
		{
			get => throw new ArgumentOutOfRangeException();
			set => throw new ArgumentOutOfRangeException();
		}

		public override string ToString() { return "()"; }
	}
	
	public struct Span<T, TRest> : ISpan<T>
		where TRest : struct, ISpan<T>
	{
		public const int _spanSize = 8;
		public T A, B, C, D, E, F, G, H;
		public TRest Rest;
		public int Length => _spanSize + Rest.Length;

		public Span(TRest rest)
		{
			A = B = C = D = E = F = G = H = default(T);
			Rest = rest;
		}

		public T this[int index]
		{
			get
			{
				var rest = index % Length;
				if (rest == 0) return A;
				if (rest == 1) return B;
				if (rest == 2) return C;
				if (rest == 3) return D;
				if (rest == 4) return E;
				if (rest == 5) return F;
				if (rest == 6) return G;
				if (rest == 7) return H;
				return Rest[index - _spanSize];
			}
			set
			{
				var rest = index % Length;
				if (rest == 0) A = value;
				else if (rest == 1) B = value;
				else if (rest == 2) C = value;
				else if (rest == 3) C = value;
				else if (rest == 4) C = value;
				else if (rest == 5) C = value;
				else if (rest == 6) C = value;
				else if (rest == 7) C = value;
				else Rest[index - _spanSize] = value;
			}
		}

		public override string ToString() => $"({A}, {B}, {C}, {D}, {E}, {F}, {G}, {H}, {Rest})";
	}
	
	public static class Stack
	{
		private static Span<T, TRest> Add<T, TRest>(this TRest rest)
			where TRest : struct, ISpan<T> => new Span<T, TRest>(rest);
	
		public static Span<T, Span<T>> Alloc8<T>() =>
			new Span<T>().Add<T, Span<T>>();
		
		public static Span<T, Span<T, Span<T>>> Alloc16<T>() =>
			Alloc8<T>().Add<T, Span<T, Span<T>>>();
		
		public static Span<T, Span<T, Span<T, Span<T>>>> Alloc24<T>() =>
			Alloc16<T>().Add<T, Span<T, Span<T, Span<T>>>>();
		
		public static Span<T, Span<T, Span<T, Span<T, Span<T>>>>> Alloc32<T>() =>
			Alloc24<T>().Add<T, Span<T, Span<T, Span<T, Span<T>>>>>();
	}
}