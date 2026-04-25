using System.Linq;
using System.Windows.Input;
using Binding = System.Windows.Data.Binding;

namespace Ace.Controls;

public partial class Pivot
{
	public Pivot() => InitializeComponent();

	private object IsActiveConvert(Markup.Patterns.ConvertArgs args)
	{
		args.Parameter.To(out ItemCell cell);

		if (ActiveItem.IsNot() || cell.GetContext().IsNot())
			return false;

		ActiveCell = cell;
		return cell.GetContext().Is(ActiveItem);
	}

	Markup.Converters.Converter IsActiveConverter;

	//private void ItemCell_BindingContextChanged(object sender, EventArgs args)
	private void ItemCell_BindingContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
	{
		sender.To(out ItemCell cell);

		cell.SetBinding(Type<ItemCell>.GetProperty(v => v.IsActive), new Binding
		{
			Path = new(nameof(ActiveItem)),
			Source = this,
			ConverterParameter = cell,
			Converter = IsActiveConverter ??= new(IsActiveConvert)
		});
	}

	private void ItemCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
	{
		if (args.ClickCount.IsNot(1))
			return;

		ActivateCell((ItemCell)sender);
	}

	private static readonly Key[] ActivationKeys = { Key.Enter, Key.Space };
	private void ItemCell_PreviewKeyDown(object sender, KeyEventArgs args)
	{
		if (ActivationKeys.Contains(args.Key))
			ActivateCell((ItemCell)sender);
	}

	private void ActivateCell(ItemCell cell)
	{
		ActiveCell = cell;
		var activeItem = cell.GetContext();
		ActiveItem = ActiveItemUnset.Is(true) && ActiveItem.Is(activeItem)
			? default
			: activeItem;
	}
}