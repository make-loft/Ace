using System.Windows.Data;
using DepObject = System.Windows.DependencyObject;
using DepProperty = System.Windows.DependencyProperty;

namespace Ace
{
	public static class BindingOperationsExt
	{
		public static Binding GetBinding(this DepObject target, DepProperty dp) =>
			BindingOperations.GetBinding(target, dp);
		
		public static void ClearBinding(this DepObject target, DepProperty dp) =>
			BindingOperations.ClearBinding(target, dp);
		
		public static void ClearAllBindings(this DepObject target) =>
			BindingOperations.ClearAllBindings(target);
	}
}