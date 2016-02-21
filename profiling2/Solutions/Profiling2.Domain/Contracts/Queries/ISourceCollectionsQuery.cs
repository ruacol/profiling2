using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface ISourceCollectionsQuery
    {
        IList<AdminReviewedSource> GetReviewsForSource(int sourceId);

        IList<AdminSourceImport> GetAdminImportsForSource(int sourceId);

        IList<PersonSource> GetPersonSources(int sourceId);

        IList<EventSource> GetEventSources(int sourceId);

        IList<EventSource> SearchEventSources(string term);

        IList<UnitSource> GetUnitSources(int sourceId);

        IList<OperationSource> GetOperationSources(int sourceId);

        SourceRelationship GetParentSourceOf(int sourceId);

        IList<SourceRelationship> GetChildSourcesOf(int sourceId);
    }
}
