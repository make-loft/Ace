namespace Ace.Markup.Converters;

public class LocalizingConverter : Patterns.AValueConverter.Reflected
{
	public override object Convert(object value) =>
		LocalizationSource.Wrap[value?.ToString()];
}
