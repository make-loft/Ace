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
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static System.Windows.DependencyProperty;
#endif

namespace Ace.Controls
{
	public struct DefinitionProperties
	{
		public static DefinitionProperties Rows = new()
		{
#if !XAMARIN
			Min = RowDefinition.MinHeightProperty,
			Max = RowDefinition.MaxHeightProperty,
#endif
			Length = RowDefinition.HeightProperty,
		};

		public static DefinitionProperties Cols = new()
		{
#if !XAMARIN
			Min = ColumnDefinition.MinWidthProperty,
			Max = ColumnDefinition.MinWidthProperty,
#endif
			Length = ColumnDefinition.WidthProperty,
		};

		public DependencyProperty Length { get; set; }
		public DependencyProperty Min { get; set; }
		public DependencyProperty Max { get; set; }
	}

	public partial class Rack : Grid
	{
		public string Rows
		{
			get => GetValue(RowsProperty).To<string>();
			set => SetValue(RowsProperty, value);
		}

		public string Columns
		{
			get => GetValue(ColumnsProperty).To<string>();
			set => SetValue(ColumnsProperty, value);
		}

		#region Declarations

		public static string GetCell(DependencyObject o) => o.GetValue(CellProperty).To<string>();
		public static string GetRows(DependencyObject o) => o.GetValue(RowsProperty).To<string>();
		public static string GetColumns(DependencyObject o) => o.GetValue(ColumnsProperty).To<string>();
		public static bool GetShowLines(DependencyObject o) => o.GetValue(ShowLinesProperty).To<bool>();
		public static bool GetIsTwoWayMode(DependencyObject o) => o.GetValue(IsTwoWayModeProperty).To<bool>();

		public static void SetCell(DependencyObject o, string value) => o.SetValue(CellProperty, value);
		public static void SetRows(DependencyObject o, string value) => o.SetValue(RowsProperty, value);
		public static void SetColumns(DependencyObject o, string value) => o.SetValue(ColumnsProperty, value);
		public static void SetShowLines(DependencyObject o, bool value) => o.SetValue(ShowLinesProperty, value);
		public static void SetIsTwoWayMode(DependencyObject o, bool value) => o.SetValue(IsTwoWayModeProperty, value);

		private static PropertyMetadata GetMetadata<T>(Action<T, DependencyPropertyChangedEventArgs> action)
			where T : DependencyObject =>
			new((sender, args) =>
			{
				if (args.NewValue.Is(args.OldValue)) return;
				if (sender.Is(out T typedSender)) action(typedSender, args);
			});

		public static readonly DependencyProperty ShowLinesProperty = RegisterAttached(
			"ShowLines", typeof(bool), typeof(Rack), GetMetadata<Grid>((grid, args) =>
				SetShowGridLines(grid, args.NewValue.Is(true))));

		public static readonly DependencyProperty RowsProperty = RegisterAttached(
			"Rows", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(
				grid, grid.RowDefinitions, args.NewValue?.ToString(),
				DefinitionProperties.Rows,
				RowsIsInUpdateProperty, RowsUpdateTriggerPropertyPath)));

