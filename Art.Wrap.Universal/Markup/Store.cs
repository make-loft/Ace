using System;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Data;
using Aero.Markup.Patterns;

namespace Aero.Markup
{
    public class Store : ABindingExtension
    {
        public Store() : base(new RelativeSource { Mode = RelativeSourceMode.Self })
        {
        }

        public static Assembly SourceAssembly { get; set; }

        public string Key { get; set; }

        public override object Convert(object value, Type targetType, object parameter, string culture)
        {
            var typeName = Key ?? parameter as string;

            if (SourceAssembly == null) throw new Exception("Please, initialize source assembly.");

            var types = SourceAssembly.DefinedTypes;
            var type = types.FirstOrDefault(t => (t.DeclaringType != null && t.DeclaringType.FullName == typeName) || t.FullName == typeName);
            if (type == null) throw new Exception(string.Format("Type '{0}' not found!", typeName));

            var methodInfo = typeof(Aero.Store).GetTypeInfo().GetDeclaredMethod("Get").
                MakeGenericMethod(type.AsType().DeclaringType ?? type.AsType());
            var item = methodInfo.Invoke(null, new object[] { new object[0] });
            return item;
        }
    }
}