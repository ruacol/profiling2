using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Type;
using Profiling2.Domain.Contracts.Queries.Stats;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Stats
{
    public class SourceStatisticsQueries : NHibernateQuery, ISourceStatisticsQueries
    {
        public int GetSourceCount(bool archived)
        {
            string sql = "SELECT COUNT(*) FROM [PRF_Source] WHERE Archive = :archive";
            return Session.CreateSQLQuery(sql).SetParameter("archive", archived, NHibernateUtil.Boolean).UniqueResult<int>();
        }

        public Int64 GetTotalSize()
        {
            string sql = "SELECT SUM(DATALENGTH(FileData)) + SUM(DATALENGTH(OriginalFileData)) FROM [PRF_Source]";
            return Session.CreateSQLQuery(sql).UniqueResult<Int64>();
        }

        public Int64 GetTotalArchivedSize()
        {
            string sql = "SELECT SUM(DATALENGTH(FileData)) + SUM(DATALENGTH(OriginalFileData)) FROM [PRF_Source] WHERE Archive = 1";
            return Session.CreateSQLQuery(sql).UniqueResult<Int64>();
        }

        public IList<object[]> GetSourceImportsByDay()
        {
            var qo = Session.QueryOver<Source>()
                .Select(
                    Projections.SqlGroupProjection("YEAR(SourceDate) AS [Year]", "YEAR(SourceDate)", new string[] { "YEAR" }, new IType[] { NHibernateUtil.Int32 }),
                    Projections.SqlGroupProjection("MONTH(SourceDate) AS [Month]", "MONTH(SourceDate)", new string[] { "MONTH" }, new IType[] { NHibernateUtil.Int32 }),
                    Projections.SqlGroupProjection("DAY(SourceDate) AS [Day]", "DAY(SourceDate)", new string[] { "DAY" }, new IType[] { NHibernateUtil.Int32 }),
                    Projections.Count<Source>(x => x.Id)
                )
                .Where(x => !x.Archive)
                .OrderBy(Projections.SqlFunction("YEAR", NHibernateUtil.Int32, Projections.Property<Source>(x => x.SourceDate))).Asc
                .ThenBy(Projections.SqlFunction("MONTH", NHibernateUtil.Int32, Projections.Property<Source>(x => x.SourceDate))).Asc
                .ThenBy(Projections.SqlFunction("DAY", NHibernateUtil.Int32, Projections.Property<Source>(x => x.SourceDate))).Asc;

            return qo.List<object[]>();
        }

        public DateTime GetLastAdminSourceImportDate()
        {
            return Session.QueryOver<AdminSourceImport>().Select(x => x.ImportDate).OrderBy(x => x.ImportDate).Desc.Take(1).SingleOrDefault<DateTime>();
        }

        public int GetSourceCountByFolder(string folder, ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<Source>()
                .Where(x => !x.Archive)
                .AndRestrictionOn(x => x.SourcePath).IsInsensitiveLike(folder + "%")
                .RowCount();
        }

        public DateTime GetLastDateByFolder(string folder, ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            IList<DateTime> results = thisSession.QueryOver<Source>()
                .Select(x => x.FileDateTimeStamp)
                .Where(x => !x.Archive)
                .AndRestrictionOn(x => x.FileDateTimeStamp).IsNotNull
                .AndRestrictionOn(x => x.SourcePath).IsInsensitiveLike(folder + "%")
                .OrderBy(x => x.FileDateTimeStamp).Desc
                .Take(1)
                .List<DateTime>();

            if (results != null && results.Count > 0)
                return results[0];

            return DateTime.MinValue;
        }
    }
}
