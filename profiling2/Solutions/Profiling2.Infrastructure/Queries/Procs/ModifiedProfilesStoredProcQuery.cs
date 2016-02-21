using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Procs
{
    public class ModifiedProfilesStoredProcQuery : NHibernateQuery, IModifiedProfilesStoredProcQuery
    {
        public IList<ModifiedProfilesAuditDTO> GetRows(DateTime startDate, DateTime endDate)
        {
            return Session.GetNamedQuery("PRF_SP_Reports_ProfilesModifiedLastWeek_NHibernate")
                .SetParameter("StartDate", startDate, NHibernateUtil.DateTime)
                .SetParameter("EndDate", endDate, NHibernateUtil.DateTime)
                .SetResultTransformer(Transformers.AliasToBean<ModifiedProfilesAuditDTO>())
                .List<ModifiedProfilesAuditDTO>();
        }
    }
}
