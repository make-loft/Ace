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
}
