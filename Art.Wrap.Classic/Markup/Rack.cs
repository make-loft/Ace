using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static void SetTwoWayMode(DependencyObject d, bool value) => d.SetValue(TwoWayModeProperty, value);
        public static bool GetTwoWayMode(DependencyObject d) => (bool) d.GetValue(TwoWayModeProperty);

        public static readonly DependencyProperty ShowLinesProperty = DependencyProperty.RegisterAttached(
            "ShowLines", typeof(bool), typeof(Rack), new PropertyMetadata(default(bool), (o, e) =>
            {
                var grid = o as Grid;
                if (grid == null) return;
                grid.ShowGridLines = Equals(e.NewValue, true);
            }));

        public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
            "Rows", typeof(string), typeof(Rack), new PropertyMetadata(default(string), (o, args) =>
            {
                var grid = o as Grid;
                if (grid == null || args.NewValue == null) return;
                if (grid.GetValue(UpdateTriggerProperty) != null) return;

                UpdateDefinitions(grid, grid.RowDefinitions, args.NewValue.ToString(),
                    ToRowDefinition, BindRowDefinitionWithUpdateTrigger);
            }));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
            "Columns", typeof(string), typeof(Rack), new PropertyMetadata(default(string), (o, args) =>
            {
                var grid = o as Grid;
                if (grid == null || args.NewValue == null) return;
                if (grid.GetValue(UpdateTriggerProperty) != null) return;

                UpdateDefinitions(grid, grid.ColumnDefinitions, args.NewValue.ToString(),
                    ToColumnDefinition, BindColumnDefinitionWithUpdateTrigger);
            }));

        public static readonly DependencyProperty SetProperty = DependencyProperty.RegisterAttached(
            "Set", typeof(string), typeof(Rack), new PropertyMetadata("", OnSetChangedCallback));

        public static readonly DependencyProperty TwoWayModeProperty = DependencyProperty.RegisterAttached(
            "TwoWayMode", typeof(bool), typeof(Rack), new PropertyMetadata(default(bool), (o, e) =>
            {
                var grid = o as Grid;
                if (grid == null) return;
                if (Equals(e.NewValue, true))
                {
                    grid.RowDefinitions.ForEach(d => BindRowDefinitionWithUpdateTrigger(d, grid));
                    grid.ColumnDefinitions.ForEach(d => BindColumnDefinitionWithUpdateTrigger(d, grid));
                }
                else
                {
                    UpdateDefinitions(grid, grid.RowDefinitions, grid.GetValue(RowsProperty) as string,
                        ToRowDefinition, BindRowDefinitionWithUpdateTrigger);
                    UpdateDefinitions(grid, grid.ColumnDefinitions, grid.GetValue(ColumnsProperty) as string,
                        ToColumnDefinition, BindColumnDefinitionWithUpdateTrigger);
                }
            }));

        public static readonly DependencyProperty UpdateTriggerProperty = DependencyProperty.RegisterAttached(
            "UpdateTrigger", typeof(object), typeof(Rack), new PropertyMetadata(default(object), (o, e) =>
            {
                var grid = o as Grid;
                if (grid == null) return;

                var columnsPattern = grid.ColumnDefinitions.Select(ToPattern).GluePatterns();
                var rowsPattern = grid.RowDefinitions.Select(ToPattern).GluePatterns();
                grid.SetValue(ColumnsProperty, columnsPattern);
                grid.SetValue(RowsProperty, rowsPattern);
                grid.SetValue(UpdateTriggerProperty, null);
            }));

        #endregion

        private static void BindRowDefinitionWithUpdateTrigger(RowDefinition definition, Grid grid) =>
            BindingOperations.SetBinding(definition, RowDefinition.HeightProperty, new Binding
            {
                Source = grid,
                Path = new PropertyPath(UpdateTriggerProperty),
                Mode = BindingMode.OneWayToSource,
                FallbackValue = definition.Height
            });

        private static void BindColumnDefinitionWithUpdateTrigger(ColumnDefinition definition, Grid grid) =>
            BindingOperations.SetBinding(definition, ColumnDefinition.WidthProperty, new Binding
            {
                Source = grid,
                Path = new PropertyPath(UpdateTriggerProperty),
                Mode = BindingMode.OneWayToSource,
                FallbackValue = definition.Width
            });

        private static string ToPattern(GridLength gridLength, double minValue, double maxValue) =>
            (Equals(minValue, .0) ? null : minValue + "\\") + gridLength +
            (Equals(maxValue, double.PositiveInfinity) ? null : "/" + maxValue);

        private static string ToPattern(this RowDefinition definition) =>
            ToPattern(definition.Height, definition.MinHeight, definition.MaxHeight);

        private static string ToPattern(this ColumnDefinition definition) =>
            ToPattern(definition.Width, definition.MinWidth, definition.MaxWidth);

        public static TDefinition ToDefinition<TDefinition>(this string pattern,
            Func<string, string, string, TDefinition> definitionDecoder) where TDefinition : DefinitionBase
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
            return definitionDecoder(value, valueMin, valueMax);
        }

        private static RowDefinition ToRowDefinition(string gridLength, string minValue, string maxValue) =>
            new RowDefinition
            {
                Height = gridLength.ToGridLength(),
                MinHeight = double.TryParse(minValue, out var min) ? min : .0,
                MaxHeight = double.TryParse(maxValue, out var max) ? max : double.PositiveInfinity
            };

        private static ColumnDefinition ToColumnDefinition(string gridLength, string minValue, string maxValue) =>
            new ColumnDefinition
            {
                Width = gridLength.ToGridLength(),
                MinWidth = double.TryParse(minValue, out var min) ? min : .0,
                MaxWidth = double.TryParse(maxValue, out var max) ? max : double.PositiveInfinity
            };

        private static string GluePatterns(this IEnumerable<string> patterns) =>
            patterns.Aggregate(new StringBuilder(),
                (builder, pattern) => builder.Append(builder.Length == 0 ? null : " ").Append(pattern)).ToString();

        public static string[] SplitPaterns(string value) =>
            value?.Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);

        public static void UpdateDefinitions<T>(Grid grid, ICollection<T> definitionCollection, string pattern,
            Func<string, string, string, T> definitionFactory, Action<T, Grid> definitionBinder)
            where T : DefinitionBase
        {
            var patterns = SplitPaterns(pattern);
            var definitions = patterns.Select(p => p.ToDefinition(definitionFactory));
            definitionCollection.Clear();
            definitions.ForEach(definitionCollection.Add);
            if (grid.GetValue(TwoWayModeProperty).Equals(false)) return;
            definitionCollection.ForEach(d => definitionBinder(d, grid));
        }

        private static void OnSetChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var element = o as UIElement;
            if (element == null) return;

            var patterns = SplitPaterns((args.NewValue as string ?? "").ToUpperInvariant());
            var cPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && !p.StartsWith("CS"))?.Replace("C", "");
            var rPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && !p.StartsWith("RS"))?.Replace("R", "");
            var sssPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS")).With(p => p.Replace("SSS", ""));
            var cSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("CS"))?.Replace("CS", "");
            var rSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("RS"))?.Replace("RS", "");

            if (bool.TryParse(sssPattern, out var sharedSizeScope)) Grid.SetIsSharedSizeScope(element, sharedSizeScope);
            if (int.TryParse(cSpanPattern, out var columnSpan)) Grid.SetColumnSpan(element, columnSpan);
            if (int.TryParse(rSpanPattern, out var rowSpan)) Grid.SetRowSpan(element, rowSpan);
            if (int.TryParse(cPattern, out var column)) Grid.SetColumn(element, column);
            if (int.TryParse(rPattern, out var row)) Grid.SetRow(element, row);
        }

        private static GridLength ToGridLength(this string pattern)
        {
            var unitType = pattern.Contains("*") ? GridUnitType.Star : GridUnitType.Pixel;
            pattern = unitType == GridUnitType.Star ? pattern.Replace("*", "") : pattern;
            pattern = unitType == GridUnitType.Star && string.IsNullOrWhiteSpace(pattern) ? "1" : pattern;
            return double.TryParse(pattern, out var value) ? new GridLength(value, unitType) : new GridLength();
        }
    }
}