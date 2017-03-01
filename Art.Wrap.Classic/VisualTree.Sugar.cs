using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Aero
{
    public static class VisualTree
    {
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject dependencyObject)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                yield return VisualTreeHelper.GetChild(dependencyObject, i);
            }
        }

        public static DependencyObject GetVisualParent(this DependencyObject dependencyObject)
        {
            return VisualTreeHelper.GetParent(dependencyObject);
        }

        public static List<TType> GetVisualDescendants<TType>(this DependencyObject dependencyObject)
            where TType : DependencyObject
        {
            var children = new List<TType>();
            foreach (var child in dependencyObject.GetVisualChildren())
            {
                children.AddRange(child.GetVisualDescendants<TType>());
                var item = child as TType;
                if (item != null) children.Add(item);
            }

            return children;
        }

        public static List<TType> GetVisualAncestors<TType>(this DependencyObject dependencyObject)
            where TType : DependencyObject
        {
            var parants = new List<TType>();
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent == null) return new List<TType>();
            parants.AddRange(parent.GetVisualAncestors<TType>());
            var item = parent as TType;
            if (item != null) parants.Add(item);
            return parants;
        }

        public static TType GetNearestVisualAncestor<TType>(this DependencyObject dependencyObject,
            Func<TType, bool> condition = null)
            where TType : DependencyObject
        {
            if (dependencyObject == null) return null;
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            return parent is TType && (condition == null || condition((TType) parent))
                ? (TType) parent
                : GetNearestVisualAncestor<TType>(parent);
        }

        public static TType GetDataContext<TType>(this DependencyObject dependencyObject) where TType : class
        {
            var element = dependencyObject as FrameworkElement;
            var context = element == null ? null : element.DataContext as TType;
            while (context == null && dependencyObject != null)
            {
                dependencyObject = dependencyObject.GetVisualParent();
                element = dependencyObject as FrameworkElement;
                if (element != null) context = element.DataContext as TType;
            }

            return context;
        }
    }
}