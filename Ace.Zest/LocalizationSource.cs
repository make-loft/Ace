using System.Globalization;
using System.Linq;
using System.Resources;
using Ace.Patterns;

namespace Ace
{
	public class LocalizationSource : AResourceWrap<string, string, CultureInfo, ResourceManager>
	{
		public static readonly LocalizationSource Wrap = new LocalizationSource();

		public override string this[string key] => key.Is()
			? ActiveManager?.GetString(key) ??
			  MergedManagers.Select(m => m.GetString(key)).FirstOrDefault() ??
			  GetDefault(key)
			: default;

		public override string this[string key, CultureInfo culture] => key.Is()
			? ActiveManager?.GetString(key, culture) ??
			  MergedManagers.Select(m => m.GetString(key, culture)).FirstOrDefault() ??
			  GetDefault(key, culture)
			: default;

		public virtual string GetDefault(string key, CultureInfo culture = null) => $":{key}•";
	}
}