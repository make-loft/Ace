using System.Windows;
using Ace.Converters;
using V = System.Windows.Visibility;
using Booler = Ace.Converters.BooleanConverter;
using KeyToConverterMap = System.Collections.Generic.Dictionary<string, System.Windows.Data.IValueConverter>;

namespace Ace.Dictionaries
{
	public class AppConverters : ResourceDictionary
	{
		public static readonly KeyToConverterMap DefaultKeyToConverterMap = new KeyToConverterMap
		{
			{"NullToTrueConverter", new Booler {OnNull = true, ByDefault = false}},
			{"NullToFalseConverter", new Booler {OnNull = false, ByDefault = true}},
			{"NullToVisibleConverter", new Booler {OnNull = V.Visible, ByDefault = V.Collapsed}},
			{"NullToCollapsedConverter", new Booler {OnNull = V.Collapsed, ByDefault = V.Visible}},
			{"TrueToFalseConverter", new Booler {OnTrue = false, OnFalse = true, OnNull = true}},
			{"FalseToTrueConverter", new Booler {OnTrue = false, OnFalse = true, OnNull = false}},
			{"TrueToVisibleConverter", new Booler {OnTrue = V.Visible, OnFalse = V.Collapsed, OnNull = V.Collapsed}},
			{"TrueToCollapsedConverter", new Booler {OnTrue = V.Collapsed, OnFalse = V.Visible, OnNull = V.Visible}},
			{"FalseToVisibleConverter", new Booler {OnTrue = V.Collapsed, OnFalse = V.Visible, OnNull = V.Collapsed}},
			{"FalseToCollapsedConverter", new Booler {OnTrue = V.Visible, OnFalse = V.Collapsed, OnNull = V.Visible}},
			{"EqualsToCollapsedConverter", new EqualsConverter {OnEquals = V.Collapsed, ByDefault = V.Visible}},
			{"EqualsToVisibleConverter", new EqualsConverter {OnEquals = V.Visible, ByDefault = V.Collapsed}},
			{"EqualsToFalseConverter", new EqualsConverter {OnEquals = false, ByDefault = true}},
			{"EqualsToTrueConverter", new EqualsConverter {OnEquals = true, ByDefault = false}},
			{"AnyToCollapsedConverter", new AnyConverter {OnAny = V.Collapsed, ByDefault = V.Visible}},
			{"AnyToVisibleConverter", new AnyConverter {OnAny = V.Visible, ByDefault = V.Collapsed}},
			{"AnyToFalseConverter", new AnyConverter {OnAny = false, ByDefault = true}},
			{"AnyToTrueConverter", new AnyConverter {OnAny = true, ByDefault = false}},
		};
		
		public AppConverters()
		{
			foreach (var pair in DefaultKeyToConverterMap) Add(pair.Key, pair.Value);
		}
	}
}