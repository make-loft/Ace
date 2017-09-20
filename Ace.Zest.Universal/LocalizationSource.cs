using System.Globalization;
using System.Linq;
using System.Resources;
using Aero.Patterns;

namespace Aero
{
    public class LocalizationSource : AResourceWrap<string, string, CultureInfo, ResourceManager>
    {
        public static readonly LocalizationSource Wrap = new LocalizationSource();

        public override string this[string key]
        {
            get
            {
                if (ActiveManager == null) return ":" + key + "•";
                return ActiveManager.GetString(key) ??
                       MergedManagers.Select(m => m.GetString(key)).FirstOrDefault() ??
                       ":" + key + "•";
            }
        }

        public override string this[string key, CultureInfo culture]
        {
            get
            {
                if (ActiveManager == null) return ":" + key + "•";
                return ActiveManager.GetString(key, culture) ??
                       MergedManagers.Select(m => m.GetString(key, culture)).FirstOrDefault() ??
                       ":" + key + "•";
            }
        }
    }
}
