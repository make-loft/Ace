using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Ace.Controls
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class Pivot
	{
		public Pivot() => InitializeComponent();

		public static readonly BindableProperty ActiveCellProperty = BindableProperty.Create(nameof(ActiveCell), typeof(ItemCell), typeof(Pivot));

		public ItemCell ActiveCell
		{
			get => GetValue(ActiveCellProperty).To<ItemCell>();
			set => SetValue(ActiveCellProperty, value);
		}

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

			cell.SetBinding(ItemCell.IsActiveProperty, new Binding
			{
				Path = nameof(ActiveItem),
				Source = this,
				ConverterParameter = cell,
				Converter = IsActiveConverter ??= new(IsActiveConvert)
			});
		}
	}
}