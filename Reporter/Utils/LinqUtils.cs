using System.Collections.Generic;
using System.Linq;

namespace Reporter.Utils
{
    public static class LinqUtils
    {
        public static SortableBindingList<T> ToSbl<T>(this IEnumerable<T> items)
        {
            return new SortableBindingList<T>(items.ToList());
        }
    }
}
