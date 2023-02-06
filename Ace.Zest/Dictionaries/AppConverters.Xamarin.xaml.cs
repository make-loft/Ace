using Xamarin.Forms.Xaml;

namespace Ace.Zest.Dictionaries
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class AppConverters
	{
		public AppConverters() => InitializeComponent();

		private object Not_Convert(Markup.Patterns.ConvertArgs args) => args.Value.Is(out bool b) ? b.Not() : args.Value;
	}
}