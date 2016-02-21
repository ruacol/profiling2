using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using log4net;
using NHibernate;
using NHibernate.Envers;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Persons;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public static class AuditableExtensions
    {
        // REVINFO.Id used for initial population of audit tables.
        public static int BASE_REVISION_ID = Convert.ToInt32(ConfigurationManager.AppSettings["BaseRevisionId"]);
        public static readonly ILog log = LogManager.GetLogger(typeof(AuditableExtensions));

        public static IList<AuditTrailDTO> GetAuditTrailDTOs<T>(this IPersonAuditable<T> auditable, Person target)
            where T : Entity
        {
            return (from row in auditable.GetRawRevisions(target)
                    select new AuditTrailDTO()
                    {
                        Entity = (T)row[0],
                        REVINFO = (REVINFO)row[1],
                        RevisionType = (RevisionType)row[2]
                    }).ToList<AuditTrailDTO>();
        }

        public static IList<AuditTrailDTO> GetRevisions<T>(this IPersonAuditable<T> auditable, Person person)
            where T : Entity
        {
            IList<AuditTrailDTO> trail = auditable.GetAuditTrailDTOs<T>(person)
                .OrderByDescending(x => x.REVINFO.REVTSTMP)
                .ToList<AuditTrailDTO>();

            for (int i = 0; i < trail.Count; i++)
            {
                AuditTrailDTO current = trail[i];

                // HACK: ignoring changesets made before the BASE_REVISION, because Envers will occasionally throw ObjectNotFoundExceptions when it
                // needs to refer to a base entity whose revision id is 100 (i.e. 'newer' than the current entity).  PROF2-191.
                if (current.REVINFO.Id >= BASE_REVISION_ID)
                {
                    // find previous entity change to compare it with
                    AuditTrailDTO last = null;
                    for (int j = i + 1; j < trail.Count; j++)
                    {
                        if (trail[j].Entity.Id == current.Entity.Id)
                        {
                            last = trail[j];
                            break;
                        }
                    }

                    try
                    {
                        if (current.REVINFO.Id != BASE_REVISION_ID && last != null)  // don't show diffs for base revision
                        {
                            ComparisonResult result = auditable.CompareLogic.Compare(current.Entity, last.Entity);
                            if (!result.AreEqual)
                                trail[i].Differences = trail[i].Differences.Concat(result.Differences).ToList();
                        }
                    }
                    catch (ObjectNotFoundException e)
                    {
                        AuditableExtensions.log.Error("PersonID " + person.Id
                            + ": Object not found comparing revision " + trail[i].REVINFO.Id + " with " + (last != null ? last.REVINFO.Id.ToString() : "none")
                            + " for " + current.Entity.ToString() + ": " + e.Message);
                    }
                }
                else
                    AuditableExtensions.log.Error("PersonID " + person.Id 
                        + ": Encountered audit revision " + current.REVINFO.Id + " < " + BASE_REVISION_ID + ", skipping (see PROF2-191).");
            }
            return trail;
        }
    }
}
