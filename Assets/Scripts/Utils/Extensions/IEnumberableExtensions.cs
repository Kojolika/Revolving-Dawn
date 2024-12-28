using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class IEnumberableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
            => enumerable == null || !enumerable.Any();
    }
}