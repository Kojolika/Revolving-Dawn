using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class StringExtensions
    {
        public static string ToCommaSeparatedList(this IEnumerable<string> source)
        {
            // prevents multiple enumeration
            var stringArray = source as string[] ?? source.ToArray();

            return stringArray.IsNullOrEmpty()
                ? string.Empty
                : stringArray.Aggregate((a, b) => $"{a}, {b}");
        }
    }
}