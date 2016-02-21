using System;

namespace Profiling2.Domain.Extensions
{
    public static class StringExtensions
    {
        public static bool IContains(this string source, string toCheck)
        {
            if (string.IsNullOrEmpty(source))
                return false;
            else
                return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
