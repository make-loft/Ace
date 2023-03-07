namespace Ace.Controls
{
	public partial class Pick
	{
		public Pick() => InitializeComponent();

		public string DisplayMemberPath { get; set; }

		private object Converter_Convert(Markup.Patterns.ConvertArgs args)
		{
			var value = args.Value;
			if (DisplayMemberPath.IsNullOrWhiteSpace())
				return value;

			var type = value.GetType();
			var propertyNames = DisplayMemberPath.Split('.');
			foreach (var propertyName in propertyNames)
				value = type.GetProperty(propertyName).GetValue(value);
			return value;
		}
	}
}