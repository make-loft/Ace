using Ace.Converters.Patterns;

namespace HelloAceViews
{
	public partial class AppView
	{
		public AppView()
		{
			InitializeComponent();
		}

		private void InlineConverter_OnConverting(object sender, ConverterEventArgs e)
		{
			e.ConvertedValue =
				string.Format("Title: {0} \nDataContext:\n{1} \nConverter Value: {2}",
					Title,
					DataContext,
					e.Value);
		}
	}
}