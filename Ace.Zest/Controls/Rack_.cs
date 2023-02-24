#if XAMARIN
using Xamarin.Forms;
#else
using System.Windows.Controls;
#endif

namespace Ace.Controls
{
	public partial class Rack : Grid
	{
		public string Rows
		{
			get => GetValue(RowsProperty).To<string>();
			set => SetValue(RowsProperty, value);
		}

		public string Columns
		{
			get => GetValue(ColumnsProperty).To<string>();
			set => SetValue(ColumnsProperty, value);
		}
	}
}
