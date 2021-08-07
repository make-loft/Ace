namespace Ace.Markup
{
	public class NullExtension : Patterns.AMarkupExtension
	{
		public override object Provide(object targetObject, object targetProperty) => null;
	}
}