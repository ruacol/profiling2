using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Type;
using Profiling2.Domain.Contracts.Queries.Stats;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Stats
{
    public class PersonStatisticsQuery : NHibernateQuery, IPersonStatisticsQuery
    {
        // Uses PRF_Person.Created which started on 23/1/2013.
        public IList<object[]> GetCreatedProfilesCountByMonth()
        {
            var qo = Session.QueryOver<Person>()
                .Select(
                    Projections.SqlGroupProjection("YEAR(Created) AS [Year]", "YEAR(Created)", new string[] { "YEAR" }, new IType[] { NHibernateUtil.Int32 }),
                    Projections.SqlGroupProjection("MONTH(Created) AS [Month]", "MONTH(Created)", new string[] { "MONTH" }, new IType[] { NHibernateUtil.Int32 }),
                    Projections.SqlGroupProjection("DAY(Created) AS [Day]", "DAY(Created)", new string[] { "DAY" }, new IType[] { NHibernateUtil.Int32 }),
                    Projections.Count<Person>(x => x.Id)
                )
                .Where(x => x.Created != null)
                .OrderBy(Projections.SqlFunction("YEAR", NHibernateUtil.Int32, Projections.Property<Person>(x => x.Created))).Asc
                .ThenBy(Projections.SqlFunction("MONTH", NHibernateUtil.Int32, Projections.Property<Person>(x => x.Created))).Asc
                .ThenBy(Projections.SqlFunction("DAY", NHibernateUtil.Int32, Projections.Property<Person>(x => x.Created))).Asc;

            return qo.List<object[]>();
        }

        public IList<object[]> GetLiveCreatedProfilesCount(ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<Person>()
                .Select(
                    Projections.SqlGroupProjection("ProfileStatusID AS [ProfileStatusID]", "ProfileStatusID", new string[] { "ProfileStatusID" }, new IType[] { NHibernateUtil.Int32 }),
                    Projections.Count<Person>(x => x.ProfileStatus)
                )
                .Where(x => !x.Archive)
                .List<object[]>();
        }

        public IList<object[]> GetProfileStatusCountsByOrganization()
        {
            string sql = @"
                select ps.ProfileStatusName, o.OrgShortName, COUNT(p.PersonID)
                  FROM [PRF_Person] as p
                  cross apply
                  (
                    select top 1 * from [PRF_Career] as c
                    where p.PersonID = c.PersonID and c.Archive = 0
                    order by c.IsCurrentCareer, c.YearOfStart, c.MonthOfStart, c.DayOfStart, c.YearOfEnd, c.MonthOfEnd, c.DayOfEnd 
                  ) as c
                  left join
                  PRF_Organization o on c.OrganizationID = o.OrganizationID,
                  PRF_ProfileStatus ps
                  where p.ProfileStatusID = ps.ProfileStatusID
                  and p.Archive = 0
                  group by ps.ProfileStatusName, o.OrgShortName
                  order by ps.ProfileStatusName, o.OrgShortName
            ";
            return Session.CreateSQLQuery(sql).List<object[]>();
        }

        public IList<object[]> GetProfileStatusCountsNoOrganization()
        {
            string sql = @"
                select ps.ProfileStatusName, COUNT(p.PersonID)
                  FROM [PRF_Person] as p,
                  PRF_ProfileStatus ps
                  where p.ProfileStatusID = ps.ProfileStatusID
                  and p.Archive = 0
                  and p.PersonID not in (select PersonID from [PRF_Career])
                  group by ps.ProfileStatusName
                  order by ps.ProfileStatusName
            ";
            return Session.CreateSQLQuery(sql).List<object[]>();
        }
    }
}
