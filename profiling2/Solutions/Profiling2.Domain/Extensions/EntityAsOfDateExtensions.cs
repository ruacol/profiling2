using System;
using System.Dynamic;

namespace Profiling2.Domain.Extensions
{
    public static class EntityAsOfDateExtensions
    {
        public static bool HasAsOfDate<T>(this T subject)
            where T : IAsOfDate
        {
            return subject.YearAsOf > 0 || subject.MonthAsOf > 0 || subject.DayAsOf > 0;
        }

        /// <summary>
        /// Printable date which doesn't attempt to fill in empty values.
        /// </summary>
        /// <returns></returns>
        public static string GetAsOfDateString<T>(this T subject)
            where T : IAsOfDate
        {
            string y, m, d;
            y = subject.YearAsOf > 0 ? subject.YearAsOf.ToString() : "-";
            m = subject.MonthAsOf > 0 ? subject.MonthAsOf.ToString() : "-";
            d = subject.DayAsOf > 0 ? subject.DayAsOf.ToString() : "-";
            return string.Join("/", new string[] { y, m, d });
        }

        public static DateTime GetDateTime(int year, int month, int day)
        {
            return new DateTime(year > 0 ? year : 1, month > 0 ? month : 1, day > 0 ? day : 1);
        }

        /// <summary>
        /// Replaces missing date parameters with the value '1' - usable in date boundary calculations or sorting.
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetStartDate<T>(this T subject)
            where T : IAsOfDate
        {
            if (subject.YearOfStart > 0 || subject.MonthOfStart > 0 || subject.DayOfStart > 0)
                return GetDateTime(subject.YearOfStart, subject.MonthOfStart, subject.DayOfStart);
            return null;
        }

        /// <summary>
        /// Replaces missing date parameters with the value '1' - usable in date boundary calculations or sorting.
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetEndDate<T>(this T subject)
            where T : IAsOfDate
        {
            if (subject.YearOfEnd > 0 || subject.MonthOfEnd > 0 || subject.DayOfEnd > 0)
                return GetDateTime(subject.YearOfEnd, subject.MonthOfEnd, subject.DayOfEnd);
            return null;
        }

        /// <summary>
        /// Replaces missing date parameters with the value '1' - usable in date boundary calculations or sorting.
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetAsOfDate<T>(this T subject)
            where T : IAsOfDate
        {
            if (subject.YearAsOf > 0 || subject.MonthAsOf > 0 || subject.DayAsOf > 0)
                return GetDateTime(subject.YearAsOf, subject.MonthAsOf, subject.DayAsOf);
            return null;
        }

        /// <summary>
        /// Returns best date to use in order to sort careers chronologically.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetSortDate<T>(this T subject)
            where T : IAsOfDate
        {
            DateTime? a = subject.GetAsOfDate();
            DateTime? s = subject.GetStartDate();
            DateTime? e = subject.GetEndDate();
            if (a.HasValue)
                return a.Value;
            else if (s.HasValue)
                return s.Value;
            else if (e.HasValue)
                return e.Value;
            return new DateTime();
        }

        public static object GetIncompleteAsOfDate<T>(this T subject)
            where T : IAsOfDate
        {
            if (subject.HasAsOfDate())
            {
                dynamic o = new ExpandoObject();
                if (subject.YearAsOf > 0)
                    o.year = subject.YearAsOf;
                if (subject.MonthAsOf > 0)
                    o.month = subject.MonthAsOf;
                if (subject.DayAsOf > 0)
                    o.day = subject.DayAsOf;
                return o;
            }
            return null;
        }
    }
}
