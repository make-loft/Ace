using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Aero
{
    public static class AsyncAdapter
    {
        public static TResult Await<TResult>(this Task<TResult> operation)
        {
            return operation.Result;
        }

        public static TResult Await<TResult>(this IAsyncOperation<TResult> operation)
        {
            return operation.AsTask().Result;
        }

        public static TResult Await<TResult, TProgress>(this IAsyncOperationWithProgress<TResult, TProgress> operation)
        {
            return operation.AsTask().Result;
        }
    }

    internal static class RoutedCommandsAdapter
    {
        public static void SetCommandBindings(object frameworkElement, object contextObject)
        {
        }
    }
}
