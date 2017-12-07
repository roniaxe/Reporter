using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reporter.Utils
{
    public static class LinqUtils
    {
        public static SortableBindingList<T> ToSbl<T>(this IEnumerable<T> items)
        {
            return new SortableBindingList<T>(items.ToList());
        }

        public static Task<object> AsTask(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();
            cancellationToken.Register(() => tcs.TrySetCanceled(),
                useSynchronizationContext: false);
            return tcs.Task;
        }
    }
}
