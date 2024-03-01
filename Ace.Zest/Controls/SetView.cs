using Ace.Controls;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

using static Xamarin.Forms.ScrollOrientation;
using static Xamarin.Forms.ScrollToPosition;

namespace Ace.Markup
{
	public delegate object ItemMakerDelegate();

	[ContentProperty(nameof(Items))]
	public class SetView : ContentView
	{
		public class Cell : Rack
		{
			private static readonly Color TranparentGray = new(0.5, 0.5, 0.5, 0.0); 

			public void SetState(bool isSelected, SetView setView)
			{
				if (isSelected)
				{
					setView._selectedCell?.SetState(false, setView);
					setView._selectedCell = this;
				}

				Children[0].BackgroundColor = isSelected
					? Color.Orange
					: TranparentGray
					;
			}

			public View Content
			{
				get => Children[1];
				set => Children[1] = value;
			}
		}

		private Cell _selectedCell;

		public Cell CreateCell(object item, ItemMakerDelegate maker, EventHandler tapped,
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
				new Frame { CornerRadius = 5f, HasShadow = false },
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

		public async void TryScrollTo(double scrollX, double scrollY, bool animated = true)
		{
			var scrollView = (ScrollView)Content;
			if (scrollView.IsNot()) return;

			while (IsLoaded.Not() || scrollView.Content.HeightRequest < 0)
				await Task.Delay(32);

			await scrollView.ScrollToAsync(scrollX, scrollY, animated);
		}

		public async void TryScrollTo(object item, ScrollToPosition position = Center, bool animated = true)
		{
			var scrollView = (ScrollView)Content;
			if (scrollView.IsNot()) return;

			while (IsLoaded.Not() || scrollView.Content.HeightRequest < 0)
				await Task.Delay(32);

			var items = ItemsSource;
			if (items.IsNot()) return;

			var group = lineGroups.FirstOrDefault(g => g.Contains(item));
			if (group.IsNot()) return;

			var isGrouping = GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();

			var itemSize = ItemSize;
			var groupHeaderSize = GroupHeaderSize;
			var groupOffset = isGrouping
				? GetOffset(lineGroups, group, itemSize, groupHeaderSize)
				: lineGroups.IndexOf(group) * itemSize
				;

			var itemIndex = group.IndexOf(item);
			var offset = isGrouping
				? groupOffset + itemIndex * itemSize + groupHeaderSize
				: groupOffset
				;

			var itemSizeHalf = itemSize / 2d;
			var length =
				position.Is(Start) ? -itemSizeHalf :
				position.Is(Center) ? -scrollView.Height / 2 + itemSizeHalf :
				position.Is(End) ? +scrollView.Height + itemSizeHalf  :
				0d;

			var from = scrollView.Orientation switch
			{
				Vertical => scrollView.ScrollY,
				Horizontal => scrollView.ScrollX,
				_ => throw new NotImplementedException()
			};

			var till = offset + length;
			if (animated)
				Mathematics.Visualisation
					.Animate(async v => await scrollView.ScrollToAsync(0, v, false), from, till);
			else
				await scrollView.ScrollToAsync(0, till, animated);
		}

		private INotifyCollectionChanged _collection;

		void Changed()
		{
			if (ItemsSource.IsNot() || ItemTemplate.IsNot())
				return;

			var scrollView = new ScrollView { Orientation = Orientation };
			var content = new Rack();

			Content = scrollView;

			indexToGroupContainer = new();

			scrollView.Content = content;
			scrollView.Scrolled += (o, e) => FillContent(scrollView, content);

			void CollectionChanged(object sender, NotifyCollectionChangedEventArgs args) =>
				FillContent(scrollView, content);

			if (_collection.Is())
			{
				_collection.CollectionChanged -= CollectionChanged;
				_collection = default;
			}

			if (ItemsSource.Is(out _collection))
			{
				_collection.CollectionChanged += CollectionChanged;
			}

			content.SizeChanged += async (o, e) =>
			{
				if (ItemsSource.IsNot()) return;

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
				var itemsInLineCount = itemLength > 0d ? (int)(lineSize / itemLength) : 1;
				var isGrouping = GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();
				groups = isGrouping
					? items.Cast<IGrouping<object, object>>().ToList()
					: items.GroupBy(o => (object)(items.IndexOf(o) / itemsInLineCount)).ToList()
					;

				lineGroups = groups.Cast<IEnumerable>().Select(g => g.Cast<object>().ToList()).ToList();
				indexToGroupContainer.Clear();

				var totalLinesCount = (int)Math.Ceiling((double)(items.Count / itemsInLineCount));
				var totalSize = isGrouping
					? GetGroupsSize(lineGroups, itemLength, GroupHeaderSize)
					: totalLinesCount * itemSize
					;

				if (Orientation.Is(Vertical)) content.HeightRequest = totalSize;
				if (Orientation.Is(Horizontal)) content.WidthRequest = totalSize;

				if (items.Count > scrollView.Height / itemLength)
					await Task.Delay(8);

				FillContent(scrollView, content);

				IsLoaded = true;
			};
		}

		public static BindableProperty IsLoadedProperty = Type<SetView>.Create(v => v.IsLoaded);
		public bool IsLoaded
		{
			get => GetValue(IsLoadedProperty).To<bool>();
			set => SetValue(IsLoadedProperty, value);
		}

		Dictionary<int, View> indexToGroupContainer;
		List<IGrouping<object, object>> groups = default;
		List<List<object>> lineGroups = default;

		void SetRange(ScrollView scrollView, Func<List<object>, double> getGroupSize, out int fromIndex, out int tillIndex)
		{
			fromIndex = 0;
			tillIndex = lineGroups.Count - 1;

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
			for (var groupIndex = 0; groupIndex < lineGroups.Count; groupIndex++)
			{
				var group = lineGroups[groupIndex];
				var groupSize = getGroupSize(group);

				activeOffset += groupSize;

				if (activeOffset < fromOffset)
				{
					fromIndex = groupIndex;
				}

				if (activeOffset > tillOffset)
				{
					tillIndex = groupIndex;
					break;
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

		void FillContent(ScrollView scrollView, Rack content)
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

				ItemSelected?.Invoke(SelectedItem, new(item, items.IndexOf(item)));
			}

			var isGrouping = GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();

			double GetSize(List<object> group) => isGrouping
					? GetGroupSize(group, itemSize, groupHeaderSize)
					: itemSize
					;

			SetRange(scrollView, GetSize,
				out var fromIndex, out var tillIndex);

			var headerMaker = GroupHeaderMaker ?? new(() => GroupHeaderTemplate?.CreateContent());
			var itemMaker = ItemMaker ?? new(() => ItemTemplate?.CreateContent());

			for (var groupIndex = fromIndex; groupIndex <= tillIndex; groupIndex++)
			{
				var lineGroup = lineGroups[groupIndex];

				if (indexToGroupContainer.TryGetValue(groupIndex, out var view))
					continue;

				var lineOrientation =
					isGrouping ? Vertical :
					Orientation.Is(Vertical) ? Horizontal : 
					Orientation.Is(Horizontal) ? Vertical : 
					throw new NotSupportedException();

				var lineContainer = CreateStack(lineOrientation);
				var groupContainer = CreateStack(Orientation);

				indexToGroupContainer[groupIndex] = groupContainer;

				lineGroup
					.Cast<object>()
					.Select(i => CreateCell(i, itemMaker, tapped, itemSize, scrollView.Orientation))
					.ForEach(lineContainer.Children.Add)
					;

				if (isGrouping)
				{
					CreateCell(groups[groupIndex], headerMaker, default, groupHeaderSize, scrollView.Orientation)
						.Use(groupContainer.Children.Add);
				}

				groupContainer.Children.Add(lineContainer);
				groupContainer.Use(content.Children.Add);

				var groupOffset = isGrouping
					? GetOffset(lineGroups, lineGroup, itemSize, groupHeaderSize)
					: itemSize * groupIndex
					;

				if (Orientation.Is(Vertical))
				{
					groupContainer.Margin = new(0, groupOffset, 0, 0);
					groupContainer.HeightRequest = isGrouping
						? itemSize * (lineGroup.Count + 1)
						: itemSize
						;
				}

				if (Orientation.Is(Horizontal))
				{
					groupContainer.Margin = new(groupOffset, 0, 0, 0);
					groupContainer.WidthRequest = isGrouping
						? itemSize * (lineGroup.Count + 1)
						: itemSize
						;
				}
			}
		}


		double GetGroupsSize(List<List<object>> groups, double itemSize, double groupHeaderSize) =>
			groups.Aggregate(0d, (s, g) => s + GetGroupSize(g, itemSize, groupHeaderSize));

		double GetGroupSize(List<object> group, double itemSize, double groupHeaderSize) =>
			group.Count * itemSize + groupHeaderSize;

		double GetOffset(List<List<object>> groups, List<object> group, double itemSize, double groupHeaderSize)
		{
			var offset = 0d;
			for (var index = 0; index < groups.Count; index++)
			{
				var g = groups[index];
				if (g.Is(group))
					return offset;
				var groupSize = GetGroupSize(g, itemSize, groupHeaderSize);
				offset += groupSize;
			}

			return offset;
		}

		public static BindableProperty SelectedItemProperty = Type<SetView>.Create(v => v.SelectedItem, args =>
		{
			var lines = args.Sender.To(out var setView)
				.Content?.To<ScrollView>()
				.Content?.To<Rack>()
				.Children.OfType<Layout<View>>();

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
		});

		public object SelectedItem
		{
			get => GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public static BindableProperty ItemsSourceProperty = Type<SetView>.Create(v => v.ItemsSource, args =>
			args.Sender.Changed());

		public IList ItemsSource
		{
			get => this.Get(default(IList));
			set => this.Set(value);
		}

		public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
		public event ItemMakerDelegate GroupHeaderMaker;
		public event ItemMakerDelegate ItemMaker;

		public bool IsGroupingEnabled { get; set; } = false;
		public DataTemplate GroupHeaderTemplate { get; set; }
		public BindingBase ItemDisplayBinding { get; set; }
		public DataTemplate ItemTemplate { get; set; }
		public SmartSet<object> Items { get; } = new();

		public ScrollOrientation Orientation { get; set; } = Vertical;
		public double ItemSize { get; set; } = 48d;
		public double ItemLength { get; set; } = 0d;
		public double GroupHeaderSize { get; set; } = 48d;
		public bool AllowSelectedItemReset { get; set; } = true;
	}
}
