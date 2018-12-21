using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if XAMARIN
using Xamarin.Forms;
using FrameworkElement = Xamarin.Forms.BindableObject;
using DependencyObject = Xamarin.Forms.BindableObject;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using DependencyPropertyChangedEventArgs = System.Windows.DependencyPropertyChangedEventArgs;
using PropertyMetadata = System.Windows.PropertyMetadata;
using PropertyPath = System.Windows.PropertyPath;
using Binding = System.Windows.Data.Binding;
using static System.Windows.BindablePropertyExtensions;
using static Xamarin.Forms.RowDefinition;
using static Xamarin.Forms.ColumnDefinition;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static System.Windows.DependencyProperty;
using static System.Windows.Controls.RowDefinition;
using static System.Windows.Controls.ColumnDefinition;
#endif

namespace Ace.Markup
{
	public static class Rack
	{
		#region Declarations

		public static void SetRow(DependencyObject o, int value) => o.SetValue(Grid.RowProperty, value);
		public static void SetColumn(DependencyObject o, int value) => o.SetValue(Grid.ColumnProperty, value);
		public static void SetRows(DependencyObject o, string value) => o.SetValue(RowsProperty, value);
		public static void SetColumns(DependencyObject o, string value) => o.SetValue(ColumnsProperty, value);
		public static void SetCell(DependencyObject o, string value) => o.SetValue(CellProperty, value);
		public static void SetShowLines(DependencyObject o, bool value) => o.SetValue(ShowLinesProperty, value);
		public static void SetIsTwoWayMode(DependencyObject o, bool value) => o.SetValue(IsTwoWayModeProperty, value);

		public static int GetRow(DependencyObject o) => o.GetValue(Grid.RowProperty).To<int>();
		public static int GetColumn(DependencyObject o) => o.GetValue(Grid.ColumnProperty).To<int>();
		public static string GetRows(DependencyObject o) => o.GetValue(RowsProperty).To<string>();
		public static string GetColumns(DependencyObject o) => o.GetValue(ColumnsProperty).To<string>();
		public static string GetCell(DependencyObject o) => o.GetValue(CellProperty).To<string>();
		public static bool GetShowLines(DependencyObject o) => o.GetValue(ShowLinesProperty).To<bool>();
		public static bool GetIsTwoWayMode(DependencyObject o) => o.GetValue(IsTwoWayModeProperty).To<bool>();

		private static PropertyMetadata GetMetadata<T>(Action<T, DependencyPropertyChangedEventArgs> action)
			where T : DependencyObject =>
			new PropertyMetadata((sender, args) =>
			{
				if (args.NewValue.Is(args.OldValue)) return;
				if (sender.Is(out T typedSender)) action(typedSender, args);
			});

		public static readonly DependencyProperty ShowLinesProperty = RegisterAttached(
			"ShowLines", typeof(bool), typeof(Rack), GetMetadata<Grid>((grid, args) =>
				grid.SetShowGridLines(args.NewValue.Is(true))));

		public static readonly DependencyProperty RowsProperty = RegisterAttached(
			"Rows", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(
				grid, grid.RowDefinitions, args.NewValue?.ToString(),
				HeightProperty, MinHeightProperty, MaxHeightProperty,
				RowsIsInUpdateProperty, RowsUpdateTriggerPropertyPath)));

