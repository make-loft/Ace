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

			var group = groups.FirstOrDefault(g => g.Contains(item));
			if (group.IsNot()) return;

			var isGrouping = GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();

			var itemSize = ItemSize;
			var groupHeaderSize = GroupHeaderSize;
			var groupOffset = GetGroupOffset(groups, group);

			var itemIndex = group.OffsetOf(item);
			var offset = isGrouping
				? groupOffset + itemIndex * itemSize + groupHeaderSize
				: groupOffset
				;

			var itemSizeHalf = itemSize / 2d;
			var length =
				position.Is(Start) ? -itemSizeHalf :
				position.Is(Center) ? -scrollView.Height / 2 + itemSizeHalf :
				position.Is(End) ? +scrollView.Height + itemSizeHalf :
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
		private int itemsInLineCount;

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
				itemsInLineCount = itemLength > 0d ? (int)(lineSize / itemLength) : 1;
				var isGrouping = GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();
				groups = isGrouping
					? items.Cast<IGrouping<object, object>>().ToList()
					: items.GroupBy(o => (object)0, o => o).ToList()
					;

				indexToGroupContainer.Clear();
				if (Orientation.Is(Vertical)) content.HeightRequest = 0d;
				if (Orientation.Is(Horizontal)) content.WidthRequest = 0d;

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

				var groupsSize = GetGroupsSize(groups);
				if (Orientation.Is(Vertical)) content.HeightRequest = groupsSize;
				if (Orientation.Is(Horizontal)) content.WidthRequest = groupsSize;

				if (items.Count > scrollView.Height / itemLength)
					await Task.Delay(8);

				FillContent(scrollView, content);

				IsLoaded = true;
			};
		}

		Dictionary<int, Controls.Stack> indexToGroupContainer;
		List<IGrouping<object, object>> groups = default;
		Dictionary<IGrouping<object, object>, List<List<object>>> groupToLines = new();
		Dictionary<List<object>, View> lineToLineContainer = new();

		void SetRange(ScrollView scrollView,
			out int fromGroupIndex, out int tillGroupIndex,
			out int fromLineIndex, out int tillLineIndex
			)
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

					var groupOffset = GetGroupOffset(groups, group);
					var groupSize = GetGroupSize(group);

					if (Orientation.Is(Vertical))
					{
						groupContainer.Margin = new(0, groupOffset, 0, 0);
						groupContainer.HeightRequest = groupSize;
					}

					if (Orientation.Is(Horizontal))
					{
						groupContainer.Margin = new(groupOffset, 0, 0, 0);
						groupContainer.WidthRequest = groupSize;
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

		double GetGroupsSize(List<IGrouping<object, object>> groups) =>
			groups.Aggregate(0d, (s, g) => s + GetGroupSize(g));

		bool IsGrouping => GroupHeaderMaker.Is() || GroupHeaderTemplate.Is();
		double GetGroupSize(IGrouping<object, object> group) =>
			groupToLines[group].Count * ItemSize + (IsGrouping ? GroupHeaderSize : 0);

		double GetGroupOffset(List<IGrouping<object, object>> groups, IGrouping<object, object> group)
		{
			var offset = 0d;
			for (var index = 0; index < groups.Count; index++)
			{
				var g = groups[index];
				if (g.Is(group))
					return offset;
				var groupSize = GetGroupSize(g);
				offset += groupSize;
			}

			return offset;
		}

		#region Properties
		public static BindableProperty IsLoadedProperty = Type<SetView>.Create(v => v.IsLoaded);
		public bool IsLoaded
		{
			get => GetValue(IsLoadedProperty).To<bool>();
			set => SetValue(IsLoadedProperty, value);
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
		#endregion
	}
}
