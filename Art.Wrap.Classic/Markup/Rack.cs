using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Art.Markup
{
    public static class Rack
    {
        #region Declarations

        public static void SetRow(DependencyObject d, int value) => d.SetValue(Grid.RowProperty, value);
        public static int GetRow(DependencyObject d) => (int) d.GetValue(Grid.RowProperty);
        public static void SetColumn(DependencyObject d, int value) => d.SetValue(Grid.ColumnProperty, value);
        public static int GetColumn(DependencyObject d) => (int) d.GetValue(Grid.ColumnProperty);
        public static string GetRows(DependencyObject d) => (string) d.GetValue(RowsProperty);
        public static void SetRows(DependencyObject d, string value) => d.SetValue(RowsProperty, value);
        public static string GetColumns(DependencyObject d) => (string) d.GetValue(ColumnsProperty);
        public static void SetColumns(DependencyObject d, string value) => d.SetValue(ColumnsProperty, value);
        public static void SetSet(DependencyObject d, string value) => d.SetValue(SetProperty, value);
        public static string GetSet(DependencyObject d) => (string) d.GetValue(SetProperty);
        public static void SetShowLines(DependencyObject d, bool value) => d.SetValue(ShowLinesProperty, value);
        public static bool GetShowLines(DependencyObject d) => (bool) d.GetValue(ShowLinesProperty);

        public static readonly DependencyProperty ShowLinesProperty = DependencyProperty.RegisterAttached(
            "ShowLines", typeof(bool), typeof(Rack), new PropertyMetadata(default(bool), (o, args) =>
            {
                var grid = o as Grid;
                if (grid == null) return;
                grid.ShowGridLines = Equals(args.NewValue, true);
            }));

        public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
            "Rows", typeof(string), typeof(Rack), new PropertyMetadata("", OnRowsPropertyChanged));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
            "Columns", typeof(string), typeof(Rack), new PropertyMetadata("", OnColumnsPropertyChanged));

        public static readonly DependencyProperty SetProperty = DependencyProperty.RegisterAttached(
            "Set", typeof(string), typeof(Rack), new PropertyMetadata("", OnSetChangedCallback));

        #endregion
        
                public static readonly DependencyProperty TwoWayModeProperty = DependencyProperty.RegisterAttached(
            "TwoWayMode", typeof(bool), typeof(Rack), new PropertyMetadata(default(bool), (o, args) =>
            {
                var grid = o as Grid;
                if (grid == null) return;
                
                //BindColumns(grid);
               // BindRows(grid);
            }));

        public static readonly DependencyProperty StabProperty = DependencyProperty.RegisterAttached(
            "Stab", typeof(object), typeof(Rack), new PropertyMetadata(default(object), (o, args) =>
            {
                var grid = o as Grid;
                if (grid == null) return;
                
                grid.SetValue(ColumnsProperty, GetColumnsValue(grid));
                grid.SetValue(RowsProperty, GetRowsValue(grid));
                grid.SetValue(StabProperty, null);
            }));

        public static void SetStab(DependencyObject element, object value)
        {
            element.SetValue(StabProperty, value);
        }

        public static object GetStab(DependencyObject element)
        {
            return (object) element.GetValue(StabProperty);
        }
        
        public static void SetTwoWayMode(DependencyObject d, bool value)
        {
            d.SetValue(TwoWayModeProperty, value);
        }

        public static bool GetTwoWayMode(DependencyObject d)
        {
            return (bool) d.GetValue(TwoWayModeProperty);
        }

        private static void BindRows(Grid grid)
        {
            foreach (var defenition in grid.RowDefinitions)
            {
                BindingOperations.SetBinding(defenition, RowDefinition.HeightProperty,
                    new Binding
                    {
                        Source = grid,
                        Path = new PropertyPath(StabProperty),
                        Mode = BindingMode.OneWayToSource,
                        FallbackValue = defenition.Height
                    });
            }
        }
        
        private static void BindColumns(Grid grid)
        {
            foreach (var defenition in grid.ColumnDefinitions)
            {
                BindingOperations.SetBinding(defenition, ColumnDefinition.WidthProperty,
                    new Binding
                    {
                        Source = grid,
                        Path = new PropertyPath(StabProperty),
                        Mode = BindingMode.OneWayToSource,
                        FallbackValue = defenition.Width
                    });
            }
        }

        private static string GetRowsValue(Grid grid) => 
            grid.RowDefinitions.Aggregate("", (s, definition) => s + (s == "" ? null : " ") + definition.Height);
        
        private static string GetColumnsValue(Grid grid) => 
            grid.ColumnDefinitions.Aggregate("", (s, definition) => s + (s == "" ? null : " ") + definition.Width);


        private static void OnRowsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var grid = o as Grid;
            if (grid == null) return;
            if (grid.GetValue(StabProperty) != null) return;

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
            
            if (grid.GetValue(TwoWayModeProperty).Equals(true)) BindRows(grid);
        }

        private static void OnColumnsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var grid = o as Grid;
            if (grid == null) return;
            if (grid.GetValue(StabProperty) != null) return;

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
            
            if (grid.GetValue(TwoWayModeProperty).Equals(true)) BindColumns(grid);
        }

        private static void OnSetChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var d = o as FrameworkElement;
            if (d == null) return;
            var patterns = (e.NewValue as string ?? "").ToUpperInvariant()
                .Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);
            var columnPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && !p.StartsWith("CS"))
                .With(p => p.Replace("C", ""));
            var rowPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && !p.StartsWith("RS"))
                .With(p => p.Replace("R", ""));
            var columnSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("CS")).With(p => p.Replace("CS", ""));
            var rowSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("RS")).With(p => p.Replace("RS", ""));
            //var sharedSizeGroupPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS")).With(p => p.Replace("SSS", ""));

            //bool sharedSizeScope;
            int column, row, columnSpan, rowSpan;
            //if (bool.TryParse(sharedSizeGroupPattern, out sharedSizeScope)) Grid.SetIsSharedSizeScope(d, sharedSizeScope);
            if (int.TryParse(columnSpanPattern, out columnSpan)) Grid.SetColumnSpan(d, columnSpan);
            if (int.TryParse(rowSpanPattern, out rowSpan)) Grid.SetRowSpan(d, rowSpan);
            if (int.TryParse(columnPattern, out column)) Grid.SetColumn(d, column);
            if (int.TryParse(rowPattern, out row)) Grid.SetRow(d, row);
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