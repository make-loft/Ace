using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using RowDef = System.Windows.Controls.RowDefinition;
using ColDef = System.Windows.Controls.ColumnDefinition;

namespace Ace.Markup
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

		private static PropertyMetadata GetMetadata<T>(Action<T, DependencyPropertyChangedEventArgs> action) =>
			new PropertyMetadata((o, args) =>
			{
				if (o.Is(out T sender)) action(sender, args);
			});

		public static readonly DependencyProperty ShowLinesProperty = DependencyProperty.RegisterAttached(
			"ShowLines", typeof(bool), typeof(Rack), GetMetadata<Grid>((grid, args) =>
				grid.ShowGridLines = args.NewValue.Is(true)));

		public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
			"Rows", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(grid,
				grid.RowDefinitions, args.NewValue?.ToString(),
				RowDef.HeightProperty, RowDef.MinHeightProperty, RowDef.MaxHeightProperty)));

		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
			"Columns", typeof(string), typeof(Rack), GetMetadata<Grid>((grid, args) => UpdateDefinitions(grid,
				grid.ColumnDefinitions, args.NewValue?.ToString(),
				ColDef.WidthProperty, ColDef.MinWidthProperty, ColDef.MaxWidthProperty)));

		public static readonly DependencyProperty SetProperty = DependencyProperty.RegisterAttached(
			"Set", typeof(string), typeof(Rack), GetMetadata<UIElement>(OnSetChangedCallback));

		public static readonly DependencyProperty UpdateTriggerProperty = DependencyProperty.RegisterAttached(
			"UpdateTrigger", typeof(object), typeof(Rack), GetMetadata<Grid>((grid, args) => 
			{
				var newColumnsPattern = grid.ColumnDefinitions.Select(ToPattern).GluePatterns();
				var newRowsPattern = grid.RowDefinitions.Select(ToPattern).GluePatterns();
				var oldColumnsPattern = grid.GetValue(ColumnsProperty);
				var oldRowsPattern = grid.GetValue(RowsProperty);
				
				if (newColumnsPattern.IsNot(oldColumnsPattern))
					grid.SetValue(ColumnsProperty, newColumnsPattern);
				if (newRowsPattern.IsNot(oldRowsPattern))
					grid.SetValue(RowsProperty, newRowsPattern);
			}));

		private static readonly PropertyPath UpdateTriggerPropertyPath = new PropertyPath(UpdateTriggerProperty);

		#endregion

		private static string ToPattern(this RowDef definition) => definition.ToPattern(
			RowDef.HeightProperty, RowDef.MinHeightProperty, RowDef.MaxHeightProperty);

		private static string ToPattern(this ColDef definition) => definition.ToPattern(
			ColDef.WidthProperty, ColDef.MinWidthProperty, ColDef.MaxWidthProperty);
		
		private static string ToPattern(this DefinitionBase definition,
			DependencyProperty lengthProperty,
			DependencyProperty minValueProperty,
			DependencyProperty maxValueProperty)
		{
			//var lengthBinding = BindingOperations.GetBinding(definition, lengthProperty);
			var minValueBinding = BindingOperations.GetBinding(definition, minValueProperty);
			var maxValueBinding = BindingOperations.GetBinding(definition, maxValueProperty);

			var length = definition.GetValue(lengthProperty).To<GridLength>();
			var minValue = definition.GetValue(minValueProperty);
			var maxValue = definition.GetValue(maxValueProperty);

			var builder = new StringBuilder();

			var isDefaultMinValue = minValue.Is(.0);
			var hasMinValueBinding = UpdateTriggerPropertyPath.Is(minValueBinding?.Path);
			builder.Append(isDefaultMinValue && !hasMinValueBinding ? null : minValue);
			builder.Append(hasMinValueBinding ? "MIN" : null);
			builder.Append(isDefaultMinValue ? null : "\\");

			//var rounded = new GridLength(Math.Round(length.Value*10)/10, length.GridUnitType);
			//var hasLengthBinding = lengthBinding?.Path == UpdateTriggerPropertyPath;
			builder.Append(length);
			//builder.Append(hasLengthBinding ? "LEN" : null);

			var isDefaultMaxValue = maxValue.Is(double.PositiveInfinity);
			var hasMaxValueBinding = UpdateTriggerPropertyPath.Is(maxValueBinding?.Path);
			builder.Append(isDefaultMaxValue ? null : "/");
			builder.Append(isDefaultMaxValue && !hasMaxValueBinding ? null : minValue);
			builder.Append(hasMaxValueBinding ? "MAX" : null);

			return builder.ToString();
		}

		private static TDefinition ToDefinition<TDefinition>(this string pattern, Grid grid,
			DependencyProperty lengthProperty,
			DependencyProperty minValueProperty,
			DependencyProperty maxValueProperty)
			where TDefinition : DefinitionBase, new()
		{
			var indexMin = pattern.IndexOf(@"\", StringComparison.Ordinal);
			var indexMax = pattern.IndexOf(@"/", StringComparison.Ordinal);
			var hasMin = indexMin >= 0;
			var hasMax = indexMax >= 0;
			var minValuePattern = hasMin ? pattern.Substring(0, indexMin) : "";
			var maxValuePattern = hasMax ? pattern.Substring(indexMax + 1, pattern.Length - indexMax - 1) : "";
			var start = hasMin ? indexMin + 1 : 0;
			var finish = hasMax ? indexMax : pattern.Length;
			var lengthPattern = pattern.Substring(start, finish - start);
			var bindMinValue = minValuePattern.EndsWith("MIN", StringComparison.OrdinalIgnoreCase);
			var bindMaxValue = maxValuePattern.EndsWith("MAX", StringComparison.OrdinalIgnoreCase);
			//var bindLengthValue = lengthPattern.EndsWith("LEN", StringComparison.OrdinalIgnoreCase);

			lengthPattern = lengthPattern.Trim("LEN len".ToCharArray());
			minValuePattern = minValuePattern.Trim("MIN min".ToCharArray());
			maxValuePattern = maxValuePattern.Trim("MAX max".ToCharArray());

			var definition = new TDefinition();
			definition.SetValue(lengthProperty, lengthPattern.ToGridLength());
			if (hasMin)
			definition.SetValue(minValueProperty, minValuePattern.TryParse(out double minValue) ? minValue : .0);
			if (hasMax)
			definition.SetValue(maxValueProperty, maxValuePattern.TryParse(out double maxValue) ? maxValue : double.PositiveInfinity);

			//if (bindLengthValue)
				Bind(grid, definition, lengthProperty);
			//if (bindMinValue) 
				//Bind(grid, definition, minValueProperty);
			//if (bindMaxValue) 
				//Bind(grid, definition, maxValueProperty);

			return definition;
		}

		private static void Bind(Grid grid, DefinitionBase definition, DependencyProperty property) =>
			BindingOperations.SetBinding(definition, property, new Binding
			{
				Source = grid,
				Path = UpdateTriggerPropertyPath,
				Mode = BindingMode.OneWayToSource,
				FallbackValue = definition.GetValue(property)
			});

		private static string GluePatterns(this IEnumerable<string> patterns) => patterns.Aggregate(new StringBuilder(),
			(builder, pattern) => builder.Append(builder.Length.Is(0) ? null : " ").Append(pattern)).ToString();

		private static string[] SplitPaterns(string value) =>
			value?.Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries);

		private static void UpdateDefinitions<T>(Grid grid, ICollection<T> definitionCollection, string pattern,
			DependencyProperty lengthProperty,
			DependencyProperty minValueProperty,
			DependencyProperty maxValueProperty)
			where T : DefinitionBase, new()
		{
			definitionCollection.Clear();
			var patterns = SplitPaterns(pattern);
			var definitions = patterns.Select(p =>
				p.ToDefinition<T>(grid, lengthProperty, minValueProperty, maxValueProperty)).ToList();
			var sourceDefinitions = definitionCollection.ToArray();
			if (definitionCollection.Count.Is(definitions.Count))
			{
				for (var i = 0; i < definitions.Count; i++)
				{
					var source = definitions[i];
					var target = sourceDefinitions[i];
					lengthProperty.Assign(source, target);
					minValueProperty.Assign(source, target);
					maxValueProperty.Assign(source, target);
				}
			}
			else
			{
				definitionCollection.Clear();
				definitions.ForEach(definitionCollection.Add);
			}
		}

		private static void Assign(this DependencyProperty dependencyProperty,
			DependencyObject source, DependencyObject target)
		{
			var newValue = source.GetValue(dependencyProperty);
			var oldValue = target.GetValue(dependencyProperty);
			if (newValue.Is(oldValue)) return;
			target.SetValue(dependencyProperty, newValue);
		}

		private static void OnSetChangedCallback(UIElement element, DependencyPropertyChangedEventArgs args)
		{
			var patterns = SplitPaterns((args.NewValue as string ?? "").ToUpperInvariant());
			var colPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && !p.StartsWith("CS"))?.Replace("C", "");
			var rowPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && !p.StartsWith("RS"))?.Replace("R", "");
			var sssPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS"))?.Replace("SSS", "");
			var colSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("CS"))?.Replace("CS", "");
			var rowSpanPattern = patterns.FirstOrDefault(p => p.StartsWith("RS"))?.Replace("RS", "");

			if (sssPattern.TryParse(out bool sharedSizeScope)) Grid.SetIsSharedSizeScope(element, sharedSizeScope);
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