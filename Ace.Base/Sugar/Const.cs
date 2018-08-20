using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static class Const
	{
		public const object Null = null;
		public static readonly Func<bool, bool> Not = LE.Not;
		public static readonly Func<bool, bool> IsTrue = LE.IsTrue;
		public static readonly Func<bool, bool> IsFalse = LE.IsFalse;
		public static readonly Func<object, bool> IsNot = LE.IsNot;
		public static readonly Func<object, bool> Is = LE.Is;
		public static readonly Action NotImplementedException = () => throw new NotImplementedException();
		public static readonly Action Stub = () => { };
	}
	
	public static class Const<T>
	{
		public static readonly Func<T, bool> IsNot = LE.IsNot;
		public static readonly Func<T, bool> Is = LE.Is;
	}

	public static class Pair<TKey, TValue>
	{
		public static readonly Func<KeyValuePair<TKey, TValue>, TKey> Key = p => p.Key;
		public static readonly Func<KeyValuePair<TKey, TValue>, TValue> Value = p => p.Value;
	}
}