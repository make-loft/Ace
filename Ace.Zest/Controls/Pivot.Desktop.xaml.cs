using System;

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

		public static readonly Property ActiveCellProperty
			= Type<Pivot>.Create(p => p.ActiveCell);

		public ItemCell ActiveCell
		{
			get => GetValue(ActiveCellProperty).To<ItemCell>();
			set => SetValue(ActiveCellProperty, value);
		}

		private void ItemCell_Tapped(object sender, EventArgs e)
		{
			ActiveCell = sender.To(out ItemCell cell);
			ActiveItem = cell.GetContext();
		}

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

		private void ItemCell_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			ActiveCell = sender.To(out ItemCell cell);
			ActiveItem = cell.GetContext();
		}
	}
}