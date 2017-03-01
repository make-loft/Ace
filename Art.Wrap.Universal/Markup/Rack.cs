using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Aero.Markup
{
    public static class Rack
    {
        #region Declarations

        //public static readonly DependencyProperty ShowLinesProperty = DependencyProperty.RegisterAttached(
        //    "ShowLines", typeof (bool), typeof (Rack), new PropertyMetadata(default(bool), (o, args) =>
        //    {
        //        var grid = o as Grid;
        //        if (grid == null) return;
        //        grid.ShowGridLines = Equals(args.NewValue, true);
        //    }));

        //public static void SetShowLines(DependencyObject element, bool value)
        //{
        //    element.SetValue(ShowLinesProperty, value);
        //}

        //public static bool GetShowLines(DependencyObject element)
        //{
        //    return (bool) element.GetValue(ShowLinesProperty);
        //}

        public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
            "Rows", typeof (string), typeof (Rack), new PropertyMetadata("", OnRowsPropertyChanged));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
            "Columns", typeof (string), typeof (Rack), new PropertyMetadata("", OnColumnsPropertyChanged));

        public static string GetRows(DependencyObject d)
        {
            return (string) d.GetValue(RowsProperty);
        }

        public static void SetRows(DependencyObject d, string value)
        {
            d.SetValue(RowsProperty, value);
        }

        public static string GetColumns(DependencyObject d)
        {
            return (string) d.GetValue(ColumnsProperty);
        }

        public static void SetColumns(DependencyObject d, string value)
        {
            d.SetValue(ColumnsProperty, value);
        }

        #endregion

        private static void OnRowsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var grid = o as Grid;
            if (grid == null) return;

            grid.RowDefinitions.Clear();
            var patterns = (e.NewValue as string ?? "").Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pattern in patterns)
            {
                var indexMin = pattern.IndexOf(@"\", StringComparison.Ordinal);
                var indexMax = pattern.IndexOf(@"/", StringComparison.Ordinal);
                var hasMin = indexMin >= 0;
                var hasMax = indexMax >= 0;
                var valueMin = hasMin ? pattern.Substring(0, indexMin) : "";
                var valueMax = hasMax ? pattern.Substring(indexMax + 1, pattern.Length - indexMax - 1) : "";
                var start = hasMin ? indexMin + 1 : 0;
                var finish = hasMax ? indexMax : pattern.Length;
                var value = pattern.Substring(start, finish - start);
                var definition = new RowDefinition {Height = value.ToGridLength()};
                if (valueMin != "") definition.MinHeight = double.Parse(valueMin);
                if (valueMax != "") definition.MaxHeight = double.Parse(valueMax);
                grid.RowDefinitions.Add(definition);
            }
        }

        private static void OnColumnsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var grid = o as Grid;
            if (grid == null) return;

            grid.ColumnDefinitions.Clear();
            var patterns = (e.NewValue as string ?? "").Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pattern in patterns)
            {
                var indexMin = pattern.IndexOf(@"\", StringComparison.Ordinal);
                var indexMax = pattern.IndexOf(@"/", StringComparison.Ordinal);
                var hasMin = indexMin >= 0;
                var hasMax = indexMax >= 0;
                var valueMin = hasMin ? pattern.Substring(0, indexMin) : "";
                var valueMax = hasMax ? pattern.Substring(indexMax + 1, pattern.Length - indexMax - 1) : "";
                var start = hasMin ? indexMin + 1 : 0;
                var finish = hasMax ? indexMax : pattern.Length;
                var value = pattern.Substring(start, finish - start);
                var definition = new ColumnDefinition {Width = value.ToGridLength()};
                if (valueMin != "") definition.MinWidth = double.Parse(valueMin);
                if (valueMax != "") definition.MaxWidth = double.Parse(valueMax);
                grid.ColumnDefinitions.Add(definition);
            }
        }

        private static GridLength ToGridLength(this string length)
        {
            length = length.Trim();
            if (length.ToLowerInvariant().Equals("auto")) return new GridLength(0, GridUnitType.Auto);
            if (!length.Contains("*")) return new GridLength(double.Parse(length), GridUnitType.Pixel);
            length = length.Replace("*", "");
            if (string.IsNullOrEmpty(length)) length = "1";
            return new GridLength(double.Parse(length), GridUnitType.Star);
        }
    }
}