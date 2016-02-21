using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Type;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class EventSearchQuery : NHibernateQuery, IEventSearchQuery
    {
        protected string term { get; set; }
        protected int? year { get; set; }
        protected int? month { get; set; }
        protected int? day { get; set; }

        protected void ParseTerm(string term)
        {
            this.year = null;
            this.month = null;
            this.day = null;

            if (!string.IsNullOrEmpty(term))
            {
                this.term = term;

                string yearStr, monthStr, dayStr;
                int year, month, day;
                Match yearMonthDayMatch = Regex.Match(term, @"(\d{4})([-/\.]{1})(\d{1,2})([-\/.]{1})(\d{1,2})");
                Match yearMonthMatch = Regex.Match(term, @"(\d{4})([-/\.]{1})(\d{1,2})");
                Match yearMatch = Regex.Match(term, @"\d{4}");

                if (yearMonthDayMatch.Success)
                {
                    yearStr = yearMonthDayMatch.Groups[1].Value;
                    monthStr = yearMonthDayMatch.Groups[3].Value;
                    dayStr = yearMonthDayMatch.Groups[5].Value;
                    if (int.TryParse(yearStr, out year))
                    {
                        this.year = year;
                    }
                    if (int.TryParse(monthStr, out month))
                    {
                        this.month = month;
                    }
                    if (int.TryParse(dayStr, out day))
                    {
                        this.day = day;
                    }
                    this.term = this.term.Replace(yearMonthDayMatch.Value, "").Trim();
                }
                else if (yearMonthMatch.Success)
                {
                    yearStr = yearMonthMatch.Groups[1].Value;
                    monthStr = yearMonthMatch.Groups[3].Value;
                    if (int.TryParse(yearStr, out year))
                    {
                        this.year = year;
                    }
                    if (int.TryParse(monthStr, out month))
                    {
                        this.month = month;
                    }
                    this.term = this.term.Replace(yearMonthMatch.Value, "").Trim();
                }
                else if (yearMatch.Success)
                {
                    yearStr = yearMatch.Value;
                    if (int.TryParse(yearStr, out year))
                    {
                        this.year = year;
                    }
                    this.term = this.term.Replace(yearMatch.Value, "").Trim();
                }
                else
                {
                    this.term = term;
                }
            }
        }

        public IList<Event> GetResults(string term)
        {
            //var qo = Session.QueryOver<Event>();
            //if (!string.IsNullOrEmpty(term))
            //    return qo.Where(Restrictions.On<Event>(x => x.EventName).IsLike("%" + term + "%"))
            //        .OrderBy(x => x.EventName).Asc
            //        .Take(50)
            //        .List<Event>();
            //else
            //    return new List<Event>();

            if (!string.IsNullOrEmpty(term))
            {
                this.ParseTerm(term);

                // TODO this returns the event multiple times when joining with Violations
                ICriteria criteria = Session.CreateCriteria<Event>()
                    .CreateAlias("Location", "l")
                    .CreateAlias("Violations", "v")
                    .Add(Expression.Sql(@"
                        (v2_.Name LIKE ? COLLATE Latin1_general_CI_AI
                        OR l1_.LocationName LIKE ? COLLATE Latin1_general_CI_AI)
                    ", new object[] { "%" + this.term + "%", "%" + this.term + "%" }, new IType[] { NHibernateUtil.String, NHibernateUtil.String }));

                // TODO these conditions don't account for events covering years
                if (this.year.HasValue)
                {
                    criteria = criteria.Add(Expression.Sql(@"
                        (YearOfStart = ? OR YearOfEnd = ? OR ? BETWEEN YearOfStart AND YearOfEnd)
                    ", new object[] { this.year, this.year, this.year }, new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32, NHibernateUtil.Int32 }));
                }
                if (this.month.HasValue)
                {
                    criteria = criteria.Add(Expression.Sql(@"(
                        MonthOfStart = ? OR MonthOfEnd = ? OR ? BETWEEN MonthOfStart AND MonthOfEnd
                    )", new object[] { this.month, this.month, this.month }, new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32, NHibernateUtil.Int32 }));
                }
                if (this.day.HasValue)
                {
                    criteria = criteria.Add(Expression.Sql(@"(
                        DayOfStart = ? OR DayOfEnd = ? OR ? BETWEEN DayOfStart AND DayOfEnd
                    )", new object[] { this.day, this.day, this.day }, new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32, NHibernateUtil.Int32 }));
                }

                return criteria.AddOrder(Order.Desc("YearOfStart"))
                    .AddOrder(Order.Desc("MonthOfStart"))
                    .AddOrder(Order.Desc("DayOfStart"))
                    .SetMaxResults(100)
                    .List<Event>();
            }
            else
                return new List<Event>();
        }

        public IList<Tag> SearchTags(string term)
        {
            return Session.QueryOver<Tag>()
                .WhereRestrictionOn(x => x.TagName).IsInsensitiveLike("%" + term + "%")
                .And(x => !x.Archive)
                .List();
        }
    }
}
