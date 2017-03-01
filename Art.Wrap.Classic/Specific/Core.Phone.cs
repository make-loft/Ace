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
            return stringFormat == null
                ? LocalizationSource.Wrap[key]
                : string.Format(stringFormat, LocalizationSource.Wrap[key]);
        }

        public void Exit()
        {
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