		public static readonly DependencyProperty ColumnsProperty = RegisterAttached(
			"Columns", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(
				grid, grid.ColumnDefinitions, args.NewValue?.ToString(),
				DefinitionProperties.Cols,
				ColsIsInUpdateProperty, ColsUpdateTriggerPropertyPath)));

		public static readonly DependencyProperty CellProperty = RegisterAttached(
			"Cell", typeof(string), typeof(Rack), GetMetadata<FrameworkElement>(OnCellChanged));

		private static readonly DependencyProperty RowsIsInUpdateProperty = RegisterAttached(
			"RowsIsInUpdate", typeof(object), typeof(Rack), default);

		private static readonly DependencyProperty ColsIsInUpdateProperty = RegisterAttached(
			"ColsIsInUpdate", typeof(object), typeof(Rack), default);

		private static readonly DependencyProperty RowsUpdateTriggerProperty = RegisterAttached(
			"RowsUpdateTrigger", typeof(object), typeof(Rack), GetMetadata<Grid>((grid, args) =>
			{
				if (grid.GetValue(RowsIsInUpdateProperty).Is(True)) return;

				var newRowsPattern = Glue(grid.RowDefinitions.Select(ToPattern));
				var oldRowsPattern = grid.GetValue(RowsProperty).To<string>();
				if (newRowsPattern.Is(oldRowsPattern)) return;

				grid.SetValue(RowsIsInUpdateProperty, True);
				grid.SetValue(RowsProperty, newRowsPattern);
				grid.SetValue(RowsIsInUpdateProperty, False);
			}));

		private static readonly DependencyProperty ColsUpdateTriggerProperty = RegisterAttached(
			"ColsUpdateTrigger", typeof(object), typeof(Rack), GetMetadata<Grid>((grid, args) =>
			{
				if (grid.GetValue(ColsIsInUpdateProperty).Is(True)) return;

				var newColsPattern = Glue(grid.ColumnDefinitions.Select(ToPattern));
				var oldColsPattern = grid.GetValue(ColumnsProperty).To<string>();
				if (newColsPattern.Is(oldColsPattern)) return;

				grid.SetValue(ColsIsInUpdateProperty, True);
				grid.SetValue(ColumnsProperty, newColsPattern);
				grid.SetValue(ColsIsInUpdateProperty, False);
			}));

		public static readonly DependencyProperty IsTwoWayModeProperty = RegisterAttached(
			"IsTwoWayMode", typeof(bool), typeof(Rack), GetMetadata<Grid>((grid, args) =>
			{
				UpdateDefinitions(
					grid, grid.RowDefinitions, GetRows(grid),
					DefinitionProperties.Rows,
					RowsIsInUpdateProperty, RowsUpdateTriggerPropertyPath);

				UpdateDefinitions(
					grid, grid.ColumnDefinitions, GetColumns(grid),
					DefinitionProperties.Cols,
					ColsIsInUpdateProperty, ColsUpdateTriggerPropertyPath);
			}));

		private static readonly PropertyPath RowsUpdateTriggerPropertyPath = new(RowsUpdateTriggerProperty);
		private static readonly PropertyPath ColsUpdateTriggerPropertyPath = new(ColsUpdateTriggerProperty);

		#endregion

		private static string ToPattern(RowDefinition definition) => ToPattern(definition, DefinitionProperties.Rows);

		private static string ToPattern(ColumnDefinition definition) => ToPattern(definition, DefinitionProperties.Cols);

		private static string ToPattern(DependencyObject definition, DefinitionProperties properties)
		{
			var lengthBinding = definition.GetBinding(properties.Length);
			var minValueBinding = definition.GetBinding(properties.Min);
			var maxValueBinding = definition.GetBinding(properties.Max);

			var length = definition.GetValue(properties.Length).To<GridLength>();
			var minValue = definition.GetValue(properties.Min);
			var maxValue = definition.GetValue(properties.Max);

			var builder = new StringBuilder();

			var isDefaultMinValue = minValue.Is(.0);
			var hasMinValueBinding = minValueBinding.Is();
			builder.Append(isDefaultMinValue && hasMinValueBinding.Not() ? null : minValue);
			builder.Append(hasMinValueBinding ? "\\" : null);

			var rounded = new GridLength(Math.Round(length.Value * 10d) / 10d, length.GridUnitType);
			var hasLengthBinding = lengthBinding.Is();
			builder.Append(hasLengthBinding ? rounded.ToString().Replace("Auto", AutoKeyword) : null);

			var isDefaultMaxValue = maxValue.Is(double.PositiveInfinity);
			var hasMaxValueBinding = maxValueBinding.Is();
			builder.Append(hasMaxValueBinding ? "/" : null);
			builder.Append(isDefaultMaxValue && hasMaxValueBinding.Not() ? null : maxValue);

			return builder.ToString();
		}

		private static void SetValues<TDefinition>(TDefinition definition,
			string pattern, Grid grid,
			DefinitionProperties properties,
			PropertyPath updateTriggerPropertyPath)
			where TDefinition : DependencyObject, new()
		{
			var indexMin = pattern.IndexOf(@"\", StringComparison.Ordinal);
			var indexMax = pattern.IndexOf(@"/", StringComparison.Ordinal);
			var hasMinInPattern = indexMin >= 0;
			var hasMaxInPattern = indexMax >= 0;
			var minPattern = hasMinInPattern ? pattern.Substring(0, indexMin) : "";
			var maxPattern = hasMaxInPattern ? pattern.Substring(indexMax + 1, pattern.Length - indexMax - 1) : "";
			var start = hasMinInPattern ? indexMin + 1 : 0;
			var finish = hasMaxInPattern ? indexMax : pattern.Length;
			var lengthPattern = pattern.Substring(start, finish - start);
			var hasLengthInPattern = lengthPattern.IsNullOrWhiteSpace().Not();

			if (hasLengthInPattern)
				definition.SetValue(properties.Length, ToGridLength(lengthPattern));
			
			if (hasMinInPattern)
				definition.SetValue(properties.Min, minPattern.TryParse(out double minValue) ? minValue : double.NegativeInfinity);
			
			if (hasMaxInPattern)
				definition.SetValue(properties.Max, maxPattern.TryParse(out double maxValue) ? maxValue : double.PositiveInfinity);

			if (GetIsTwoWayMode(grid).IsNot(True)) return;

			if (hasLengthInPattern)
				Bind(grid, definition, properties.Length, updateTriggerPropertyPath);
			if (hasMinInPattern)
				Bind(grid, definition, properties.Min, updateTriggerPropertyPath);
			if (hasMaxInPattern)
				Bind(grid, definition, properties.Max, updateTriggerPropertyPath);
		}

		private static void Bind(Grid grid, DependencyObject definition, DependencyProperty property,
			PropertyPath updateTriggerPropertyPath) =>
			definition.SetBinding(property, new Binding
			{
				Source = grid,
				Path = updateTriggerPropertyPath,
				Mode = BindingMode.OneWayToSource,
				FallbackValue = definition.GetValue(property)
			});

		private static void UpdateDefinitions<TDefinition>(Grid grid,
			ICollection<TDefinition> definitions, string pattern,
			DefinitionProperties properties,
			DependencyProperty isInUpdateProperty,
			PropertyPath path)
			where TDefinition : DependencyObject, new()
		{
			if (grid.GetValue(isInUpdateProperty).Is(True) || pattern.IsNot()) return;
			
			grid.SetValue(isInUpdateProperty, True);
			
			var patterns = Separate(pattern);
			
			definitions.Clear();
			patterns.Select(p =>
			{
				var d = new TDefinition();

				try
				{
					SetValues(d, p, grid, properties, path);
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
				}

				return d;
			}).ForEach(definitions.Add);

			grid.SetValue(isInUpdateProperty, False);
		}

		private static void OnCellChanged(FrameworkElement element, DependencyPropertyChangedEventArgs args)
		{
			var patterns = Separate(args.NewValue.As("").ToUpperInvariant());
			var colPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && p.StartsWith("CS").Not())?.Replace("C", "");
			var rowPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && p.StartsWith("RS").Not())?.Replace("R", "");
			var sssPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS"))?.Replace("SSS", "").TrimStart(TrimStartChars);
			var colSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("CS"))?.Replace("CS", "").TrimStart(TrimStartChars);
			var rowSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("RS"))?.Replace("RS", "").TrimStart(TrimStartChars);
