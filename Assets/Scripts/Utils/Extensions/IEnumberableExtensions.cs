using System.Collections;
using System.Collections.Generic;

namespace Utils.Extensions
{
    public static class IEnumberableExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable enumerable)
            => enumerable == null || !enumerable.GetEnumerator().MoveNext();
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
            => enumerable == null || !enumerable.GetEnumerator().MoveNext();
    }
}