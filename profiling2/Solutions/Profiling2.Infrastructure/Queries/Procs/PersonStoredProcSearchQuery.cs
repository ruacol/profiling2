using System.Collections.Generic;
using NHibernate;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Infrastructure.Util;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Procs
{
    /// <summary>
    /// Reuses the PRF_Search_SearchForPerson stored procedure to conduct person searches.
    /// </summary>
    public class PersonStoredProcSearchQuery : NHibernateQuery, IPersonDataTablesQuery
    {
        public int GetSearchTotal(SearchTerm term, string username, bool includeRestrictedProfiles)
        {
            return Session.GetNamedQuery("PRF_SP_Search_SearchForPersonCount_NHibernate")
                .SetParameter("ExactName", term.FormattedExactName, NHibernateUtil.String)
                .SetParameter("PartialName", term.FormattedPartialName, NHibernateUtil.String)
                .SetParameter("MilitaryID", term.MilitaryId, NHibernateUtil.String)
                .SetParameter("AlternativeMilitaryID", null, NHibernateUtil.String)
                .SetParameter("RankID", term.RankId.HasValue ? term.RankId : null, NHibernateUtil.Int64)
                .SetParameter("RoleID", term.RoleId.HasValue ? term.RoleId : null, NHibernateUtil.Int64)
                .SetParameter("YearOfBirth", null, NHibernateUtil.Int32)
                .SetParameter("MonthOfBirth", null, NHibernateUtil.Int32)
                .SetParameter("DayOfBirth", null, NHibernateUtil.Int32)
                .SetParameter("UserID", username, NHibernateUtil.String)
                .SetParameter("Separator", ";", NHibernateUtil.String)
                .SetParameter("IncludeRestrictedProfiles", includeRestrictedProfiles, NHibernateUtil.Boolean)
                .UniqueResult<int>();
        }

        public IList<SearchForPersonDTO> GetPaginatedResults(int iDisplayStart, int iDisplayLength, SearchTerm term,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir, string username, bool includeRestrictedProfiles)
        {
            return Session.GetNamedQuery("PRF_SP_Search_SearchForPerson_NHibernate")
                .SetParameter("ExactName", term.FormattedExactName, NHibernateUtil.String)
                .SetParameter("PartialName", term.FormattedPartialName, NHibernateUtil.String)
                .SetParameter("MilitaryID", term.MilitaryId, NHibernateUtil.String)
                .SetParameter("AlternativeMilitaryID", null, NHibernateUtil.String)
                .SetParameter("RankID", term.RankId.HasValue ? term.RankId : null, NHibernateUtil.Int64)
                .SetParameter("RoleID", term.RoleId.HasValue ? term.RoleId : null, NHibernateUtil.Int64)
                .SetParameter("YearOfBirth", null, NHibernateUtil.Int32)
                .SetParameter("MonthOfBirth", null, NHibernateUtil.Int32)
                .SetParameter("DayOfBirth", null, NHibernateUtil.Int32)
                .SetParameter("UserID", username, NHibernateUtil.String)
                .SetParameter("Separator", ";", NHibernateUtil.String)
                .SetParameter("MaximumRows", iDisplayLength, NHibernateUtil.Int32)
                .SetParameter("StartRowIndex", iDisplayStart, NHibernateUtil.Int32)
                .SetParameter("IncludeRestrictedProfiles", includeRestrictedProfiles, NHibernateUtil.Boolean)
                .SetResultTransformer(Transformers.AliasToBean<SearchForPersonDTO>())
                .List<SearchForPersonDTO>();
         
        }
    }
}
