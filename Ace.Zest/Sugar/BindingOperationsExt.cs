using System.Windows.Data;
using DepObject = System.Windows.DependencyObject;
using DepProperty = System.Windows.DependencyProperty;

namespace Ace
{
	public static class BindingOperationsExt
	{
#if WINDOWS_PHONE
        public static Binding GetBinding(this DepObject target, DepProperty dp) => null;
		
		public static void ClearBinding(this DepObject target, DepProperty dp) { }
		
		public static void ClearAllBindings(this DepObject target) { }
#else
		public static Binding GetBinding(this DepObject target, DepProperty dp) =>
			BindingOperations.GetBinding(target, dp);
		
		public static void ClearBinding(this DepObject target, DepProperty dp) =>
			BindingOperations.ClearBinding(target, dp);
		
		public static void ClearAllBindings(this DepObject target) =>
			BindingOperations.ClearAllBindings(target);
#endif
	}
}