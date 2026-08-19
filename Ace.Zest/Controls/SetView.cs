using Ace.Controls;
using Ace.Mathematics;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

#if XAMARIN
using static Xamarin.Forms.ScrollOrientation;
using static Xamarin.Forms.ScrollToPosition;

using IView = Xamarin.Forms.View;
using Colors = Xamarin.Forms.Color;
using TappedHandler = System.EventHandler;
#endif
#if MAUI
using Microsoft.Maui.Controls.Shapes;
using static Microsoft.Maui.ScrollOrientation;
using static Microsoft.Maui.Controls.ScrollToPosition;
using TappedHandler = System.EventHandler<Microsoft.Maui.Controls.TappedEventArgs>;
#endif

namespace Ace.Markup;

public delegate object ItemMakerDelegate();

[ContentProperty(nameof(Items))]
public class SetView : ContentControl
{
	public class Cell : Rack
	{
#if MAUI
		public static IShape StrokeShape = new RoundRectangle { CornerRadius = new(5d) };
#endif
		private static readonly Color TransparentGrayColor = new(0.5f, 0.5f, 0.5f, 0.0f);
		private static readonly Brush TransparentGrayBrush = new SolidColorBrush(TransparentGrayColor);

		public void SetState(bool isSelected, SetView setView)
		{
			if (isSelected)
			{
				setView._selectedCell?.SetState(false, setView);
				setView._selectedCell = this;
			}

			Children[0].To<View>().BackgroundColor = isSelected
				? setView.SelectionBackgroundColor
				: TransparentGrayColor
				;

			Children[0].To<View>().Background = isSelected
				? setView.SelectionBackground
				: TransparentGrayBrush
				;
		}

		public IView Content
		{
			get => Children[1];
			set => Children[1] = value;
		}
	}

	private Cell _selectedCell;

	public Cell CreateCell(object item, ItemMakerDelegate maker, TappedHandler tapped,
		double size, ScrollOrientation orientation) => new Cell()
		{
			HeightRequest = orientation.Is(Vertical)
				? size
				: ItemLength > 0 ? ItemLength : Height
				,
			WidthRequest = orientation.Is(Horizontal)
				? size
				: ItemLength > 0 ? ItemLength : Width
				,
			BindingContext = item,
			Children =
			{
#if MAUI
				new Border { StrokeShape = Cell.StrokeShape, StrokeThickness = 0 },
#else
				new Frame { CornerRadius = 5f, HasShadow = false },
#endif
				maker().To(out var content).Is(out ViewCell cell)
					? cell.View
					: content.As<View>()
			},
			GestureRecognizers =
			{
				new TapGestureRecognizer().Use(r => r.Tapped += tapped)
			}
		}
		.Use(c => c.SetState(item.Is(SelectedItem), this));

	public async Task TryScrollTo(double scrollX, double scrollY, bool animated = true)
	{
		if (Content.IsNot(out ScrollView scrollView)) return;

		while (scrollView.Content.HeightRequest < 0)
			await Task.Delay(32);

		await scrollView.ScrollToAsync(scrollX, scrollY, animated);
	}

	public bool TryGetItemVisualOffset(object item, out double itemVisualOffset)
	{
		itemVisualOffset = double.NaN;

		var items = ItemsSource;
		if (items.IsNot()) return false;

		var group = groups.FirstOrDefault(g => g.Contains(item));
		if (group.IsNot()) return false;

		var isGrouping = GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();

		var itemSize = ItemSize;
		var groupHeaderSize = GroupHeaderSize;
		var groupVisualOffset = GetGroupVisualOffset(groups, group);

		var itemOffset = group.OffsetOf(item);
		itemVisualOffset = groupVisualOffset + itemOffset * itemSize / itemsInLineCount
			+ (isGrouping ? groupHeaderSize : 0d);

		return true;
	}

