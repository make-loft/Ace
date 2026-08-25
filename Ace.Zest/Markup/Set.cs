using System.Collections;
using System.Collections.ObjectModel;

namespace Ace.Markup;

public class Set : ObservableCollection<object>
{
	public IList Source
	{
		set => this.AppendRange(value.Cast<object>());
	}
}
