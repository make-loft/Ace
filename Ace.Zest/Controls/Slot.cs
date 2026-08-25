#if MAUI
#endif
#if DESKTOP
using System.Windows;
using System.Windows.Markup;

using Label = System.Windows.Controls.TextBlock;
using ContentView = System.Windows.Controls.ContentPresenter;
#endif

namespace Ace.Controls;

[ContentProperty(nameof(Item))]
public class Slot : ContentControl
{
	public Slot() => this.ContextChanged(args => ApplyContent());

	public static readonly Property ItemTemplateProperty
		= Type<Slot>.Create(s => s.ItemTemplate, args => args.Sender.ApplyContent());

	public static readonly Property ItemProperty
		= Type<Slot>.Create(s => s.Item, args => args.Sender.ApplyContent());

	public DataTemplate ItemTemplate
	{
		get => this.Get(default(DataTemplate));
		set => this.Set(value);
	}

	public object Item
	{
		get => this.Get(default(object));
		set => this.Set(value);
	}

	private void ApplyContent() => Content = (Item ?? this.GetContext()).Is(out var item) && ItemTemplate.Is(out var template)
		? template.CreateView(item)
		: item.Is(out View view) ? view : new Label { Text = item?.ToString() }
		;
}
