using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Art.Markup
{
    public class XamlTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Type);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null) return null;
            var typeName = value.ToString().Split(':').Last();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                var type = types.FirstOrDefault(t => (t.DeclaringType != null && t.DeclaringType.Name == typeName) || t.Name == typeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }
    }
}
