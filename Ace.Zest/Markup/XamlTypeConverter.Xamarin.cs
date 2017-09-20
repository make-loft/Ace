using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Ace.Markup
{
    public class XamlTypeConverter : TypeConverter
    {
        public static readonly List<Assembly> RegisteredAssemblies = new List<Assembly>();

        public override bool CanConvertFrom(Type sourceType) =>
            sourceType == typeof(string);

        public override object ConvertFromInvariantString(string value)
        {
            if (value == null) return null;
            var typeName = value.Split(':').Last();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var assembly in RegisteredAssemblies)
            {
                var types = assembly.DefinedTypes;
                var type = types.FirstOrDefault(t =>
                    (t.DeclaringType != null && t.DeclaringType.Name == typeName) || t.Name == typeName);
                if (type != null)
                    return type;
            }

            return null;
        }
    }
}