		public static readonly DependencyProperty ColumnsProperty = RegisterAttached(
			"Columns", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(
				grid, grid.ColumnDefinitions, args.NewValue?.ToString(),
				WidthProperty, MinWidthProperty, MaxWidthProperty,
				ColsIsInUpdateProperty, ColsUpdateTriggerPropertyPath)));

		public static readonly DependencyProperty CellProperty = RegisterAttached(
			"Cell", typeof(string), typeof(Rack), GetMetadata<FrameworkElement>(OnSetChangedCallback));

		private static readonly DependencyProperty RowsIsInUpdateProperty = RegisterAttached(
			"RowsIsInUpdate", typeof(object), typeof(Rack), default);

		private static readonly DependencyProperty ColsIsInUpdateProperty = RegisterAttached(
			"ColsIsInUpdate", typeof(object), typeof(Rack), default);

		private static readonly DependencyProperty RowsUpdateTriggerProperty = RegisterAttached(
			"RowsUpdateTrigger", typeof(object), typeof(Rack), GetMetadata<Grid>((grid, args) =>
			{
				grid.SetValue(RowsIsInUpdateProperty, true);

				var newRowsPattern = grid.RowDefinitions.Select(ToPattern).Glue();
				var oldRowsPattern = grid.GetValue(RowsProperty).To<string>();

				if (newRowsPattern.IsNot(oldRowsPattern))
					grid.SetValue(RowsProperty, newRowsPattern);

				grid.SetValue(RowsIsInUpdateProperty, false);
			}));

		private static readonly DependencyProperty ColsUpdateTriggerProperty = RegisterAttached(
			"ColsUpdateTrigger", typeof(object), typeof(Rack), GetMetadata<Grid>((grid, args) =>
			{
				grid.SetValue(ColsIsInUpdateProperty, true);

				var newColsPattern = grid.ColumnDefinitions.Select(ToPattern).Glue();
				var oldColsPattern = grid.GetValue(ColumnsProperty).To<string>();

				if (newColsPattern.IsNot(oldColsPattern))
					grid.SetValue(ColumnsProperty, newColsPattern);

				grid.SetValue(ColsIsInUpdateProperty, false);
			}));

		public static readonly DependencyProperty IsTwoWayModeProperty = RegisterAttached(
			"IsTwoWayMode", typeof(bool), typeof(Rack), GetMetadata<Grid>((grid, args) =>
			{
				SetRows(grid, GetRows(grid));
				SetColumns(grid, GetColumns(grid));
			}));

		private static readonly PropertyPath
			RowsUpdateTriggerPropertyPath = new PropertyPath(RowsUpdateTriggerProperty);

		private static readonly PropertyPath
			ColsUpdateTriggerPropertyPath = new PropertyPath(ColsUpdateTriggerProperty);

		#endregion

		private static string ToPattern(this RowDefinition definition) => definition.ToPattern(
			HeightProperty, MinHeightProperty, MaxHeightProperty);

		private static string ToPattern(this ColumnDefinition definition) => definition.ToPattern(
			WidthProperty, MinWidthProperty, MaxWidthProperty);

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
			var hasMinValueBinding = minValueBinding.Is();
			builder.Append(isDefaultMinValue && !hasMinValueBinding ? null : minValue);
			builder.Append(!hasMinValueBinding ? null : "\\");

			var rounded = new GridLength(Math.Round(length.Value * 10d) / 10d, length.GridUnitType);
			var hasLengthBinding = lengthBinding.Is();
			builder.Append(hasLengthBinding ? rounded.ToString() : null);

			var isDefaultMaxValue = maxValue.Is(double.PositiveInfinity);
			var hasMaxValueBinding = maxValueBinding.Is();
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

			if (GetIsTwoWayMode(grid).IsNot(true)) return;

			if (hasLengthInPattern && definition.GetBinding(lengthProperty).IsNot())
				Bind(grid, definition, lengthProperty, updateTriggerPropertyPath);
			if (hasMinInPattern && definition.GetBinding(minValueProperty).IsNot())
				Bind(grid, definition, minValueProperty, updateTriggerPropertyPath);
			if (hasMaxInPattern && definition.GetBinding(maxValueProperty).IsNot())
				Bind(grid, definition, maxValueProperty, updateTriggerPropertyPath);
		}

		private static void Bind(Grid grid, DependencyObject definition, DependencyProperty property,
			PropertyPath updateTriggerPropertyPath) =>
			definition.SetBinding(property, new Binding
			{
				Source = grid,
#if !WINDOWS_PHONE
				Path = updateTriggerPropertyPath,
				Mode = BindingMode.OneWayToSource,
#endif
				FallbackValue = definition.GetValue(property)
			});

		private static void UpdateDefinitions<TDefinition>(Grid grid,
			ICollection<TDefinition> definitions, string pattern,
			DependencyProperty lengthProperty,
			DependencyProperty minValueProperty,
			DependencyProperty maxValueProperty,
			DependencyProperty updateTriggerProperty,
			PropertyPath path)
			where TDefinition : DependencyObject, new()
		{
			if (grid.GetValue(updateTriggerProperty).Is(true) || pattern.IsNot()) return;

			var patterns = pattern.Separate();

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
			var patterns = args.NewValue.As("").ToUpperInvariant().Separate();
			var colPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && !p.StartsWith("CS"))?.Replace("C", "");
			var rowPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && !p.StartsWith("RS"))?.Replace("R", "");
			var sssPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS"))?.Replace("SSS", "");
			var colSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("CS"))?.Replace("CS", "");
			var rowSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("RS"))?.Replace("RS", "");
#if !WINDOWS_PHONE && !XAMARIN
			if (sssPattern.TryParse(out bool sharedSizeScope)) Grid.SetIsSharedSizeScope(element, sharedSizeScope);
#endif
			if (colSpanPattern.TryParse(out int colSpan)) Grid.SetColumnSpan(element, colSpan);
			if (rowSpanPattern.TryParse(out int rowSpan)) Grid.SetRowSpan(element, rowSpan);
			if (colPattern.TryParse(out int col)) Grid.SetColumn(element, col);
			if (rowPattern.TryParse(out int row)) Grid.SetRow(element, row);
		}

		#region Markup

		public static string ActiveSplitter { get; set; } = " ";
		public static string[] AllowedSplitters { get; set; } = {" ", ","};

		private static string[] Separate(this string pattern) =>
			pattern?.Split(AllowedSplitters, StringSplitOptions.RemoveEmptyEntries);

		private static string Glue(this IEnumerable<string> patterns) =>
			patterns.Aggregate(new StringBuilder(), (builder, pattern) =>
				builder.Append(builder.Length.Is(0) ? null : ActiveSplitter).Append(pattern)).ToString();

		private static GridLength ToGridLength(this string pattern)
		{
			var unitType = pattern.Contains("*") ? Star : Pixel;
			pattern = unitType.Is(Star) ? pattern.Replace("*", "") : pattern;
			pattern = unitType.Is(Star) && pattern.IsNullOrWhiteSpace() ? "1" : pattern;
			return pattern.TryParse(out double value) ? new GridLength(value, unitType) : new GridLength();
		}

		private static readonly GridUnitType Star = GridUnitType.Star;
#if XAMARIN
		private static readonly GridUnitType Pixel = GridUnitType.Absolute;
		private static readonly DependencyProperty MinWidthProperty = WidthProperty;
		private static readonly DependencyProperty MaxWidthProperty = WidthProperty;
		private static readonly DependencyProperty MinHeightProperty = HeightProperty;
		private static readonly DependencyProperty MaxHeightProperty = HeightProperty;
		private static void SetShowGridLines(this Grid grid, bool value) { }
#else
		private static readonly GridUnitType Pixel = GridUnitType.Pixel;
		private static void SetShowGridLines(this Grid grid, bool value) => grid.ShowGridLines = value;
#endif

		#endregion
	}
}