using System;
using System.Windows.Input;
using System.Linq;

#if XAMARIN
using Xamarin.Forms;
using Property = Xamarin.Forms.Property;
#else
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Property = System.Windows.DependencyProperty;
using Binding =  System.Windows.Data.Binding;
#endif

namespace Ace.Controls
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
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

			cell.SetBinding(ItemCell.IsActiveProperty, new Binding
			{
				Path = new(nameof(ActiveItem)),
				Source = this,
				ConverterParameter = cell,
				Converter = IsActiveConverter ??= new(IsActiveConvert)
			});
		}

		private void ItemCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) =>
			ActivateCell((ItemCell)sender);

		private static Key[] ActivationKeys = { Key.Enter, Key.Space };
		private void ItemCell_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (ActivationKeys.Contains(e.Key))
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
}