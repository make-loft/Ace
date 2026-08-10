using System.Windows.Data;

namespace Ace.Markup;

public class SelfBinding : System.Windows.Data.Binding
{
	public SelfBinding() => RelativeSource = new(RelativeSourceMode.Self);
	public SelfBinding(string path) : this() => Path = new(path);
}
