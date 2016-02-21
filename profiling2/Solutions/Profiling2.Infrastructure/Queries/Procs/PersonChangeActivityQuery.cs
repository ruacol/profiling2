using System.Collections.Generic;
using NHibernate;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Procs
{
    public class PersonChangeActivityQuery : NHibernateQuery, IPersonChangeActivityQuery
    {
        public IList<PersonChangeActivityDTO> GetRevisions(int personId)
        {
            return Session.GetNamedQuery("PRF_SP_Summaries_ProfileChangeActivity_NHibernate")
                .SetParameter("PersonID", personId, NHibernateUtil.Int32)
                .SetResultTransformer(Transformers.AliasToBean<PersonChangeActivityDTO>())
                .List<PersonChangeActivityDTO>();
        }
    }
}
