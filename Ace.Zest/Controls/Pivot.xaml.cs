//using Xamarin.Forms;
//using Xamarin.Forms.Xaml;

//using XamlCompilationAttribute = Microsoft.Maui.Controls.Xaml.XamlCompilationAttribute;
//using XamlCompilationOptions = Xamarin.Forms.Xaml.XamlCompilationOptions;

namespace Ace.Controls;

[XamlCompilation(XamlCompilationOptions.Skip)]
public partial class Pivot : List
{
	public Pivot() => InitializeComponent();

	private void ItemCell_Tapped(object sender, EventArgs e)
	{
		ActiveCell = sender.To(out ItemCell cell);
		ActiveItem = cell.BindingContext;
	}

	private object IsActiveConvert(Markup.Patterns.ConvertArgs args)
	{
		args.Parameter.To(out ItemCell cell);

		if (ActiveItem.IsNot() || cell.BindingContext.IsNot())
			return false;

		ActiveCell = cell;
		return cell.BindingContext.Is(ActiveItem);
	}

	Markup.Converters.Converter IsActiveConverter;
	private void ItemCell_BindingContextChanged(object sender, EventArgs args)
	{
		sender.To(out ItemCell cell);

		cell.SetBinding(Type<ItemCell>.GetProperty(v => v.IsActive), new Binding
		{
			Path = nameof(ActiveItem),
			Source = this,
			ConverterParameter = cell,
			Converter = IsActiveConverter ??= new(IsActiveConvert)
		});
	}
}