	public async Task TryScrollTo(object item, ScrollToPosition position = Center, bool animate = true)
	{
		if (Content.IsNot(out ScrollView scrollView) || IsLoaded is false) return;

		while (scrollView.Content.HeightRequest < 0)
			await Task.Delay(32);

		var items = ItemsSource;
		if (items.IsNot()) return;

		var group = groups.FirstOrDefault(g => g.Contains(item));
		if (group.IsNot()) return;

		var isGrouping = GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();

		var itemSize = ItemSize;
		var groupHeaderSize = GroupHeaderSize;
		var groupVisualOffset = GetGroupVisualOffset(groups, group);

		var itemOffset = group.OffsetOf(item);
		var itemVisualOffset = groupVisualOffset + itemOffset * itemSize / itemsInLineCount
			+ (isGrouping ? groupHeaderSize : 0d);

		double getScrollOffset() => scrollView.Orientation switch
		{
			Vertical => scrollView.ScrollY,
			Horizontal => scrollView.ScrollX,
			_ => throw new NotImplementedException()
		};

		var fromVisualOffset = getScrollOffset();

		var positionVisualOffset = (scrollView.Height - itemSize) * position switch
		{
			Start => 0d,
			Center => 0.5d,
			End => 1d,
			MakeVisible => fromVisualOffset < itemOffset ? 0d : fromVisualOffset > itemOffset ? 1d : 0.5d,
			_ => throw new NotImplementedException()
		};

		var tillVisualOffset = itemVisualOffset - positionVisualOffset;

		var scrollToAction = scrollView.Orientation switch
		{
			Vertical => New.Action(async (double value) => await scrollView.ScrollToAsync(0, value, false)),
			Horizontal => New.Action(async (double value) => await scrollView.ScrollToAsync(value, 0, false)),
			_ => throw new NotImplementedException()
		};


		if (animate)
		{
			if (_animationState.Is(true))
				_breakState = true;

			_animationState = true;
			await scrollToAction.Animate(
				change: value =>
					fromVisualOffset + (1d - Math.Pow(1d - value, 3d)) * (tillVisualOffset - fromVisualOffset),
				@break: () => _breakState,
				finish: () => _animationState = _breakState = false,
				framesCount: 48,
				frameDuration_Milliseconds: 4);
		}
		else
			scrollToAction(tillVisualOffset);
	}

	private bool _breakState;
	private bool _animationState;

	private INotifyCollectionChanged _collection;
	private int itemsInLineCount;

	void Changed()
	{
		if (ItemsSource.IsNot() || ItemTemplate.IsNot())
			return;

		if (Content is not ScrollView and not null)
			return;

		var scrollView = new ScrollView { Orientation = Orientation };
		var content = new AbsoluteLayout
		{
			VerticalOptions = LayoutOptions.Start,
			HorizontalOptions = LayoutOptions.Start,
		};

		Content = scrollView;

		indexToGroupContainer = new();

		scrollView.Content = content;
		scrollView.Scrolled += (sender, args) => FillContent(scrollView, content);

		void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
			=> FillContent(scrollView, content);

		if (_collection.Is())
		{
			_collection.CollectionChanged -= CollectionChanged;
			_collection = default;
		}

		if (ItemsSource.Is(out _collection))
		{
			_collection.CollectionChanged += CollectionChanged;
		}

		content.SizeChanged += async (sender, args) =>
		{
			var isVisibleSize = content.Height > 0 && content.Width > 0;

			if (ItemsSource.IsNot())
				return;

			IsLoaded = false;

			var items = ItemsSource.Cast<object>().ToList();
			var lineSize = scrollView.Orientation switch
			{
				Vertical => scrollView.Width,
				Horizontal => scrollView.Height,
				_ => throw new NotImplementedException()
			};

			var itemSize = ItemSize;
			var itemLength = ItemLength;
			itemsInLineCount = itemLength > 0d ? (int)(lineSize / itemLength) : 1;
			if (itemsInLineCount is 0) itemsInLineCount = 1;
			var isGrouping = GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();
			groups = isGrouping
				? items.Cast<IGrouping<object, object>>().ToList()
				: items.GroupBy(o => (object)0, o => o).ToList()
				;

			content.Children.Clear();
			indexToGroupContainer.Clear();
			groupToLines.Clear();

			foreach (var group in groups)
			{
				var groupItems = group.ToList();
				var lines = groupItems
					.GroupBy(o => (object)(groupItems.IndexOf(o) / itemsInLineCount))
					.Select(g => g.Cast<object>().ToList()).ToList()
					;

				groupToLines[group] = lines;
			}

			var groupsVisualSize = GetGroupsVisualSize(groups);
			if (Orientation.Is(Vertical)) content.HeightRequest = groupsVisualSize;
			if (Orientation.Is(Horizontal)) content.WidthRequest = groupsVisualSize;

			if (items.Count > scrollView.Height / itemLength)
				await Task.Delay(8);

			if (TryGetItemVisualOffset(SelectedItem, out var itemVisualOffset))
			{
				var selectedItemOffset = items.OffsetOf(SelectedItem);
				var animationVisualOffset = items.Count / 2 < selectedItemOffset ? itemSize - scrollView.Height : +0d; 
			
				if (Orientation is Vertical)
				{
					await scrollView.ScrollToAsync(scrollView.ScrollX, itemVisualOffset + animationVisualOffset, false);
				}
				if (Orientation is Horizontal)
				{
					await scrollView.ScrollToAsync(itemVisualOffset + animationVisualOffset, scrollView.ScrollY, false);
				}
			}

			FillContent(scrollView, content);

			IsLoaded = true;

			if (isVisibleSize)
				await TryScrollTo(SelectedItem);
		};
	}

