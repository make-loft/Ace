using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

		public static readonly DependencyProperty ShowLinesProperty = DependencyProperty.RegisterAttached(
			"ShowLines", typeof(bool), typeof(Rack), new PropertyMetadata(default(bool), (o, args) =>
			{
				if (o.Is(out Grid grid).Not() || args.NewValue.IsNull()) return;
				
				grid.ShowGridLines = Equals(args.NewValue, true);
			}));

		public static readonly DependencyProperty RowsProperty = DependencyProperty.RegisterAttached(
			"Rows", typeof(string), typeof(Rack), new PropertyMetadata((o, args) =>
			{
				if (o.Is(out Grid grid).Not() || args.NewValue.IsNull()) return;
				if (grid.GetValue(UpdateTriggerProperty).Is(DependencyProperty.UnsetValue)) return;

				UpdateDefinitions(grid, grid.RowDefinitions, args.NewValue.ToString(),
					RowDefinition.HeightProperty, RowDefinition.MinHeightProperty, RowDefinition.MaxHeightProperty);
			}));

		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached(
			"Columns", typeof(string), typeof(Rack), new PropertyMetadata((o, args) =>
			{
				if (o.Is(out Grid grid).Not() || args.NewValue.IsNull()) return;
				if (grid.GetValue(UpdateTriggerProperty).Is(DependencyProperty.UnsetValue)) return;

				UpdateDefinitions(grid, grid.ColumnDefinitions, args.NewValue.ToString(),
					ColumnDefinition.WidthProperty, ColumnDefinition.MinWidthProperty,
					ColumnDefinition.MaxWidthProperty);
			}));

		public static readonly DependencyProperty SetProperty = DependencyProperty.RegisterAttached(
			"Set", typeof(string), typeof(Rack), new PropertyMetadata("", OnSetChangedCallback));

		public static readonly DependencyProperty UpdateTriggerProperty = DependencyProperty.RegisterAttached(
			"UpdateTrigger", typeof(object), typeof(Rack), new PropertyMetadata(default(object), (o, e) =>
			{
				if (o.Is(out Grid grid).Not()) return;

				var columnsPattern = grid.ColumnDefinitions.Select(ToPattern).GluePatterns();
				var rowsPattern = grid.RowDefinitions.Select(ToPattern).GluePatterns();
				grid.SetValue(ColumnsProperty, columnsPattern);
				grid.SetValue(RowsProperty, rowsPattern);
				grid.SetValue(UpdateTriggerProperty, null);
			}));

		public static readonly PropertyPath UpdateTriggerPropertyPath = new PropertyPath(UpdateTriggerProperty);

		#endregion

		private static string ToPattern(this RowDefinition definition) => definition.ToPattern(
			RowDefinition.HeightProperty, RowDefinition.MinHeightProperty, RowDefinition.MaxHeightProperty);

		private static string ToPattern(this ColumnDefinition definition) => definition.ToPattern(
			ColumnDefinition.WidthProperty, ColumnDefinition.MinWidthProperty, ColumnDefinition.MaxWidthProperty);
		
		private static string ToPattern(this DefinitionBase definition,
			DependencyProperty lengthProperty,
			DependencyProperty minValueProperty,
			DependencyProperty maxValueProperty)
		{
			//var lengthBinding = BindingOperations.GetBinding(definition, lengthProperty);
			var minValueBinding = BindingOperations.GetBinding(definition, minValueProperty);
			var maxValueBinding = BindingOperations.GetBinding(definition, maxValueProperty);

			var length = definition.GetValue(lengthProperty);
			var minValue = definition.GetValue(minValueProperty);
			var maxValue = definition.GetValue(maxValueProperty);

			var builder = new StringBuilder();

			var isDefaultMinValue = Equals(minValue, .0);
			var hasMinValueBinding = minValueBinding?.Path == UpdateTriggerPropertyPath;
			builder.Append(isDefaultMinValue && !hasMinValueBinding ? null : minValue);
			builder.Append(hasMinValueBinding ? "MIN" : null);
			builder.Append(isDefaultMinValue ? null : "\\");

			//var hasLengthBinding = lengthBinding?.Path == UpdateTriggerPropertyPath;
			builder.Append(length);
			//builder.Append(hasLengthBinding ? "LEN" : null);

			var isDefaultMaxValue = Equals(maxValue, double.PositiveInfinity);
			var hasMaxValueBinding = maxValueBinding?.Path == UpdateTriggerPropertyPath;
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
			definition.SetValue(minValueProperty, double.TryParse(minValuePattern, out var minValue) ? minValue : .0);
			definition.SetValue(maxValueProperty,
				double.TryParse(maxValuePattern, out var maxValue) ? maxValue : double.PositiveInfinity);

			//if (bindLengthValue)
				Bind(grid, definition, lengthProperty);
			if (bindMinValue) 
				Bind(grid, definition, minValueProperty);
			if (bindMaxValue) 
				Bind(grid, definition, maxValueProperty);

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
			DependencyObject source, DependencyObject target) =>
			target.SetValue(dependencyProperty, source.GetValue(dependencyProperty));

		private static void OnSetChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs args)
		{
			if (o.Is(out UIElement element).Not()) return;

			var patterns = SplitPaterns((args.NewValue as string ?? "").ToUpperInvariant());
			var cPattern = patterns.FirstOrDefault(p => p.StartsWith("C") && !p.StartsWith("CS"))?.Replace("C", "");
			var rPattern = patterns.FirstOrDefault(p => p.StartsWith("R") && !p.StartsWith("RS"))?.Replace("R", "");
			var sssPattern = patterns.FirstOrDefault(p => p.StartsWith("SSS"))?.Replace("SSS", "");
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
			pattern = unitType.Is(GridUnitType.Star) ? pattern.Replace("*", "") : pattern;
			pattern = unitType.Is(GridUnitType.Star) && pattern.IsNullOrWhiteSpace() ? "1" : pattern;
			return double.TryParse(pattern, out var value) ? new GridLength(value, unitType) : new GridLength();
		}
	}
}