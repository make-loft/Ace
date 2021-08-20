using System;
using System.Globalization;
using Ace.Markup.Patterns;

namespace Ace.Markup
{
	public class ContextBinding : ABindingExtension
	{
		public ContextBinding() : base(default) => Key = default;
		public ContextBinding(string key) : base(default) => Key = key;

		public string Key { get; set; }

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
			value.To<ContextObject>().GetMediator(Ace.Context.Get(Key));
	}
}
