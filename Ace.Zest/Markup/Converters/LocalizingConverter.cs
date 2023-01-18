using Ace.Markup.Patterns;

namespace Ace.Markup.Converters
{
	public class LocalizingConverter : AValueConverter.Reflected
	{
		public override object Convert(object value) =>
			LocalizationSource.Wrap[value?.ToString().Replace(" ", "").Replace("-", "")];
	}
}
