using System;
using System.Linq;

namespace Ace.Markup
{
#if XAMARIN
	using Xamarin.Forms;

	public class TypeTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value.IsNot()) return default;
			var typeName = value.ToString().Split(':').Last();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				var type = types.FirstOrDefault(t => typeName.Is(t.DeclaringType?.Name) || typeName.Is(t.Name));
				if (type.Is())
					return type;
			}

			return default;
		}
	}
#else
	using System.Globalization;
	using System.ComponentModel;

	public class TypeTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
			TypeOf.String.Raw.Is(sourceType);

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
			TypeOf.Type.Raw.Is(destinationType);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value.IsNot()) return null;
			var typeName = value.ToString().Split(':').Last();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				var type = types.FirstOrDefault(t => typeName.Is(t.DeclaringType?.Name) || typeName.Is(t.Name));
				if (type.Is())
					return type;
			}

			return null;
		}

		

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
			object value, Type destinationType) =>
			value.ToString();
	}
#endif
}