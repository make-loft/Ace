using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

// ReSharper disable once CheckNamespace
namespace System.Windows.Media
{
	public static class VisualTreeHelper
	{
		public static Element GetParent(Element element) => element.Parent;

		public static int GetChildrenCount(Element element) => GetChildrenCount(GetContent(element));

		public static Element GetChild(Element element, int index) => GetChildren(GetContent(element), index);

		private static int GetChildrenCount(object content) => content is IList<View> listedContent
			? listedContent.Count
			: content is null
				? 0
				: 1;

		private static Element GetChildren(object content, int index) => content is IList<View> listedContent
			? listedContent[index]
			: content as Element;

		private static PropertyInfo GetContentProperty(Type type) =>
			type.GetRuntimeProperty("Children") ?? type.GetRuntimeProperty("Content");

		private static object GetContent(this Element element) =>
			GetContentProperty(element.GetType())?.GetValue(element);
	}

	public static class BrushBrushExtensions
	{
		public static Brush Clone(this Brush brush) => brush switch
		{
			SolidColorBrush b => b.Clone(),
			LinearGradientBrush b => b.Clone(),
			RadialGradientBrush b => b.Clone(),
			_ => throw new NotImplementedException(),
		};

		public static SolidColorBrush Clone(this SolidColorBrush value) => new(value.Color);
		public static LinearGradientBrush Clone(this LinearGradientBrush value) => new()
		{
			GradientStops = value.GradientStops,
			StartPoint = value.StartPoint,
			EndPoint = value.EndPoint,
		};

		public static RadialGradientBrush Clone(this RadialGradientBrush value) => new()
		{
			GradientStops = value.GradientStops,
			Radius = value.Radius,
		};
	}
}
