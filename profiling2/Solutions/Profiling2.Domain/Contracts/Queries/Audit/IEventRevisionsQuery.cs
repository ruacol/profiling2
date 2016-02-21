using System;
using System.Collections.Generic;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Domain.Contracts.Queries.Audit
{
    public interface IEventRevisionsQuery
    {
        IList<AuditTrailDTO> GetEventRevisions(Event e);

        IList<AuditTrailDTO> GetEventCollectionRevisions<T>(Event e);

        IList<AuditTrailDTO> GetEventRelationshipRevisions(Event e);

        IList<ChangeActivityDTO> GetOldEventAuditTrail(int eventId);

        int GetEventCount(DateTime date);
    }
}
