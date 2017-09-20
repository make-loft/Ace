using System.Globalization;
using Windows.System.Threading;

// ReSharper disable once CheckNamespace

namespace System.Threading
{
    public class Thread
    {
        public static readonly Thread CurrentThread = new Thread(() => { });

        private event Action Action;

        public Thread(Action action)
        {
            Action = async () => await ThreadPool.RunAsync(state => action());
        }

        public CultureInfo CurrentCulture { get; set; }

        public void Start()
        {
            Action?.Invoke();
        }
    }
}