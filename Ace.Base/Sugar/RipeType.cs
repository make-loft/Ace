using System;
using System.Collections.Generic;
using System.Reflection;
using Ace.Sugar;

// ReSharper disable once CheckNamespace
namespace Ace
{
	public class RipeType
	{
		private static readonly Dictionary<Type, RipeType> RawToRipe = new Dictionary<Type, RipeType>();

		public static RipeType Get(Type raw) => Lock.Invoke(RawToRipe, _ =>
			RawToRipe.TryGetValue(raw, out var ripe)
				? ripe
				: RawToRipe[raw] = new RipeType(raw));
		
		private RipeType(Type raw)
		{
			Raw = raw;
			
			Assembly = raw.Assembly;
			AssemblyQualifiedName = raw.AssemblyQualifiedName;
			FullName = raw.FullName;
			Name = raw.Name;
			Namespace = raw.Namespace;

			HasElementType = raw.HasElementType;
			IsArray = raw.IsArray;
			IsAbstract = raw.IsAbstract;
			IsByRef = raw.IsByRef;
			IsClass = raw.IsClass;
			IsEnum = raw.IsEnum;
			IsGenericParameter = raw.IsGenericParameter;
			IsGenericType = raw.IsGenericType;
			IsGenericTypeDefinition = raw.IsGenericTypeDefinition;
			IsInterface = raw.IsInterface;
			IsNested = raw.IsNested;
			IsNestedAssembly = raw.IsNestedAssembly;
			IsNestedFamANDAssem = raw.IsNestedFamANDAssem;
			IsNestedFamily = raw.IsNestedFamily;
			IsNestedFamORAssem = raw.IsNestedFamORAssem;
			IsNestedPrivate = raw.IsNestedPrivate;
			IsNestedPublic = raw.IsNestedPublic;
			IsNotPublic = raw.IsNotPublic;
			IsPointer = raw.IsPointer;
			IsPrimitive = raw.IsPrimitive;
			IsPublic = raw.IsPublic;
			IsSealed = raw.IsSealed;
			IsSpecialName = raw.IsSpecialName;
			IsValueType = raw.IsValueType;
			IsVisible = raw.IsVisible;
		}

		public Type Raw { get; }
		
		public Assembly Assembly { get; }
		public string AssemblyQualifiedName { get; }
		public string FullName { get; }
		public string Name { get; }
		public string Namespace { get; }

		public bool HasElementType { get; }
		public bool IsArray { get; }
		public bool IsAbstract { get; }
		public bool IsByRef { get; }
		public bool IsClass { get; }
		public bool IsEnum { get; }
		public bool IsGenericParameter { get; }
		public bool IsGenericType { get; }
		public bool IsGenericTypeDefinition { get; }
		public bool IsInterface { get; }
		public bool IsNested { get; }
		public bool IsNestedAssembly { get; }
		public bool IsNestedFamANDAssem { get; }
		public bool IsNestedFamily { get; }
		public bool IsNestedFamORAssem { get; }
		public bool IsNestedPrivate { get; }
		public bool IsNestedPublic { get; }
		public bool IsNotPublic { get; }
		public bool IsPointer { get; }
		public bool IsPrimitive { get; }
		public bool IsPublic { get; }
		public bool IsSealed { get; }
		public bool IsSpecialName { get; }
		public bool IsValueType { get; }
		public bool IsVisible { get; }
	}
}