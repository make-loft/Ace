using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ace.Serialization.Converters;

namespace Ace.Serialization
{
	public class SimplexConverter
	{
		public bool AppendTypeInfo = true;

		public List<Converter> Converters = New.List<Converter>
		(
			new NullConverter(),
			new BooleanConverter(),
			new NumericConverter(),
			new StringConverter(),
			new IsoDateTimeConverter(),
			new ComplexConverter()
		);
		
		public static Assembly SystemAssembly = TypeOf<object>.Assembly;
		public static Assembly ExtendedAssembly = TypeOf<Uri>.Assembly;

		public virtual string GetTypeName(Type type) =>
			type.Assembly.Is(SystemAssembly) || type.Assembly.Is(ExtendedAssembly)
				? type.Name
				: type.AssemblyQualifiedName;
		
		protected readonly Simplex Simplex = new Simplex();
	
		public Simplex Convert(object value, KeepProfile keepProfile)
		{
			var convertedValue = Converters.Select(c => c.Convert(value)).FirstOrDefault(s => s != null) ??
								 throw new Exception("Can not convert value " + value);
			
			Simplex.Clear();
			Simplex.Add(keepProfile.GetHead(value));
			Simplex.Add(convertedValue);
			Simplex.Add(keepProfile.GetTail(value));
			
			var type = value?.GetType();
			if (type == null || type.IsPrimitive) return Simplex;
			if (type == TypeOf.String.Raw) return Simplex.Escape(keepProfile.EscapeProfile, 1);

			if (!AppendTypeInfo) return Simplex.Escape(keepProfile.EscapeProfile, 1);
			
			Simplex.Add(keepProfile.GetHead(type));
			Simplex.Add(GetTypeName(type));
			Simplex.Add(keepProfile.GetTail(type));
			return Simplex.Escape(keepProfile.EscapeProfile, 1);
		}

		public object Revert(Simplex simplex)
		{
			if (simplex.Count == 3) return simplex[1]; /* optimization for strings */
			var convertedValue = simplex.Count == 1 ? simplex[0] : simplex[1];
			var typeCode = simplex.Count == 6 ? simplex[4] : null;
			return Converters.Select(c => c.Revert(convertedValue, typeCode))
					   .First(v => v != Converter.Undefined);
		}
	}
}