using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Art.Markup
{
    public class Smart : Patterns.ABindingExtension
    {
        public Smart()
            : base(null)
        {
            Mode = BindingMode.TwoWay;
            Path = new PropertyPath(SmartObject.SmartPropertyName);
        }

        public Smart(string set)
            : this()
        {
            Set = set;
        }

        public Smart(string key, string defaultValue)
            : this()
        {
            Key = key;
            DefaultValue = defaultValue;
        }

        public string Set
        {
            set { Initialize(value); }
        }

        public string Key { get; set; }
        public object DefaultValue { get; set; }
        public bool Segregate { get; set; }
        public new IValueConverter Converter { get; set; }
        public SmartObject SmartObject { get; private set; }
        public new object Source { get; set; }

        [TypeConverter(typeof(XamlTypeConverter))]
        public Type StoreKey
        {
            get { return Source == null ? null : Source.GetType(); }
            set
            {
                var itemType = value;
                var methodInfo = typeof(Art.Store).GetMethod("Get").
                    MakeGenericMethod(itemType.DeclaringType ?? itemType);
                Source = methodInfo.Invoke(null, new object[] { new object[0] });
            }
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SmartObject = (Source ?? value) as SmartObject;
            value = SmartObject == null ? null : SmartObject[Key, DefaultValue, Segregate];
            if (Segregate) value = value.Of<Segregator>().Value;
            return Converter == null ? value : Converter.Convert(value, targetType, parameter, culture);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (SmartObject == null) return null;
            value = Converter == null ? value : Converter.ConvertBack(value, targetType, parameter, culture);
            if (Segregate)
                SmartObject[Key].Of<Segregator>().Value = value;
            else SmartObject[Key] = value;
            return Segregate ? null : SmartObject;
        }

        private void Initialize(string set = "")
        {
            set = set.Replace("[", "").Replace("]", "");
            var parts = set.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) Key = parts[0].Trim();
            if (parts.Length > 1) DefaultValue = parts[1].Trim();
            if (parts.Length > 2)
                Segregate = parts[2].Trim().ToLower() == "true" || parts[2].Trim().ToLower() == "segregate";
        }
    }
}