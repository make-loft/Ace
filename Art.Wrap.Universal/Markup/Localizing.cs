using System;
using System.Resources;
using Windows.UI.Xaml;
using Aero.Markup.Patterns;

namespace Aero.Markup
{
    public partial class Localizing : ABindingExtension
    {
        public enum Cases
        {
            Default,
            Lower,
            Upper
        }

        public static readonly Manager ActiveManager = new Manager();

        public Localizing()
        {
            Source = ActiveManager;
            Path = new PropertyPath("Source");
        }

        public Localizing(string key)
        {
            Key = key;
            Source = ActiveManager;
            Path = new PropertyPath("Source");
        }

        public string Key { get; set; }
        public Cases Case { get; set; }
        public string StringFormat { get; set; }

        public override string ToString()
        {
            return Convert(ActiveManager.Source, null, Key, string.Empty) as string ?? string.Empty;
        }

        public override object Convert(object value, Type targetType, object parameter, string culture)
        {
            var key = Key;
            var resourceManager = value as ResourceManager;
            var localizedValue = resourceManager == null || string.IsNullOrEmpty(key)
                ? ":" + key + ":"
                : (resourceManager.GetString(key) ?? ":" + key + ":");
            if (!string.IsNullOrEmpty(StringFormat))
                localizedValue = string.Format(StringFormat, localizedValue);

            switch (Case)
            {
                case Cases.Lower:
                    return localizedValue.ToLower();
                case Cases.Upper:
                    return localizedValue.ToUpper();
                default:
                    return localizedValue;
            }
        }
    }
}