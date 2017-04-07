using System;
using System.Globalization;
using System.Windows.Data;

namespace Art.Converters
{
    public enum Letter
    {
        Original,
        Lower,
        Upper
    }

    public static class LetterSugar
    {
        public static string To(this string text, Letter letterCase)
        {
            switch (letterCase)
            {
                case Letter.Upper:
                    return text.ToUpper();
                case Letter.Lower:
                    return text.ToLower();
                default:
                    return text;
            }
        }
    }

    public class LetterCaseConverter : IValueConverter
    {
        public Letter Case { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            return text == null ? null : text.To(Case);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}