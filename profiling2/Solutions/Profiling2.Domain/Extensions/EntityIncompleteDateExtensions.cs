using System;
using System.Dynamic;

namespace Profiling2.Domain.Extensions
{
    public static class EntityIncompleteDateExtensions
    {
        public static bool HasIncompleteDate<T>(this T subject) 
            where T : IIncompleteDate
        {
            return subject.YearOfStart == 0 || subject.YearOfEnd == 0;
        }

        public static bool HasIntersectingDateWith<T>(this T subject, IIncompleteDate e)
            where T : IIncompleteDate
        {
            if (e != null)
            {
                DateTime rStartDate = new DateTime(subject.YearOfStart > 0 ? subject.YearOfStart : 1, subject.MonthOfStart > 0 ? subject.MonthOfStart : 1, subject.DayOfStart > 0 ? subject.DayOfStart : 1);
                DateTime rEndDate = new DateTime(subject.YearOfEnd > 0 ? subject.YearOfEnd : 1, subject.MonthOfEnd > 0 ? subject.MonthOfEnd : 1, subject.DayOfEnd > 0 ? subject.DayOfEnd : 1);
                DateTime eStartDate = new DateTime(e.YearOfStart > 0 ? e.YearOfStart : 1, e.MonthOfStart > 0 ? e.MonthOfStart : 1, e.DayOfStart > 0 ? e.DayOfStart : 1);
                DateTime eEndDate = new DateTime(e.YearOfEnd > 0 ? e.YearOfEnd : 1, e.MonthOfEnd > 0 ? e.MonthOfEnd : 1, e.DayOfEnd > 0 ? e.DayOfEnd : 1);

                bool result = true;
                if (e.YearOfStart > 0 || e.YearOfEnd > 0)
                {
                    if (e.YearOfStart > 0)
                    {
                        if (subject.YearOfStart > 0)
                            result = result && eStartDate >= rStartDate;
                        if (subject.YearOfEnd > 0)
                            result = result && eStartDate <= rEndDate;
                    }
                    if (e.YearOfEnd > 0)
                    {
                        if (subject.YearOfStart > 0)
                            result = result && eEndDate >= rStartDate;
                        if (subject.YearOfEnd > 0)
                            result = result && eEndDate <= rEndDate;
                    }
                }
                else
                    return false;

                return result;
            }
            return false;
        }

        /// <summary>
        /// Printable date summary which doesn't attempt to fill in empty values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static string GetDateSummary<T>(this T subject)
            where T : IIncompleteDate
        {
            string s = string.Empty;
            if (subject.HasStartDate())
            {
                s += subject.GetStartDateString();
                if (subject.HasEndDate())
                {
                    s += " - " + subject.GetEndDateString();
                }
            }
            else if (subject.HasEndDate())
            {
                s += subject.GetEndDateString();
            }
            return s;
        }

        public static bool HasStartDate<T>(this T subject)
            where T : IIncompleteDate
        {
            return subject.YearOfStart > 0 || subject.MonthOfStart > 0 || subject.DayOfStart > 0;
        }

        public static bool HasEndDate<T>(this T subject)
            where T : IIncompleteDate
        {
            return subject.YearOfEnd > 0 || subject.MonthOfEnd > 0 || subject.DayOfEnd > 0;
        }

        /// <summary>
        /// Fills in incomplete start date with minimal values.
        /// </summary>
        /// <returns>Null on invalid date.</returns>
        public static DateTime? GetStartDateTime<T>(this T subject)
            where T : IIncompleteDate
        {
            string y, m, d;
            y = subject.YearOfStart > 0 ? subject.YearOfStart.ToString() : "1";
            m = subject.MonthOfStart > 0 ? subject.MonthOfStart.ToString() : "1";
            d = subject.DayOfStart > 0 ? subject.DayOfStart.ToString() : "1";
            string start = string.Join("-", new string[] { y, m, d });

            DateTime result;
            if (DateTime.TryParse(start, out result))
                return result;
            return null;
        }

