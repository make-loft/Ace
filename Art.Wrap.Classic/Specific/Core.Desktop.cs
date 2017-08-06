using System;

namespace Art.Specific
{
    public class Core
    {
        public static Core Aid = new Core();

        public string Localize(string key, string stringFormat = null) => LocalizationSource.Wrap[key];

        public void Exit() => Environment.Exit(0);

        public void Trace(Exception exception) => System.Diagnostics.Trace.WriteLine(exception);
    }
}