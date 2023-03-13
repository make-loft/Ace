using System.Linq;
using System.Reflection;

namespace Ace.Extensions
{
	public static class Reflection
	{
		public static PropertyInfo GetProperty(this object @this, string name) =>
			@this.GetType().GetProperty(name)
			?? @this.GetType().GetInterfaces().Select(i => i.GetProperty(name)).FirstOrDefault(p => p.Is());

		public static object Get(this object @this, string propertyName) => @this.GetProperty(propertyName).GetValue(@this);
		public static void Set(this object @this, string propertyName, object value) => @this.GetProperty(propertyName).SetValue(@this, value);
	}
}
