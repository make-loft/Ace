using System;
using System.Collections.Generic;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable once InconsistentNaming
	/* LanguageExtensions */
	public static partial class LE
	{
		// Since C# 7.3
		// public static bool Is<T>(this ref T o) where T: struct => true;
		
		public static bool Is(this bool o) => true;
		public static bool Is(this char o) => true;
		
		public static bool Is(this sbyte o) => true;
		public static bool Is(this short o) => true;
		public static bool Is(this int o) => true;
		public static bool Is(this long o) => true;
		
		public static bool Is(this byte o) => true;
		public static bool Is(this ushort o) => true;
		public static bool Is(this uint o) => true;
		public static bool Is(this ulong o) => true;		
		
		public static bool Is(this float o) => true;
		public static bool Is(this double o) => true;
		public static bool Is(this decimal o) => true;
		
		public static bool Is(this bool o, out bool x) => (x = o).Is();
		public static bool Is(this char o, out char x) => (x = o).Is();
		
		public static bool Is(this sbyte o, out sbyte x) => (x = o).Is();
		public static bool Is(this short o, out short x) => (x = o).Is();
		public static bool Is(this int o, out int x) => (x = o).Is();
		public static bool Is(this long o, out long x) => (x = o).Is();
		
		public static bool Is(this byte o, out byte x) => (x = o).Is();
		public static bool Is(this ushort o, out ushort x) => (x = o).Is();
		public static bool Is(this uint o, out uint x) => (x = o).Is();
		public static bool Is(this ulong o, out ulong x) => (x = o).Is();	
		
		public static bool Is(this float o, out float x) => (x = o).Is();
		public static bool Is(this double o, out double x) => (x = o).Is();
		public static bool Is(this decimal o, out decimal x) => (x = o).Is();

		
		// Since C# 7.3
		// public static bool IsNull<T>(this ref T o) where T: struct => false;
		
		public static bool IsNull(this bool o) => false;
		public static bool IsNull(this char o) => false;
		
		public static bool IsNull(this sbyte o) => false;
		public static bool IsNull(this short o) => false;
		public static bool IsNull(this int o) => false;
		public static bool IsNull(this long o) => false;
		
		public static bool IsNull(this byte o) => false;
		public static bool IsNull(this ushort o) => false;
		public static bool IsNull(this uint o) => false;
		public static bool IsNull(this ulong o) => false;		
		
		public static bool IsNull(this float o) => false;
		public static bool IsNull(this double o) => false;
		public static bool IsNull(this decimal o) => false;
		
		public static bool IsNull(this bool o, out bool x) => (x = o).IsNull();
		public static bool IsNull(this char o, out char x) => (x = o).IsNull();
		
		public static bool IsNull(this sbyte o, out sbyte x) => (x = o).IsNull();
		public static bool IsNull(this short o, out short x) => (x = o).IsNull();
		public static bool IsNull(this int o, out int x) => (x = o).IsNull();
		public static bool IsNull(this long o, out long x) => (x = o).IsNull();
		
		public static bool IsNull(this byte o, out byte x) => (x = o).IsNull();
		public static bool IsNull(this ushort o, out ushort x) => (x = o).IsNull();
		public static bool IsNull(this uint o, out uint x) => (x = o).IsNull();
		public static bool IsNull(this ulong o, out ulong x) => (x = o).IsNull();	
		
		public static bool IsNull(this float o, out float x) => (x = o).IsNull();
		public static bool IsNull(this double o, out double x) => (x = o).IsNull();
		public static bool IsNull(this decimal o, out decimal x) => (x = o).IsNull();
	}
}