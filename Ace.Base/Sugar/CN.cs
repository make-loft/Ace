﻿using System;
// ReSharper disable once CheckNamespace
namespace Ace
{
    // ReSharper disable InconsistentNaming
    /* Constants */
    public static class CN
    {
        public const object Null = null;
        public static readonly Func<bool, bool> Not = LE.Not;
        public static readonly Func<bool, bool> IsTrue = LE.IsTrue;
        public static readonly Func<bool, bool> IsFalse = LE.IsFalse;
        public static readonly Func<object, bool> IsNull = LE.IsNull;
        public static readonly Func<object, bool> Is = LE.Is;
        public static readonly Action NotImplementedException = () => throw new NotImplementedException();
        public static readonly Action Stub = () => { };
    }
    
    public static class CN<T>
    {
        public static readonly Func<T, bool> IsNull = LE.IsNull;
        public static readonly Func<T, bool> Is = LE.Is;
    }
}