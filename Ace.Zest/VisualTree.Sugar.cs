using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
#if XAMARIN
using DependencyObject = Xamarin.Forms.Element;
#endif

namespace Ace
{
    public static class VisualTree
    {
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject current)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(current); i++)
                yield return VisualTreeHelper.GetChild(current, i);
        }

        public static DependencyObject GetVisualParent(this DependencyObject current) => 
            VisualTreeHelper.GetParent(current);

        public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject current) => 
            current.GetVisualChildren().SelectMany(child => child.GetVisualDescendants());

        public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject current)
        {
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(current);
                if (parent == null) yield break;
                yield return current = parent;
            }
        }

        //public static TType GetDataContext<TType>(this DependencyObject current) where TType : class
        //{
        //    var element = current as FrameworkElement;
        //    var context = element?.DataContext as TType;
        //    while (context == null && current != null)
        //    {
        //        current = current.GetVisualParent();
        //        element = current as FrameworkElement;
        //        if (element != null) context = element.DataContext as TType;
        //    }

        //    return context;
        //}
    }
}