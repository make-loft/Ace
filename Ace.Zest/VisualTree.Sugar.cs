using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
#if XAMARIN
using DependencyObject = Xamarin.Forms.Element;
#endif

namespace Ace
{
	public static class VisualTree
	{
		public static DependencyObject GetVisualParent(this DependencyObject current) =>
			VisualTreeHelper.GetParent(current);

		public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject current)
		{
			var n = VisualTreeHelper.GetChildrenCount(current);
			for (var i = 0; i < n; i++)
				yield return VisualTreeHelper.GetChild(current, i);
		}

		public static IEnumerable<DependencyObject> EnumerateVisualDescendants(this DependencyObject current)
		{
			foreach (var child in current.EnumerateVisualChildren())
			{
				yield return child;
				
				foreach (var descendant in child.EnumerateVisualDescendants())
				{
					yield return descendant;
				}
			}
		}

		public static IEnumerable<DependencyObject> EnumerateVisualAncestors(this DependencyObject current)
		{
			while (true)
			{
				var parent = VisualTreeHelper.GetParent(current);
				if (parent == null) yield break;
				yield return current = parent;
			}
		}
	}
}