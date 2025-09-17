using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Extensions;

namespace Common.Util
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static string ToCommaSeparatedString(this IEnumerable source)
        {
            if (source is string str)
            {
                return str;
            }

            var array = source?.Cast<object>().ToArray();
            if (array.IsNullOrEmpty())
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");

            for (var index = 0; index < array.Length; index++)
            {
                var item = array[index];
                stringBuilder.Append(item);

                if (index != array.Length - 1)
                {
                    stringBuilder.Append(", ");
                }
            }

            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }
    }
}