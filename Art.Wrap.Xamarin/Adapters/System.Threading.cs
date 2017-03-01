using System.Globalization;

// ReSharper disable once CheckNamespace

namespace System.Threading
{
    public class Thread
    {
        public static readonly Thread CurrentThread = new Thread(() => { });

        private readonly Action _action;

        public Thread(Action action)
        {
            _action = action;
        }

        public CultureInfo CurrentCulture { get; set; }

        public void Start()
        {
        }
    }
}