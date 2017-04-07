using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Art.Markup
{
    public static class Rack
    {
        #region Declarations

        public static readonly DependencyProperty ShowLinesProperty = DependencyProperty.RegisterAttached(
            "ShowLines", typeof (bool), typeof (Rack), new PropertyMetadata(default(bool), (o, args) =>
            {
                var grid = o as Grid;
                if (grid == null) return;
                grid.ShowGridLines = Equals(args.NewValue, true);
            }));

        public static void SetShowLines(DependencyObject element, bool value)
        {
            element.SetValue(ShowLinesProperty, value);
        }

        public static bool GetShowLines(DependencyObject element)
        {
            return (bool) element.GetValue(ShowLinesProperty);
        }

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

        public static readonly DependencyProperty SetProperty = DependencyProperty.RegisterAttached(
            "Set", typeof (string), typeof (Rack), new PropertyMetadata("", OnSetChangedCallback));

        public static void SetSet(DependencyObject element, string value)
        {
            element.SetValue(SetProperty, value);
        }

        public static string GetSet(DependencyObject element)
        {
            return (string) element.GetValue(SetProperty);
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

        private static void OnSetChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var element = o as FrameworkElement;
            if (element == null) return;
            var patterns = (e.NewValue as string ?? "").ToUpperInvariant().Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);
            var columnPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && !p.StartsWith("CS")).With(p => p.Replace("C", ""));
            var rowPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && !p.StartsWith("RS")).With(p => p.Replace("R", ""));
            var columnSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("CS")).With(p => p.Replace("CS", ""));
            var rowSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("RS")).With(p => p.Replace("RS", ""));
            //var sharedSizeGroupPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS")).With(p => p.Replace("SSS", ""));

            //bool sharedSizeScope;
            int column, row, columnSpan, rowSpan;
            //if (bool.TryParse(sharedSizeGroupPattern, out sharedSizeScope)) Grid.SetIsSharedSizeScope(element, sharedSizeScope);
            if (int.TryParse(columnSpanPattern, out columnSpan)) Grid.SetColumnSpan(element, columnSpan);
            if (int.TryParse(rowSpanPattern, out rowSpan)) Grid.SetRowSpan(element, rowSpan);
            if (int.TryParse(columnPattern, out column)) Grid.SetColumn(element, column);
            if (int.TryParse(rowPattern, out row)) Grid.SetRow(element, row);
        }

        private static GridLength ToGridLength(this string length)
        {
            try
            {
                length = length.Trim();
                if (length.ToLowerInvariant().Equals("auto")) return new GridLength(0, GridUnitType.Auto);
                if (!length.Contains("*")) return new GridLength(double.Parse(length), GridUnitType.Pixel);
                length = length.Replace("*", "");
                if (string.IsNullOrEmpty(length)) length = "1";
                return new GridLength(double.Parse(length), GridUnitType.Star);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return new GridLength();
            }
        }
    }
}