	Dictionary<int, Controls.Stack> indexToGroupContainer;
	List<IGrouping<object, object>> groups = default;
	readonly Dictionary<IGrouping<object, object>, List<List<object>>> groupToLines = new();
	readonly Dictionary<List<object>, View> lineToLineContainer = new();

	void SetRange(ScrollView scrollView,
		out int fromGroupIndex, out int tillGroupIndex,
		out int fromLineIndex, out int tillLineIndex)
	{
		fromGroupIndex = 0;
		tillGroupIndex = groups.Count - 1;

		fromLineIndex = 0;
		tillLineIndex = 0;

		var scrollOffset = scrollView.Orientation switch
		{
			Vertical => scrollView.ScrollY,
			Horizontal => scrollView.ScrollX,
			_ => throw new NotImplementedException()
		};

		var viewScopeSize = scrollView.Orientation switch
		{
			Vertical => scrollView.Height,
			Horizontal => scrollView.Width,
			_ => throw new NotImplementedException()
		};

		var fromOffset = scrollOffset - 1 * viewScopeSize;
		var tillOffset = scrollOffset + 2 * viewScopeSize;

		var activeOffset = 0d;
		for (var groupIndex = 0; groupIndex < groups.Count; groupIndex++)
		{
			var group = groups[groupIndex];
			var lines = groupToLines[group];

			tillLineIndex = lines.Count;

			foreach (var line in lines)
			{
				activeOffset += ItemSize;

				if (activeOffset < fromOffset)
				{
					fromGroupIndex = groupIndex;
					fromLineIndex = lines.IndexOf(line);
				}

				if (activeOffset > tillOffset)
				{
					tillGroupIndex = groupIndex;
					tillLineIndex = lines.IndexOf(line);
					return;
				}
			}
		}
	}

	static Controls.Stack CreateStack(ScrollOrientation orientation) => orientation switch
	{
		Horizontal => new Controls.Stack
		{
			Orientation = StackOrientation.Horizontal,
			HorizontalOptions = LayoutOptions.FillAndExpand,
			VerticalOptions = LayoutOptions.Start,
		},
		Vertical => new Controls.Stack
		{
			Orientation = StackOrientation.Vertical,
			HorizontalOptions = LayoutOptions.Start,
			VerticalOptions = LayoutOptions.FillAndExpand,
		},
		_ => throw new NotImplementedException()
	};

	void FillContent(ScrollView scrollView, AbsoluteLayout content)
	{
		var items = ItemsSource;
		var itemSize = ItemSize;
		var groupHeaderSize = GroupHeaderSize;

		void tapped(object s, EventArgs e)
		{
			s.To(out Cell cell).BindingContext.To(out var item);

			SelectedItem = AllowSelectedItemReset && SelectedItem.Is(item)
				? default
				: item
				;
		}

		var isGrouping = IsGrouping;

		SetRange(scrollView,
			out var fromGroupIndex, out var tillGroupIndex,
			out var fromLineIndex, out var tillLineIndex
			);

		var headerMaker = GroupHeaderMaker ?? new(() => GroupHeaderTemplate?.CreateContent());
		var itemMaker = ItemMaker ?? new(() => ItemTemplate?.CreateContent());

		for (var groupIndex = fromGroupIndex; groupIndex <= tillGroupIndex; groupIndex++)
		{
			var group = groups[groupIndex];
			var lines = groupToLines[group];
			var lineOrientation =
				Orientation.Is(Vertical) ? Horizontal :
				Orientation.Is(Horizontal) ? Vertical :
				throw new NotSupportedException();

			if (indexToGroupContainer.TryGetValue(groupIndex, out var groupContainer) is false)
			{
				groupContainer = CreateStack(Orientation);
				var linesContainer = new Rack();

				indexToGroupContainer[groupIndex] = groupContainer;

				if (isGrouping)
				{
					CreateCell(groups[groupIndex], headerMaker, default, groupHeaderSize, scrollView.Orientation)
						.Use(groupContainer.Children.Add);
				}

				linesContainer.Use(groupContainer.Children.Add);
				groupContainer.Use(content.Children.Add);

				var groupVisualOffset = GetGroupVisualOffset(groups, group);
				var groupVisualSize = GetGroupVisualSize(group);

				if (Orientation.Is(Vertical))
				{
					groupContainer.Margin = new(0, groupVisualOffset, 0, 0);
					groupContainer.HeightRequest = groupVisualSize;
				}

				if (Orientation.Is(Horizontal))
				{
					groupContainer.Margin = new(groupVisualOffset, 0, 0, 0);
					groupContainer.WidthRequest = groupVisualSize;
				}
			}

			foreach (var line in lines)
			{
				var lineIndex = lines.IndexOf(line);

				if (groupIndex.Is(fromGroupIndex) && lineIndex < fromLineIndex)
					continue;

				if (groupIndex.Is(tillGroupIndex) && lineIndex > tillLineIndex)
					break;

				if (lineToLineContainer.ContainsKey(line))
					continue;

				var lineContainer = CreateStack(lineOrientation);
				lineToLineContainer[line] = lineContainer;

				line
					.Select(i => CreateCell(i, itemMaker, tapped, itemSize, scrollView.Orientation))
					.ForEach(lineContainer.Children.Add)
					;

				var lineOffset = ItemSize * lineIndex;
				lineContainer.Margin = Orientation switch
				{
					Vertical => new(0d, lineOffset, 0d, 0d),
					Horizontal => new(lineOffset, 0d, 0d, 0d),
					_ => throw new NotImplementedException()
				};

				groupContainer.Children.Last().To<Rack>().Children.Add(lineContainer);
			}
		}
	}

