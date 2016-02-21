using System.Collections.Generic;
using NHibernate;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Procs
{
    public class SuggestionEventForPersonQuery : NHibernateQuery, ISuggestionEventForPersonQuery
    {
        public int GetSuggestionTotal(int personId)
        {
            return Session.GetNamedQuery("PRF_SP_Suggestion_EventForPersonCount_NHibernate")
                .SetParameter("PersonID", personId, NHibernateUtil.Int32)
                .UniqueResult<int>();
        }

        public IList<SuggestionEventForPersonDTO> GetPaginatedResults(int iDisplayStart, int iDisplayLength, int personId)
        {
            return Session.GetNamedQuery("PRF_SP_Suggestion_EventForPerson")
                .SetParameter("PersonID", personId, NHibernateUtil.Int32)
                .SetParameter("MaximumRows", iDisplayLength, NHibernateUtil.Int32)
                .SetParameter("StartRowIndex", iDisplayStart, NHibernateUtil.Int32)
                .SetResultTransformer(Transformers.AliasToBean<SuggestionEventForPersonDTO>())
                .List<SuggestionEventForPersonDTO>();
        }
    }
}
