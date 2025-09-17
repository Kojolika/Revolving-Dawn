using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is string str)
            {
                return string.IsNullOrEmpty(str);
            }
            
            return enumerable == null || !enumerable.Any();
        }
    }
}