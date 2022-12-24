#if XAMARIN
using Xamarin.Forms;
using DependencyProperty = Xamarin.Forms.BindableProperty;
using static System.Windows.BindablePropertyExtensions;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static System.Windows.DependencyProperty;
using static System.Windows.Controls.RowDefinition;
using static System.Windows.Controls.ColumnDefinition;
#endif

namespace Ace.Markup
{
	public partial class Rack : Grid
	{
		//public static readonly DependencyProperty Rows_Property = Register(
		//	"Rows", typeof(string), typeof(Rack), GetMetadata<Rack>((grid, args) =>
		//	SetRows(grid, (string)args.NewValue)));

		//public static readonly DependencyProperty Columns_Property = Register(
		//	"Columns", typeof(string), typeof(Rack), GetMetadata<Rack>((grid, args) =>
		//	SetColumns(grid, (string)args.NewValue)));

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
