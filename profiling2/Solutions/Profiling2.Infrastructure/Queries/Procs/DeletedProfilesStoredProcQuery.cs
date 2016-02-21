using System.Collections.Generic;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Procs
{
    public class DeletedProfilesStoredProcQuery : NHibernateQuery, IDeletedProfilesStoredProcQuery
    {
        public IList<DeletedProfilesAuditDTO> GetRows()
        {
            return Session.GetNamedQuery("PRF_SP_Reports_DeletedProfiles_NHibernate")
                .SetResultTransformer(Transformers.AliasToBean<DeletedProfilesAuditDTO>())
                .List<DeletedProfilesAuditDTO>();
        }
    }
}
