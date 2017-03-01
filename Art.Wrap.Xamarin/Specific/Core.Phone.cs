using System;
using System.Globalization;
using System.Threading;

namespace Aero.Specific
{
    public class Core
    {
        public static Core Aid = new Core();

        public string Localize(string key, string stringFormat = null)
        {
            return LocalizationSource.Wrap[key];
        }

        public void Trace(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
        }

        public object GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentCulture;
        }

        public void SetInvariantCulture()
        {
            SetCurrentCulture(CultureInfo.InvariantCulture);
        }

        public void SetCurrentCulture(object culture)
        {
            Thread.CurrentThread.CurrentCulture = (CultureInfo) culture;
        }
    }
}