#if !XAMARIN
			if (sssPattern.TryParse(out bool sharedSizeScope)) SetIsSharedSizeScope(element, sharedSizeScope);
#endif
			if (colSpanPattern.TryParse(out int colSpan)) SetColumnSpan(element, AdaptSpan(colSpan));
			if (rowSpanPattern.TryParse(out int rowSpan)) SetRowSpan(element, AdaptSpan(rowSpan));
			if (colPattern.TryParse(out int col)) SetColumn(element, AdaptIndex(col, colSpan));
			if (rowPattern.TryParse(out int row)) SetRow(element, AdaptIndex(row, rowSpan));
		}

		public static char[] TrimStartChars = ":=".ToCharArray();

		private static int AdaptIndex(int index, int span) => span < 0 && index + span > 0 ? index + span : index;
		private static int AdaptSpan(int span) =>
			span > 0 ? +span :
			span < 0 ? -span :
			int.MaxValue;

#region Markup

		public static string AutoKeyword { get; set; } = "^";
		public static string ActiveSplitter { get; set; } = " ";
		public static string[] AllowedSplitters { get; set; } = {" ", ","};

		private static string[] Separate(string pattern) =>
			pattern?.Split(AllowedSplitters, StringSplitOptions.RemoveEmptyEntries);

		private static string Glue(IEnumerable<string> patterns) =>
			patterns.Aggregate(new StringBuilder(), (builder, pattern) =>
				builder.Append(builder.Length.Is(0) ? null : ActiveSplitter).Append(pattern)).ToString();

		private static GridLength ToGridLength(string pattern)
		{
			var unitType = pattern.Contains("*") ? Star : Pixel;
			pattern = unitType.Is(Star) ? pattern.Replace("*", "") : pattern;
			pattern = unitType.Is(Star) && pattern.IsNullOrWhiteSpace() ? "1" : pattern;
			return pattern.TryParse(out double value)
				? new GridLength(value, unitType)
				: new GridLength(0d, Auto);
		}
		
		private static readonly object True = true;
		private static readonly object False = false;

		private const GridUnitType Auto = GridUnitType.Auto;
		private const GridUnitType Star = GridUnitType.Star;
#if XAMARIN
		private const GridUnitType Pixel = GridUnitType.Absolute;
		private static void SetShowGridLines(Grid grid, bool value) { }
#else
		private const GridUnitType Pixel = GridUnitType.Pixel;
		private static void SetShowGridLines(Grid grid, bool value) => grid.ShowGridLines = value;
#endif

#endregion
	}
}