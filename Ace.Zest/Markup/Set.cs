using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ace.Markup
{
	public class Set : ObservableCollection<object>
	{
		public IList Source
		{
			set => this.MergeMany(value.Cast<object>());
		}
	}
}
