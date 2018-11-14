using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Xamarin.Forms;
using RowDef = Xamarin.Forms.RowDefinition;
using ColDef = Xamarin.Forms.ColumnDefinition;
using BindableObject = Xamarin.Forms.BindableObject;
using BindingMode = Xamarin.Forms.BindingMode;
using Grid = Xamarin.Forms.Grid;
using GridUnitType = Xamarin.Forms.GridUnitType;
using static System.Windows.DependencyProperty;
using Binding = System.Windows.Data.Binding;

namespace Ace.Markup
{
	public static class Rack
	{
		#region Declarations

		public static void SetRow(BindableObject d, int value) => d.SetValue(Grid.RowProperty, value);
		public static void SetColumn(BindableObject d, int value) => d.SetValue(Grid.ColumnProperty, value);
		public static void SetRows(BindableObject d, string value) => d.SetValue(RowsProperty, value);
		public static void SetColumns(BindableObject d, string value) => d.SetValue(ColumnsProperty, value);
		public static void SetSet(BindableObject d, string value) => d.SetValue(SetProperty, value);
		//public static void SetShowLines(BindableObject d, bool value) => d.SetValue(ShowLinesProperty, value);

		public static int GetRow(BindableObject d) => (int) d.GetValue(Grid.RowProperty);
		public static int GetColumn(BindableObject d) => (int) d.GetValue(Grid.ColumnProperty);
		public static string GetRows(BindableObject d) => (string) d.GetValue(RowsProperty);
		public static string GetColumns(BindableObject d) => (string) d.GetValue(ColumnsProperty);
		public static string GetSet(BindableObject d) => (string) d.GetValue(SetProperty);
		//public static bool GetShowLines(BindableObject d) => (bool) d.GetValue(ShowLinesProperty);

		private static PropertyMetadata GetMetadata<T>(Action<T, DependencyPropertyChangedEventArgs> action) =>
			new PropertyMetadata((o, args) =>
			{
				if (o.Is(out T sender) && args.NewValue.IsNot(args.OldValue)) action(sender, args);
			});

		//public static readonly DependencyProperty ShowLinesProperty = RegisterAttached(
		//	"ShowLines", typeof(bool), typeof(Rack), GetMetadata<Grid>((grid, args) =>
		//		grid.ShowGridLines = args.NewValue.Is(true)));

		public static readonly BindableProperty RowsProperty = RegisterAttached(
			"Rows", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(
				grid, grid.RowDefinitions, args.NewValue?.ToString(),
				RowDef.HeightProperty, // RowDef.HeightProperty, RowDef.HeightProperty,
				RowsIsInUpdateProperty, RowsUpdateTriggerPropertyPath))).CoreProperty;

