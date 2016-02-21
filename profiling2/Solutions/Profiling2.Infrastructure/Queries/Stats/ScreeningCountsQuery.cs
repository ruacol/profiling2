using System.Collections.Generic;
using Profiling2.Domain.Contracts.Queries.Stats;
using SharpArch.NHibernate;
using System;
using Profiling2.Domain.Scr;

namespace Profiling2.Infrastructure.Queries.Stats
{
    public class ScreeningCountsQuery : NHibernateQuery, IScreeningCountsQuery
    {
//        SELECT YEAR(h.DateStatusReached) AS year, MONTH(h.DateStatusReached) AS month, COUNT(h.ScreeningRequestPersonFinalDecisionID)
//FROM [undp-drc-profiling].[dbo].[SCR_ScreeningRequestPersonFinalDecisionHistory] h
//GROUP BY YEAR(h.DateStatusReached), MONTH(h.DateStatusReached)
//ORDER BY YEAR(h.DateStatusReached) DESC, MONTH(h.DateStatusReached) DESC
        //public IList<object[]> GetFinalDecsionCount()
        //{
        //    return Session.QueryOver<ScreeningRequestPersonFinalDecisionHistory>()
        //        .Select(
        //            //Projections.Group<ScreeningRequestPersonFinalDecisionHistory>(x => x.DateStatusReached.Year),
        //            Projections.SqlGroupProjection("YEAR(DateStatusReached) AS [Year]", "YEAR(DateStatusReached)", new string[] { "YEAR" }, new IType[] { NHibernateUtil.Int32 }),
        //            //Projections.Group<ScreeningRequestPersonFinalDecisionHistory>(x => x.DateStatusReached.Month),
        //            Projections.SqlGroupProjection("MONTH(DateStatusReached) AS [Month]", "MONTH(DateStatusReached)", new string[] { "MONTH" }, new IType[] { NHibernateUtil.Int32 }),
        //            Projections.Count<ScreeningRequestPersonFinalDecisionHistory>(x => x.ScreeningRequestPersonFinalDecision)
        //        )
        //        //.OrderBy(x => x.DateStatusReached.Year).Desc
        //        .OrderBy(Projections.SqlFunction("YEAR", NHibernateUtil.Int32, Projections.Property<ScreeningRequestPersonFinalDecisionHistory>(x => x.DateStatusReached))).Asc
        //        //.ThenBy(x => x.DateStatusReached.Month).Desc
        //        .ThenBy(Projections.SqlFunction("MONTH", NHibernateUtil.Int32, Projections.Property<ScreeningRequestPersonFinalDecisionHistory>(x => x.DateStatusReached))).Asc
        //        .List<object[]>();
        //}

        /* Returns number of final screening results, by person, sorted by month.
         * Final screening results are only included if:
         * - the original request was completed;
         * - the original request was not archived.
         * An individual may be screened more than once, but is counted each time they are screened.
         */
        public IList<object[]> GetFinalDecisionCountByMonth()
        {
            string sql = @"SELECT YEAR(filtered.DateStatusReached) AS year, MONTH(filtered.DateStatusReached) AS month, COUNT(filtered.ScreeningRequestPersonFinalDecisionID)
                FROM 
                (
	                SELECT latestHistory.*
	                FROM SCR_Request r
	                CROSS APPLY
	                (
		                SELECT TOP 1 rh.*
		                FROM SCR_RequestHistory rh
		                WHERE rh.RequestID = r.RequestID
		                ORDER BY rh.DateStatusReached DESC
	                ) AS latestRequestHistory,
	                SCR_RequestPerson rp,
	                SCR_ScreeningRequestPersonFinalDecision fd
	                CROSS APPLY
	                (
		                SELECT TOP 1 h.*
		                FROM SCR_ScreeningRequestPersonFinalDecisionHistory h
		                WHERE h.ScreeningRequestPersonFinalDecisionID = fd.ScreeningRequestPersonFinalDecisionID
		                ORDER BY h.DateStatusReached DESC
	                ) AS latestHistory
	                WHERE fd.RequestPersonID = rp.RequestPersonID
	                AND rp.RequestID = r.RequestID
	                AND r.Archive = 0
	                AND latestRequestHistory.RequestStatusID = 8
                ) AS filtered
                GROUP BY YEAR(filtered.DateStatusReached), MONTH(filtered.DateStatusReached)
                ORDER BY YEAR(filtered.DateStatusReached) ASC, MONTH(filtered.DateStatusReached) ASC
            ";
            return Session.CreateSQLQuery(sql).List<object[]>();
        }

        public IList<object[]> GetFinalDecisionCountByRequestEntity(DateTime start, DateTime end)
        {
            string sql = @"
                SELECT re.RequestEntityName, COUNT(p.personID)
                FROM SCR_RequestPerson p,
                SCR_Request r
                CROSS APPLY
                (
                    SELECT TOP 1 x.*
                    FROM SCR_RequestHistory x
                    WHERE x.RequestID = r.RequestID
                    ORDER BY x.DateStatusReached desc
                ) as rh,
                SCR_RequestEntity re,
                SCR_RequestStatus rs,
                SCR_ScreeningRequestPersonFinalDecision d
                CROSS APPLY
                (
                    SELECT TOP 1 this.DateStatusReached 
                    FROM SCR_ScreeningRequestPersonFinalDecisionHistory AS this
                    WHERE this.ScreeningRequestPersonFinalDecisionID = d.ScreeningRequestPersonFinalDecisionID
                    ORDER BY DateStatusReached DESC
                ) AS h
                WHERE p.RequestPersonID = d.RequestPersonID
                AND p.RequestID = r.RequestID
                AND r.Archive = 0
                AND rs.RequestStatusID = rh.RequestStatusID
                AND rs.RequestStatusName = :requestStatusName
                AND r.RequestEntityID = re.RequestEntityID
                AND h.DateStatusReached BETWEEN :startDate AND :endDate
                GROUP BY re.RequestEntityName
                ORDER BY re.RequestEntityName
            ";
            return Session.CreateSQLQuery(sql)
                .SetString("requestStatusName", RequestStatus.NAME_COMPLETED)
                .SetDateTime("startDate", start)
                .SetDateTime("endDate", end)
                .List<object[]>();
        }
    }
}
