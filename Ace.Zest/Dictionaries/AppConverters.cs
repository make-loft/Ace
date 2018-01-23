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
			{"NullToTrueConverter", new Booler {OnNull = true, OnNotNull = false}},
			{"NullToFalseConverter", new Booler {OnNull = false, OnNotNull = true}},
			{"NullToVisibleConverter", new Booler {OnNull = V.Visible, OnNotNull = V.Collapsed}},
			{"NullToCollapsedConverter", new Booler {OnNull = V.Collapsed, OnNotNull = V.Visible}},
			{"TrueToFalseConverter", new Booler {OnTrue = false, OnFalse = true, OnNull = true}},
			{"FalseToTrueConverter", new Booler {OnTrue = false, OnFalse = true, OnNull = false}},
			{"TrueToVisibleConverter", new Booler {OnTrue = V.Visible, OnFalse = V.Collapsed, OnNull = V.Collapsed}},
			{"TrueToCollapsedConverter", new Booler {OnTrue = V.Collapsed, OnFalse = V.Visible, OnNull = V.Visible}},
			{"FalseToVisibleConverter", new Booler {OnTrue = V.Collapsed, OnFalse = V.Visible, OnNull = V.Collapsed}},
			{"FalseToCollapsedConverter", new Booler {OnTrue = V.Visible, OnFalse = V.Collapsed, OnNull = V.Visible}},
			{"EqualsToCollapsedConverter", new EqualsConverter {OnEqual = V.Collapsed, OnNotEqual = V.Visible}},
			{"EqualsToVisibleConverter", new EqualsConverter {OnEqual = V.Visible, OnNotEqual = V.Collapsed}},
			{"EqualsToFalseConverter", new EqualsConverter {OnEqual = false, OnNotEqual = true}},
			{"EqualsToTrueConverter", new EqualsConverter {OnEqual = true, OnNotEqual = false}},
			{"AnyToCollapsedConverter", new AnyConverter {OnAny = V.Collapsed, OnNotAny = V.Visible}},
			{"AnyToVisibleConverter", new AnyConverter {OnAny = V.Visible, OnNotAny = V.Collapsed}},
			{"AnyToFalseConverter", new AnyConverter {OnAny = false, OnNotAny = true}},
			{"AnyToTrueConverter", new AnyConverter {OnAny = true, OnNotAny = false}},
		};
		
		public AppConverters()
		{
			foreach (var pair in DefaultKeyToConverterMap) Add(pair.Key, pair.Value);
		}
	}
}