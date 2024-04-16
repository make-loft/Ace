namespace HelloAceViews
{
	public partial class AppView
	{
		public AppView() => InitializeComponent();

		private object Converter_Convert(Ace.Markup.Patterns.ConvertArgs args) =>
				$"Title: {Title}\nDataContext:\n{DataContext}\nConverter Value: {args.Value}";
	}
}