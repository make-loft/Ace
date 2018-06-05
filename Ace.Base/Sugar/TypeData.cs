using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Ace
{
	public class TypeData
	{
		public readonly Type Raw;
		public TypeData(Type raw) => Raw = raw;

		private string _name, _fullName, _assemblyQualifiedName, _namespace;
		private Assembly _assembly;

		public Assembly Assembly => _assembly ?? Raw.Assembly.To(out _assembly);
		public string Namespace => _namespace ?? Raw.Namespace.To(out _namespace);
		public string FullName => _fullName ?? Raw.FullName.To(out _fullName);
		public string Name => _name ?? Raw.Name.To(out _name);
		public string AssemblyQualifiedName =>
			_assemblyQualifiedName ?? Raw.AssemblyQualifiedName.To(out _assemblyQualifiedName);

		private const int CountOfProperties = 25;
		private readonly bool[] _states = new bool[CountOfProperties];
		private readonly bool[] _values = new bool[CountOfProperties];

		private bool GetValue(int index, Func<bool> source) =>
			_states[index] ? _values[index] : _values[index] = source();

		public bool HasElementType => GetValue(0, () => Raw.HasElementType);
		public bool IsArray => GetValue(1, () => Raw.IsArray);
		public bool IsAbstract => GetValue(2, () => Raw.IsAbstract);
		public bool IsByRef => GetValue(3, () => Raw.IsByRef);
		public bool IsClass => GetValue(4, () => Raw.IsClass);
		public bool IsEnum => GetValue(5, () => Raw.IsEnum);
		public bool IsGenericParameter => GetValue(6, () => Raw.IsGenericParameter);
		public bool IsGenericType => GetValue(7, () => Raw.IsGenericType);
		public bool IsGenericTypeDefinition => GetValue(8, () => Raw.IsGenericTypeDefinition);
		public bool IsInterface => GetValue(9, () => Raw.IsInterface);
		public bool IsNested => GetValue(10, () => Raw.IsNested);
		public bool IsNestedAssembly => GetValue(11, () => Raw.IsNestedAssembly);
		public bool IsNestedFamANDAssem => GetValue(12, () => Raw.IsNestedFamANDAssem);
		public bool IsNestedFamily => GetValue(13, () => Raw.IsNestedFamily);
		public bool IsNestedFamORAssem => GetValue(14, () => Raw.IsNestedFamORAssem);
		public bool IsNestedPrivate => GetValue(15, () => Raw.IsNestedPrivate);
		public bool IsNestedPublic => GetValue(16, () => Raw.IsNestedPublic);
		public bool IsNotPublic => GetValue(17, () => Raw.IsNotPublic);
		public bool IsPointer => GetValue(18, () => Raw.IsPointer);
		public bool IsPrimitive => GetValue(19, () => Raw.IsPrimitive);
		public bool IsPublic => GetValue(20, () => Raw.IsPublic);
		public bool IsSealed => GetValue(21, () => Raw.IsSealed);
		public bool IsSpecialName => GetValue(22, () => Raw.IsSpecialName);
		public bool IsValueType => GetValue(23, () => Raw.IsValueType);
		public bool IsVisible => GetValue(24, () => Raw.IsVisible);
	}
}