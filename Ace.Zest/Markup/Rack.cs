using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RowDef = System.Windows.Controls.RowDefinition;
using ColDef = System.Windows.Controls.ColumnDefinition;
using static System.Windows.DependencyProperty;

namespace Ace.Markup
{
	public static class Rack
	{
		#region Declarations

		public static void SetRow(DependencyObject d, int value) => d.SetValue(Grid.RowProperty, value);
		public static void SetColumn(DependencyObject d, int value) => d.SetValue(Grid.ColumnProperty, value);
		public static void SetRows(DependencyObject d, string value) => d.SetValue(RowsProperty, value);
		public static void SetColumns(DependencyObject d, string value) => d.SetValue(ColumnsProperty, value);
		public static void SetSet(DependencyObject d, string value) => d.SetValue(SetProperty, value);
		public static void SetShowLines(DependencyObject d, bool value) => d.SetValue(ShowLinesProperty, value);

		public static int GetRow(DependencyObject d) => (int) d.GetValue(Grid.RowProperty);
		public static int GetColumn(DependencyObject d) => (int) d.GetValue(Grid.ColumnProperty);
		public static string GetRows(DependencyObject d) => (string) d.GetValue(RowsProperty);
		public static string GetColumns(DependencyObject d) => (string) d.GetValue(ColumnsProperty);
		public static string GetSet(DependencyObject d) => (string) d.GetValue(SetProperty);
		public static bool GetShowLines(DependencyObject d) => (bool) d.GetValue(ShowLinesProperty);

		private static PropertyMetadata GetMetadata<T>(Action<T, DependencyPropertyChangedEventArgs> action) =>
			new PropertyMetadata((o, args) =>
			{
				if (o.Is(out T sender) && args.NewValue.IsNot(args.OldValue)) action(sender, args);
			});

		public static readonly DependencyProperty ShowLinesProperty = RegisterAttached(
			"ShowLines", typeof(bool), typeof(Rack), GetMetadata<Grid>((grid, args) =>
				grid.ShowGridLines = args.NewValue.Is(true)));

		public static readonly DependencyProperty RowsProperty = RegisterAttached(
			"Rows", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(
				grid, grid.RowDefinitions, args.NewValue?.ToString(),
				RowDef.HeightProperty, RowDef.MinHeightProperty, RowDef.MaxHeightProperty,
				RowsIsInUpdateProperty, RowsUpdateTriggerPropertyPath)));