        /// <summary>
        /// Fills in incomplete end date with maximal values.
        /// </summary>
        /// <returns>Null on invalid date.</returns>
        public static DateTime? GetEndDateTime<T>(this T subject)
            where T : IIncompleteDate
        {
            int y, m, d;
            y = subject.YearOfEnd > 0 ? subject.YearOfEnd : 9999;
            m = subject.MonthOfEnd > 0 ? subject.MonthOfEnd : 12;
            d = subject.DayOfEnd > 0 ? subject.DayOfEnd : DateTime.DaysInMonth(y, m);
            string end = string.Join("-", new string[] { y.ToString(), m.ToString(), d.ToString() });

            DateTime result;
            if (DateTime.TryParse(end, out result))
                return result;
            return null;
        }

        /// <summary>
        /// Printable date which doesn't attempt to fill in empty values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static string GetStartDateString<T>(this T subject)
            where T : IIncompleteDate
        {
            string y, m, d;
            y = subject.YearOfStart > 0 ? subject.YearOfStart.ToString() : "-";
            m = subject.MonthOfStart > 0 ? subject.MonthOfStart.ToString() : "-";
            d = subject.DayOfStart > 0 ? subject.DayOfStart.ToString() : "-";
            return string.Join("/", new string[] { y, m, d });
        }

        /// <summary>
        /// Printable date which doesn't attempt to fill in empty values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static string GetEndDateString<T>(this T subject)
            where T : IIncompleteDate
        {
            string y, m, d;
            y = subject.YearOfEnd > 0 ? subject.YearOfEnd.ToString() : "-";
            m = subject.MonthOfEnd > 0 ? subject.MonthOfEnd.ToString() : "-";
            d = subject.DayOfEnd > 0 ? subject.DayOfEnd.ToString() : "-";
            return string.Join("/", new string[] { y, m, d });
        }

        public static bool IsCurrentAsOf<T>(this T subject, DateTime asOf)
            where T : IIncompleteDate
        {
            if (subject.HasStartDate() && subject.GetStartDateTime() < asOf)
                return false;
            if (subject.HasEndDate() && subject.GetEndDateTime() > asOf)
                return false;
            return true;
        }

        public static string PrintDates<T>(this T subject)
            where T : IIncompleteDate
        {
            string s = string.Empty;
            if (subject.HasStartDate())
                s += "from " + subject.GetStartDateString();
            if (subject.HasEndDate())
                s += (subject.HasStartDate() ? " " : string.Empty) + "until " + subject.GetEndDateString();
            return s;
        }

        public static object GetIncompleteStartDate<T>(this T subject)
            where T : IIncompleteDate
        {
            if (subject.HasStartDate())
            {
                dynamic o = new ExpandoObject();
                if (subject.YearOfStart > 0)
                    o.year = subject.YearOfStart;
                if (subject.MonthOfStart > 0)
                    o.month = subject.MonthOfStart;
                if (subject.DayOfStart > 0)
                    o.day = subject.DayOfStart;
                return o;
            }
            return null;
        }

        public static object GetIncompleteEndDate<T>(this T subject)
            where T : IIncompleteDate
        {
            if (subject.HasEndDate())
            {
                dynamic o = new ExpandoObject();
                if (subject.YearOfEnd > 0)
                    o.year = subject.YearOfEnd;
                if (subject.MonthOfEnd > 0)
                    o.month = subject.MonthOfEnd;
                if (subject.DayOfEnd > 0)
                    o.day = subject.DayOfEnd;
                return o;
            }
            return null;
        }

        /// <summary>
        /// Returns TimelineJS date object.
        /// </summary>
        /// <returns></returns>
        public static object GetTimelineStartDateObject<T>(this T subject)
            where T : IIncompleteDate
        {
            if (subject.HasStartDate())
                return subject.GetIncompleteStartDate();
            else if (subject.HasEndDate())
                return subject.GetIncompleteEndDate();
            return null;
        }

        /// <summary>
        /// Returns TimelineJS date object.
        /// </summary>
        /// <returns></returns>
        public static object GetTimelineEndDateObject<T>(this T subject)
            where T : IIncompleteDate
        {
            if (subject.HasEndDate())
                return subject.GetIncompleteEndDate();
            return null;
        }
    }
}
