using System.Windows.Data;

namespace Ace.Converters.Patterns
{
	public interface ITwoWayConverter : IValueConverter
	{
		IValueConverter BackConverter { get; set; }
	}
}