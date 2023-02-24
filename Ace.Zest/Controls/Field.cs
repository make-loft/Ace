using System.Threading.Tasks;

namespace Ace.Controls
{
#if XAMARIN
	public class Field : Xamarin.Forms.Entry
	{
		public Field()
		{
			bool isCaptured = false;

			TextChanged += async (o, e) =>
			{
				if (isCaptured || IsEnabled.Not())
					return;

				IsEnabled = false;
				await Task.Delay(2000);
				IsEnabled = true;
			};

			Focused += (o, e) => isCaptured = true;

			Unfocused += async (o, e) =>
			{
				await Task.Delay(1000);
				isCaptured = false;
			};
		}
	}
#else
	public class Field : System.Windows.Controls.TextBox { }
#endif
}
