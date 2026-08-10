using System.Windows.Media;

#if XAMARIN
using VisualElement = Xamarin.Forms.Element;
#endif
#if MAUI
using VisualElement = Microsoft.Maui.Controls.Element;
#endif
#if DESKTOP
using VisualElement = System.Windows.DependencyObject;
#endif

namespace Ace;

public static class VisualTree
{
	public static VisualElement GetVisualParent(this VisualElement current)
		=> VisualTreeHelper.GetParent(current);

	public static IEnumerable<VisualElement> EnumerateVisualChildren(this VisualElement current)
	{
		var n = VisualTreeHelper.GetChildrenCount(current);
		for (var i = 0; i < n; i++)
			yield return VisualTreeHelper.GetChild(current, i);
	}

	public static IEnumerable<VisualElement> EnumerateVisualDescendants(this VisualElement current)
	{
		foreach (var child in current.EnumerateVisualChildren())
		{
			yield return child;
			
			foreach (var descendant in child.EnumerateVisualDescendants())
				yield return descendant;
		}
	}

	public static IEnumerable<VisualElement> EnumerateVisualAncestors(this VisualElement current)
	{
		while (true)
		{
			var parent = VisualTreeHelper.GetParent(current);
			if (parent is null) yield break;
			yield return current = parent;
		}
	}

	public static IEnumerable<VisualElement> EnumerateSelfAndVisualAncestors(this VisualElement current)
		=> current.ToEnumerable().Concat(current.EnumerateVisualAncestors());
	
	public static IEnumerable<VisualElement> EnumerateSelfAndVisualDescendants(this VisualElement current)
		=> current.ToEnumerable().Concat(current.EnumerateVisualDescendants());
}