		public static readonly DependencyProperty ColumnsProperty = RegisterAttached(
			"Columns", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(
				grid, grid.ColumnDefinitions, args.NewValue?.ToString(),
				ColDef.WidthProperty, ColDef.MinWidthProperty, ColDef.MaxWidthProperty,
				ColsIsInUpdateProperty, ColsUpdateTriggerPropertyPath)));

		public static readonly DependencyProperty SetProperty = RegisterAttached(
			"Set", typeof(string), typeof(Rack), GetMetadata<FrameworkElement>(OnSetChangedCallback));

		private static readonly DependencyProperty RowsIsInUpdateProperty = RegisterAttached(
			"RowsIsInUpdate", typeof(object), typeof(Rack), default);

		private static readonly DependencyProperty ColsIsInUpdateProperty = RegisterAttached(
			"ColsIsInUpdate", typeof(object), typeof(Rack), default);

		private static readonly DependencyProperty RowsUpdateTriggerProperty = RegisterAttached(
			"RowsUpdateTrigger", typeof(object), typeof(Rack), GetMetadata<Grid>((grid, args) =>
			{
				grid.SetValue(RowsIsInUpdateProperty, true);

				var newRowsPatterns = grid.RowDefinitions.Select(ToPattern).ToArray();
				var newRowsPattern = newRowsPatterns.GluePatterns();
				var oldRowsPattern = (string) grid.GetValue(RowsProperty);
				var oldRowsPatterns = SplitPaterns(oldRowsPattern);

				if (newRowsPatterns.Length.Is(oldRowsPatterns.Length) && newRowsPattern.IsNot(oldRowsPattern))
					grid.SetValue(RowsProperty, newRowsPatterns.GluePatterns());

				grid.SetValue(RowsIsInUpdateProperty, false);
			}));

		private static readonly DependencyProperty ColsUpdateTriggerProperty = RegisterAttached(
			"ColsUpdateTrigger", typeof(object), typeof(Rack), GetMetadata<Grid>((grid, args) =>
			{
				grid.SetValue(ColsIsInUpdateProperty, true);

				var newColsPatterns = grid.ColumnDefinitions.Select(ToPattern).ToArray();
				var newColsPattern = newColsPatterns.GluePatterns();
				var oldColsPattern = (string) grid.GetValue(ColumnsProperty);
				var oldColsPatterns = SplitPaterns(oldColsPattern);

				if (newColsPatterns.Length.Is(oldColsPatterns.Length) && newColsPattern.IsNot(oldColsPattern))
					grid.SetValue(ColumnsProperty, newColsPatterns.GluePatterns());

				grid.SetValue(ColsIsInUpdateProperty, false);
			}));

		private static readonly PropertyPath RowsUpdateTriggerPropertyPath = new PropertyPath(RowsUpdateTriggerProperty);
		private static readonly PropertyPath ColsUpdateTriggerPropertyPath = new PropertyPath(ColsUpdateTriggerProperty);

		#endregion

		private static string ToPattern(this RowDef definition) => definition.ToPattern(
			RowDef.HeightProperty, RowDef.MinHeightProperty, RowDef.MaxHeightProperty);

		private static string ToPattern(this ColDef definition) => definition.ToPattern(
			ColDef.WidthProperty, ColDef.MinWidthProperty, ColDef.MaxWidthProperty);

		private static string ToPattern(this DependencyObject definition,
			DependencyProperty lengthProperty,
			DependencyProperty minValueProperty,
			DependencyProperty maxValueProperty)
		{
			var lengthBinding = definition.GetBinding(lengthProperty);
			var minValueBinding = definition.GetBinding(minValueProperty);
			var maxValueBinding = definition.GetBinding(maxValueProperty);

			var length = definition.GetValue(lengthProperty).To<GridLength>();
			var minValue = definition.GetValue(minValueProperty);
			var maxValue = definition.GetValue(maxValueProperty);

			var builder = new StringBuilder();

			var isDefaultMinValue = minValue.Is(.0);
			var hasMinValueBinding = minValueBinding.Is(); //UpdateTriggerPropertyPath.Is(minValueBinding?.Path);
			builder.Append(isDefaultMinValue && !hasMinValueBinding ? null : minValue);
			builder.Append(!hasMinValueBinding ? null : "\\");

			var rounded = new GridLength(Math.Round(length.Value * 10d) / 10d, length.GridUnitType);
			var hasLengthBinding = lengthBinding.Is(); //UpdateTriggerPropertyPath.Is(lengthBinding?.Path);
			builder.Append(hasLengthBinding ? rounded.ToString() : null);

			var isDefaultMaxValue = maxValue.Is(double.PositiveInfinity);
			var hasMaxValueBinding = maxValueBinding.Is(); //UpdateTriggerPropertyPath.Is(maxValueBinding?.Path);
			builder.Append(!hasMaxValueBinding ? null : "/");
			builder.Append(isDefaultMaxValue && !hasMaxValueBinding ? null : maxValue);

			return builder.ToString();
		}

		private static void SetValues<TDefinition>(this TDefinition definition,
			string pattern, Grid grid,
			DependencyProperty lengthProperty,
			DependencyProperty minValueProperty,
			DependencyProperty maxValueProperty,
			PropertyPath updateTriggerPropertyPath)
			where TDefinition : DependencyObject, new()
		{
			var indexMin = pattern.IndexOf(@"\", StringComparison.Ordinal);
			var indexMax = pattern.IndexOf(@"/", StringComparison.Ordinal);
			var hasMinInPattern = indexMin >= 0;
			var hasMaxInPattern = indexMax >= 0;
			var minValuePattern = hasMinInPattern ? pattern.Substring(0, indexMin) : "";
			var maxValuePattern = hasMaxInPattern ? pattern.Substring(indexMax + 1, pattern.Length - indexMax - 1) : "";
			var start = hasMinInPattern ? indexMin + 1 : 0;
			var finish = hasMaxInPattern ? indexMax : pattern.Length;
			var lengthPattern = pattern.Substring(start, finish - start);
			var hasLengthInPattern = lengthPattern.IsNullOrWhiteSpace().Not();

			if (hasLengthInPattern) definition.SetValue(lengthProperty, lengthPattern.ToGridLength());
			else definition.ClearBinding(lengthProperty);
			if (hasMinInPattern)
				definition.SetValue(minValueProperty, minValuePattern.TryParse(out double minValue) ? minValue : .0);
			else definition.ClearBinding(minValueProperty);
			if (hasMaxInPattern)
				definition.SetValue(maxValueProperty,
					maxValuePattern.TryParse(out double maxValue) ? maxValue : double.PositiveInfinity);
			else definition.ClearBinding(maxValueProperty);

			if (hasLengthInPattern && definition.GetBinding(lengthProperty).IsNot())
				Bind(grid, definition, lengthProperty, updateTriggerPropertyPath);
			if (hasMinInPattern && definition.GetBinding(minValueProperty).IsNot())
				Bind(grid, definition, minValueProperty, updateTriggerPropertyPath);
			if (hasMaxInPattern && definition.GetBinding(maxValueProperty).IsNot())
				Bind(grid, definition, maxValueProperty, updateTriggerPropertyPath);
		}

		private static void Bind(Grid grid, DependencyObject definition, DependencyProperty property,
			PropertyPath updateTriggerPropertyPath) =>
			BindingOperations.SetBinding(definition, property, new Binding
			{
				Source = grid,				
#if !WINDOWS_PHONE
				Path = updateTriggerPropertyPath,
				Mode = BindingMode.OneWayToSource,
#endif
				FallbackValue = definition.GetValue(property)
			});

		private static string GluePatterns(this IEnumerable<string> patterns) => patterns.Aggregate(new StringBuilder(),
			(builder, pattern) => builder.Append(builder.Length.Is(0) ? null : " ").Append(pattern)).ToString();

		private static readonly char[] PatternSplitters = {' ', ','};

		private static string[] SplitPaterns(this string pattern) =>
			pattern?.Split(PatternSplitters, StringSplitOptions.RemoveEmptyEntries);

		private static void UpdateDefinitions<TDefinition>(Grid grid,
			IList<TDefinition> definitions, string pattern,
			DependencyProperty lengthProperty,
			DependencyProperty minValueProperty,
			DependencyProperty maxValueProperty,
			DependencyProperty updateTriggerProperty,
			PropertyPath path)
			where TDefinition : DependencyObject, new()
		{
			if (grid.GetValue(updateTriggerProperty).Is(true) || pattern.IsNot()) return;

			var patterns = SplitPaterns(pattern);

			definitions.Clear();
			patterns.Select(p =>
			{
				var d = new TDefinition();
				d.SetValues(p, grid, lengthProperty, minValueProperty, maxValueProperty, path);
				return d;
			}).ForEach(definitions.Add);
		}

		private static void OnSetChangedCallback(FrameworkElement element, DependencyPropertyChangedEventArgs args)
		{
			var patterns = args.NewValue.As("").ToUpperInvariant().SplitPaterns();
			var colPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && !p.StartsWith("CS"))?.Replace("C", "");
			var rowPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && !p.StartsWith("RS"))?.Replace("R", "");
			var sssPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS"))?.Replace("SSS", "");
			var colSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("CS"))?.Replace("CS", "");
			var rowSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("RS"))?.Replace("RS", "");
#if !WINDOWS_PHONE
			if (sssPattern.TryParse(out bool sharedSizeScope)) Grid.SetIsSharedSizeScope(element, sharedSizeScope);
#endif
			if (colSpanPattern.TryParse(out int colSpan)) Grid.SetColumnSpan(element, colSpan);
			if (rowSpanPattern.TryParse(out int rowSpan)) Grid.SetRowSpan(element, rowSpan);
			if (colPattern.TryParse(out int col)) Grid.SetColumn(element, col);
			if (rowPattern.TryParse(out int row)) Grid.SetRow(element, row);
		}

		private static GridLength ToGridLength(this string pattern)
		{
			var unitType = pattern.Contains("*") ? GridUnitType.Star : GridUnitType.Pixel;
			pattern = unitType.Is(GridUnitType.Star) ? pattern.Replace("*", "") : pattern;
			pattern = unitType.Is(GridUnitType.Star) && pattern.IsNullOrWhiteSpace() ? "1" : pattern;
			return pattern.TryParse(out double value) ? new GridLength(value, unitType) : new GridLength();
		}
	}
}