namespace Ace.Controls;

public partial class ItemCell : Border
{
	static ItemCell()
	{
		Type<ItemCell>.When(v => v.ActiveCell).Changed += args =>
			args.Sender.Use(c => c.IsActive = c.ActiveCell.Is(c));
	}

	[RegisterProperty]
	public ItemCell ActiveCell
	{
		get => this.Get(default(ItemCell));
		set => this.Set(value);
	}

	[RegisterProperty]
	public bool IsActive
	{
		get => this.Get(false);
		set => this.Set(value);
	}
}
