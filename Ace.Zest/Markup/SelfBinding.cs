using System.Windows;
using System.Windows.Data;

namespace Ace.Markup
{
	public class SelfBinding : Binding
	{
		public SelfBinding() => RelativeSource = new RelativeSource(RelativeSourceMode.Self);
		public SelfBinding(string path) : this() => Path = new PropertyPath(path);
	}
}