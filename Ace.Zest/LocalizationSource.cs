using System.Globalization;
using System.Linq;
using System.Resources;
using Ace.Patterns;

namespace Ace
{
    public class LocalizationSource : AResourceWrap<string, string, CultureInfo, ResourceManager>
    {
        public static readonly LocalizationSource Wrap = new LocalizationSource();

        public override string this[string key] =>
            ActiveManager?.GetString(key) ??
            MergedManagers.Select(m => m.GetString(key)).FirstOrDefault() ??
            GetDefault(key);

        public override string this[string key, CultureInfo culture] =>
            ActiveManager?.GetString(key, culture) ??
            MergedManagers.Select(m => m.GetString(key, culture)).FirstOrDefault() ??
            GetDefault(key, culture);

        public virtual string GetDefault(string key, CultureInfo culture = null) => ":" + key + "•";
    }
}