		public static readonly BindableProperty ColumnsProperty = RegisterAttached(
			"Columns", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(
				grid, grid.ColumnDefinitions, args.NewValue?.ToString(),
				ColDef.WidthProperty, // ColDef.WidthProperty, ColDef.WidthProperty,
				ColsIsInUpdateProperty, ColsUpdateTriggerPropertyPath))).CoreProperty;

		public static readonly DependencyProperty SetProperty = RegisterAttached(
			"Set", typeof(string), typeof(Rack), GetMetadata<BindableObject>(OnSetChangedCallback));

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

		private static readonly PropertyPath RowsUpdateTriggerPropertyPath = new PropertyPath(RowsUpdateTriggerProperty.CoreProperty.PropertyName);
		private static readonly PropertyPath ColsUpdateTriggerPropertyPath = new PropertyPath(ColsUpdateTriggerProperty.CoreProperty.PropertyName);

		#endregion

		private static string ToPattern(this RowDef definition) => definition.ToPattern(
			RowDef.HeightProperty, RowDef.HeightProperty, RowDef.HeightProperty);

		private static string ToPattern(this ColDef definition) => definition.ToPattern(
			ColDef.WidthProperty, ColDef.WidthProperty, ColDef.WidthProperty);

		private static string ToPattern(this BindableObject definition,
			BindableProperty lengthProperty,
			BindableProperty minValueProperty,
			BindableProperty maxValueProperty)
		{
			//var lengthBinding = definition.GetBinding(lengthProperty);
			//var minValueBinding = definition.GetBinding(minValueProperty);
			//var maxValueBinding = definition.GetBinding(maxValueProperty);

			var length = definition.GetValue(lengthProperty).To<GridLength>();
			var minValue = definition.GetValue(minValueProperty);
			var maxValue = definition.GetValue(maxValueProperty);

			var builder = new StringBuilder();

			var isDefaultMinValue = minValue.Is(.0);
			var hasMinValueBinding = false; // minValueBinding.Is(); //UpdateTriggerPropertyPath.Is(minValueBinding?.Path);
			builder.Append(isDefaultMinValue && !hasMinValueBinding ? null : minValue);
			builder.Append(!hasMinValueBinding ? null : "\\");

			var rounded = new GridLength(Math.Round(length.Value * 10d) / 10d, GridUnitType.Absolute);
			var hasLengthBinding = false; //lengthBinding.Is(); //UpdateTriggerPropertyPath.Is(lengthBinding?.Path);
			builder.Append(hasLengthBinding ? rounded.ToString() : null);

			var isDefaultMaxValue = maxValue.Is(double.PositiveInfinity);
			var hasMaxValueBinding = false; //maxValueBinding.Is(); //UpdateTriggerPropertyPath.Is(maxValueBinding?.Path);
			builder.Append(!hasMaxValueBinding ? null : "/");
			builder.Append(isDefaultMaxValue && !hasMaxValueBinding ? null : maxValue);

			return builder.ToString();
		}

		private static void SetValues<TDefinition>(this TDefinition definition,
			string pattern, Grid grid,
			BindableProperty lengthProperty,
			//BindableProperty minValueProperty,
			//BindableProperty maxValueProperty,
			PropertyPath updateTriggerPropertyPath)
			where TDefinition : BindableObject, IDefinition, new()
		{
			var indexMin = pattern.IndexOf(@"\", StringComparison.Ordinal);
			var indexMax = pattern.IndexOf(@"/", StringComparison.Ordinal);
			var hasMinInPattern = indexMin >= 0;
			var hasMaxInPattern = indexMax >= 0;
			//var minValuePattern = hasMinInPattern ? pattern.Substring(0, indexMin) : "";
			//var maxValuePattern = hasMaxInPattern ? pattern.Substring(indexMax + 1, pattern.Length - indexMax - 1) : "";
			var start = hasMinInPattern ? indexMin + 1 : 0;
			var finish = hasMaxInPattern ? indexMax : pattern.Length;
			var lengthPattern = pattern.Substring(start, finish - start);
			var hasLengthInPattern = lengthPattern.IsNullOrWhiteSpace().Not();

			if (hasLengthInPattern) definition.SetValue(lengthProperty, lengthPattern.ToGridLength());
			else definition.RemoveBinding(lengthProperty);
			//if (hasMinInPattern)
			//	definition.SetValue(minValueProperty, minValuePattern.TryParse(out double minValue) ? minValue : .0);
			//else definition.RemoveBinding(minValueProperty);
			//if (hasMaxInPattern)
			//	definition.SetValue(maxValueProperty,
			//		maxValuePattern.TryParse(out double maxValue) ? maxValue : double.PositiveInfinity);
			//else definition.RemoveBinding(maxValueProperty);

			//if (hasLengthInPattern && definition.GetBinding(lengthProperty).IsNot())
			//	Bind(grid, definition, lengthProperty, updateTriggerPropertyPath);
			//if (hasMinInPattern && definition.GetBinding(minValueProperty).IsNot())
			//	Bind(grid, definition, minValueProperty, updateTriggerPropertyPath);
			//if (hasMaxInPattern && definition.GetBinding(maxValueProperty).IsNot())
			//	Bind(grid, definition, maxValueProperty, updateTriggerPropertyPath);
		}

		private static void Bind(Grid grid, DependencyObject definition, DependencyProperty property,
			PropertyPath updateTriggerPropertyPath) =>
			BindingOperations.SetBinding(definition, property, new Binding
			{
				Source = grid,
				Path = updateTriggerPropertyPath,
				Mode = BindingMode.OneWayToSource,
				FallbackValue = definition.GetValue(property)
			});

		private static string GluePatterns(this IEnumerable<string> patterns) => patterns.Aggregate(new StringBuilder(),
			(builder, pattern) => builder.Append(builder.Length.Is(0) ? null : " ").Append(pattern)).ToString();

		private static readonly char[] PatternSplitters = {' ', ','};

		private static string[] SplitPaterns(this string pattern) =>
			pattern?.Split(PatternSplitters, StringSplitOptions.RemoveEmptyEntries);

		private static void UpdateDefinitions<TDefinition>(Grid grid,
			IList<TDefinition> definitions, string pattern,
			BindableProperty lengthProperty,
			//BindableProperty minValueProperty,
			//BindableProperty maxValueProperty,
			DependencyProperty updateTriggerProperty,
			PropertyPath path)
			where TDefinition : BindableObject, IDefinition, new()
		{
			if (grid.GetValue(updateTriggerProperty).Is(true) || pattern.IsNot()) return;

			var patterns = SplitPaterns(pattern);

			definitions.Clear();
			patterns.Select(p =>
			{
				var d = new TDefinition();
				d.SetValues(p, grid, lengthProperty, /*minValueProperty, maxValueProperty,*/ path);
				return d;
			}).ForEach(definitions.Add);
		}

		private static void OnSetChangedCallback(BindableObject element, DependencyPropertyChangedEventArgs args)
		{
			var patterns = args.NewValue.As("").ToUpperInvariant().SplitPaterns();
			var colPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && !p.StartsWith("CS"))?.Replace("C", "");
			var rowPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && !p.StartsWith("RS"))?.Replace("R", "");
			//var sssPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS"))?.Replace("SSS", "");
			var colSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("CS"))?.Replace("CS", "");
			var rowSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("RS"))?.Replace("RS", "");

			//if (sssPattern.TryParse(out bool sharedSizeScope)) Grid.SetIsSharedSizeScope(element, sharedSizeScope);
			if (colSpanPattern.TryParse(out int colSpan)) Grid.SetColumnSpan(element, colSpan);
			if (rowSpanPattern.TryParse(out int rowSpan)) Grid.SetRowSpan(element, rowSpan);
			if (colPattern.TryParse(out int col)) Grid.SetColumn(element, col);
			if (rowPattern.TryParse(out int row)) Grid.SetRow(element, row);
		}

		private static GridLength ToGridLength(this string pattern)
		{
			var unitType = pattern.Contains("*") ? GridUnitType.Star : GridUnitType.Absolute;
			pattern = unitType.Is(GridUnitType.Star) ? pattern.Replace("*", "") : pattern;
			pattern = unitType.Is(GridUnitType.Star) && pattern.IsNullOrWhiteSpace() ? "1" : pattern;
			return pattern.TryParse(out double value) ? new GridLength(value, unitType) : new GridLength();
		}
	}
}