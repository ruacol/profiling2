using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    /// <summary>
    /// This cut and slice of source search results is done in SQL here because there's no easy way to use CONTAINSTABLE
    /// using NHibernate QueryOver method (see SourceDataTablesQuery for the original attempt).
    /// </summary>
    public class SourceSQLQuery : NHibernateQuery, ISourceDataTablesQuery
    {
        protected string BaseSearchSQL(bool canAccessRestricted, string searchName, string searchExt, string searchText, DateTime? start, DateTime? end,
            IList<int> adminSourceSearchIds, int? userId, int? personId, int? eventId, string authorSearchText)
        {
            string q = @"FROM [PRF_Source] AS S ";
            if (!string.IsNullOrEmpty(searchText))
            {
                q = q + "INNER JOIN CONTAINSTABLE([PRF_Source], [FileData], :searchText) AS C ON C.[Key] = S.[SourceID]";
            }
            if (!string.IsNullOrEmpty(authorSearchText))
            {
                q = q + @"
                    INNER JOIN [PRF_SourceAuthorSource] sas ON S.[SourceID] = sas.[SourceID]
                    INNER JOIN [PRF_SourceAuthor] sa ON sas.[SourceAuthorID] = sa.[SourceAuthorID]
                ";
            }
            // IsRelevant is transformed here from a nullable bool, to an integer enum that can be sorted in the SQL query.
            // 2 = relevant, 1 = unmarked, 0 = irrelevant.
            q = q + @"
                OUTER APPLY
				(
					SELECT TOP 1 CASE 
                        WHEN RS.[IsRelevant] = 1 THEN 2
                        WHEN RS.[IsRelevant] IS NULL THEN 1
                        WHEN RS.[IsRelevant] = 0 THEN 0
                        END AS IsRelevant
					FROM [PRF_AdminReviewedSource] AS RS
					WHERE RS.[SourceID] = S.[SourceID]
                    AND RS.[AdminSourceSearchID] IN ( :adminSourceSearchIds )
					AND RS.[Archive] = 0
					ORDER BY RS.[ReviewedDateTime] DESC
				) RS
            ";
            if (userId != null)
            {
                q = q + @"
                    OUTER APPLY
                    (
                        SELECT TOP 1 RS.[ReviewedDateTime]
                        FROM [PRF_AdminReviewedSource] AS RS 
                        INNER JOIN [PRF_AdminSourceSearch] AS SS ON RS.[AdminSourceSearchID] = SS.[AdminSourceSearchID]
					    WHERE RS.[SourceID] = S.[SourceID]
                        AND SS.[SearchedByAdminUserID] = :userId
                        AND (RS.[WasDownloaded] = 1 OR RS.[WasPreviewed] = 1)
                        ORDER BY RS.[ReviewedDateTime] DESC
                    ) READS
                ";
            }
            if (personId.HasValue)
            {
                q = q + @"
                    OUTER APPLY
                    (
                        SELECT COUNT(PS.[PersonSourceID]) AS IsAttached
                        FROM [PRF_PersonSource] AS PS
                        WHERE PS.[SourceID] = S.[SourceID]
                        AND PS.[PersonID] = :personId
                    ) ATTACHED_SOURCES
                ";
            }
            if (eventId.HasValue)
            {
                q = q + @"
                    OUTER APPLY
                    (
                        SELECT COUNT(ES.[EventSourceID]) AS IsAttached
                        FROM [PRF_EventSource] AS ES
                        WHERE ES.[SourceID] = S.[SourceID]
                        AND ES.[EventID] = :eventId
                    ) ATTACHED_SOURCES
                ";
            }
            q = q + " WHERE (:canAccessRestricted = 1 OR S.[IsRestricted] = 0) AND S.[Archive] = 0";
            if (!string.IsNullOrEmpty(authorSearchText))
                q = q + " AND sa.[Author] LIKE :authorSearchText";
            if (!string.IsNullOrEmpty(searchName))
                q = q + " AND S.[SourcePath] LIKE :searchName";
            if (!string.IsNullOrEmpty(searchExt))
                q = q + " AND S.[SourcePath] LIKE :searchExt";
            if (start.HasValue)
                q = q + " AND s.[FileDateTimeStamp] >= :start";
            if (end.HasValue)
                q = q + " AND s.[FileDateTimeStamp] <= :end";
            return q;
        }

        protected string BaseAttachedSelectSQL(int? personId, int? eventId)
        {
            if (personId.HasValue || eventId.HasValue)
                return "ATTACHED_SOURCES.IsAttached, ";
            else
                return string.Empty;
        }

        public int GetSearchTotal(bool canAccessRestricted, string searchName, string searchExt, string searchText, DateTime? start, DateTime? end, string authorSearchText)
        {
            string str = "SELECT COUNT(S.[SourceID]) " + BaseSearchSQL(canAccessRestricted, searchName, searchExt, searchText, start, end, null, null, null, null, authorSearchText);
            IQuery q = Session.CreateSQLQuery(str).SetString("canAccessRestricted", canAccessRestricted ? "1" : "0");
            if (!string.IsNullOrEmpty(searchText))
                q.SetString("searchText", searchText);
            q.SetParameterList("adminSourceSearchIds", new List<int>{ 0 });
            if (!string.IsNullOrEmpty(searchName))
                q.SetString("searchName", "%" + searchName + "%");
            if (!string.IsNullOrEmpty(searchExt))
                q.SetString("searchExt", "%" + searchExt);
            if (start.HasValue)
                q.SetDateTime("start", start.Value);
            if (end.HasValue)
                q.SetDateTime("end", end.Value);
            if (!string.IsNullOrEmpty(authorSearchText))
                q.SetString("authorSearchText", "%" + authorSearchText + "%");
            return q.UniqueResult<int>();
        }

        public IList<SourceSearchResultDTO> GetPaginatedResults(bool canAccessRestricted, int iDisplayStart, int iDisplayLength,
            string searchName, string searchExt, string searchText, DateTime? start, DateTime? end, IList<int> adminSourceSearchIds,
            int iSortingCols, List<int> iSortCol, List<string> sSortDir,
            int userId, int? personId, int? eventId, string authorSearchText)
        {
            // not returning ot.FileData due to memory issues
            string str = string.Format(@"SELECT ot.SourceID, ot.SourceName, ot.FullReference, ot.SourcePath, ot.SourceDate,
                    ot.FileExtension, ot.IsRestricted, ot.FileDateTimeStamp, ot.Archive, ot.IsReadOnly, ot.ReviewedDateTime, ot.IsRelevant {3} {5}
                FROM (
                    SELECT S.*, READS.ReviewedDateTime, ISNULL(RS.IsRelevant, 1) AS IsRelevant {4}, {2} ROW_NUMBER() OVER (ORDER BY {0}) as SortRow 
                    {1}
                ) AS ot
                WHERE ot.SortRow > :iDisplayStart 
                ORDER BY ot.SortRow
            ", new object[] { 
                 OrderBySQL(iSortingCols, iSortCol, sSortDir, !string.IsNullOrEmpty(searchText)), 
                 BaseSearchSQL(canAccessRestricted, searchName, searchExt, searchText, start, end, adminSourceSearchIds, userId, personId, eventId, authorSearchText),
                 BaseAttachedSelectSQL(personId, eventId),
                 (personId.HasValue || eventId.HasValue ? ", ot.IsAttached" : string.Empty),
                 string.IsNullOrEmpty(searchText) ? string.Empty : ", C.Rank",
                 string.IsNullOrEmpty(searchText) ? string.Empty : ", ot.Rank"
            });
            IQuery q = Session.CreateSQLQuery(str)
                .SetResultTransformer(Transformers.AliasToBean(typeof(SourceSearchResultDTO)))
                .SetMaxResults(iDisplayLength)
                .SetInt32("iDisplayStart", iDisplayStart)
                .SetString("canAccessRestricted", canAccessRestricted ? "1" : "0")
                .SetInt32("userId", userId);
            if (!string.IsNullOrEmpty(searchText))
                q.SetString("searchText", searchText);
            if (adminSourceSearchIds == null)
                adminSourceSearchIds.Add(0);
            else if (adminSourceSearchIds.Count < 1)
                adminSourceSearchIds.Add(0);
            q.SetParameterList("adminSourceSearchIds", adminSourceSearchIds);
            if (!string.IsNullOrEmpty(searchName))
                q.SetString("searchName", "%" + searchName + "%");
            if (!string.IsNullOrEmpty(searchExt))
                q.SetString("searchExt", "%" + searchExt);
            if (start.HasValue)
                q.SetDateTime("start", start.Value);
            if (end.HasValue)
                q.SetDateTime("end", end.Value);
            if (personId.HasValue)
                q.SetInt32("personId", personId.Value);
            if (eventId.HasValue)
                q.SetInt32("eventId", eventId.Value);
            if (!string.IsNullOrEmpty(authorSearchText))
                q.SetString("authorSearchText", "%" + authorSearchText + "%");
            return q.List<SourceSearchResultDTO>();
        }

        protected string[] DataTableCols = { "C.Rank", "ISNULL(RS.IsRelevant, 1)", "ATTACHED_SOURCES.IsAttached", "S.SourceId", "S.SourceName", "S.FileDateTimeStamp", "S.IsRestricted", "READS.ReviewedDateTime" };

        protected string OrderBySQL(int iSortingCols, List<int> iSortCol, List<string> sSortDir, bool hasRank)
        {
            string sql = "";
            for (int i = 0; i < iSortingCols-1; i++)
            {
                if (iSortCol[i] != 0 || hasRank)
                {
                    sql += DataTableCols[iSortCol[i]] + " " + sSortDir[i] + ", ";
                }
            }
            if (iSortCol[iSortingCols - 1] != 0 || hasRank)
            {
                sql += DataTableCols[iSortCol[iSortingCols - 1]] + " " + sSortDir[iSortingCols - 1];
            }

            // if single column sort by rank selected, and no search text was given (i.e. no rank), default sort is:
            if (string.IsNullOrEmpty(sql))
                sql = DataTableCols[1];

            return sql;
        }
    }
}