	double GetGroupsVisualSize(List<IGrouping<object, object>> groups)
		=> groups.Aggregate(0d, (s, g) => s + GetGroupVisualSize(g));

	double GetGroupVisualSize(IGrouping<object, object> group)
		=> groupToLines[group].Count * ItemSize + (IsGrouping ? GroupHeaderSize : 0);

	double GetGroupVisualOffset(List<IGrouping<object, object>> groups, IGrouping<object, object> group)
	{
		var visualOffset = 0d;
		for (var i = 0; i < groups.Count; i++)
		{
			var g = groups[i];
			if (g.Is(group))
				return visualOffset;
			var groupSize = GetGroupVisualSize(g);
			visualOffset += groupSize;
		}

		return visualOffset;
	}

	#region Properties
	bool IsGrouping => GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();
	
	public static BindableProperty IsLoadedProperty = Type<SetView>.Create(v => v.IsLoaded);
	public new bool IsLoaded
	{
		get => GetValue(IsLoadedProperty).To<bool>();
		set => SetValue(IsLoadedProperty, value);
	}

	public static BindableProperty SelectedItemProperty = Type<SetView>.Create(v => v.SelectedItem, args =>
	{
		var setView = args.Sender;
		var items = setView.ItemsSource ?? setView.Items;
		var item = args.NewValue;
		args.Sender.ItemSelected?.Invoke(item, new(item, items.IndexOf(item)));

		var groupContainers = args.Sender
			?.Content?.As<ScrollView>()
			?.Content?.As<AbsoluteLayout>()
			?.Children.OfType<Layout>()
			;

		if (groupContainers.IsNot()) return;

		foreach (var groupContainer in groupContainers)
		{
			var lines = groupContainer.Children.Last().To<Rack>().Children.OfType<Layout>();
			if (lines.IsNot()) return;

			foreach (var line in lines)
			{
				var cell = line.Children.OfType<Cell>().FirstOrDefault(c => c.BindingContext.Is(args.NewValue));
				if (cell.Is())
				{
					if (setView._selectedCell.Is(cell))
						return;

					cell.SetState(true, setView);
					return;
				}
			}

			if (setView.SelectedItem.Is()) return;
			setView._selectedCell?.SetState(false, setView);
			setView._selectedCell = default;
		}
	});

	public object SelectedItem
	{
		get => GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}

	public static BindableProperty ItemsSourceProperty
		= Type<SetView>.Create(v => v.ItemsSource, args => args.Sender.Changed());

	public IList ItemsSource
	{
		get => this.Get(default(IList));
		set => this.Set(value);
	}

	public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

	public ItemMakerDelegate GroupHeaderMaker;
	public ItemMakerDelegate ItemMaker;

	public bool IsGroupingEnabled { get; set; } = false;
	public DataTemplate GroupHeaderTemplate { get; set; }
	public BindingBase ItemDisplayBinding { get; set; }

	public static BindableProperty ItemTemplateProperty
		= Type<SetView>.Create(v => v.ItemTemplate, args => args.Sender.Changed());

	public DataTemplate ItemTemplate
	{
		get => this.Get(default(DataTemplate));
		set => this.Set(value);
	}
	public SmartSet<object> Items { get; } = new();

	public ScrollOrientation Orientation { get; set; } = Vertical;
	public double ItemSize { get; set; } = 48d;
	public double ItemLength { get; set; } = 0d;
	public double GroupHeaderSize { get; set; } = 48d;
	public bool AllowSelectedItemReset { get; set; } = true;

	public Color SelectionBackgroundColor { get; set; } = Colors.Orange;
	public Brush SelectionBackground { get; set; }
	#endregion
}
