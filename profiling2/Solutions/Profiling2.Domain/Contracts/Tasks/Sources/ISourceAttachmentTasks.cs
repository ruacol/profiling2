using System.Collections.Generic;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;

namespace Profiling2.Domain.Contracts.Tasks.Sources
{
    public interface ISourceAttachmentTasks
    {
        AdminSourceSearch GetAdminSourceSearch(int adminSourceSearchId);

        AdminSourceSearch SaveOrUpdateAdminSourceSearch(AdminSourceSearch adminSourceSearch);

        IList<int> GetAdminSourceSearchIds(AdminSourceSearch adminSourceSearch);

        AdminReviewedSource GetOrCreateAdminReviewedSource(int sourceId, int adminSourceSearchId);

        IList<AdminReviewedSource> GetReviewsForSource(int sourceId);

        IList<AdminSourceImport> GetAdminImportsForSource(int sourceId);

        SourceRelationship GetParentSourceOf(int sourceId);

        IList<SourceRelationship> GetChildSourcesOf(int sourceId);

        PersonSource GetPersonSource(int id);

        IList<PersonSource> GetPersonSources(int sourceId);

        IList<PersonSource> GetPersonSources(Person person, Source source);

        PersonSource SavePersonSource(PersonSource ps);

        void DeletePersonSource(int id);

        EventSource GetEventSource(int id);

        IList<EventSource> GetEventSources(int sourceId);

        IList<EventSource> GetEventSources(Event ev, Source source);

        IList<EventSource> SearchEventSources(string term);

        EventSource SaveEventSource(EventSource es);

        void DeleteEventSource(int id);

        IList<OrganizationSource> GetOrganizationSources(Organization org, Source source);

        OrganizationSource SaveOrganizationSource(OrganizationSource os);

        void DeleteOrganizationSource(int id);

        UnitSource GetUnitSource(int id);

        IList<UnitSource> GetUnitSources(int sourceId);

        UnitSource SaveUnitSource(UnitSource us);

        void DeleteUnitSource(int id);

        OperationSource GetOperationSource(int id);

        IList<OperationSource> GetOperationSources(int sourceId);

        OperationSource SaveOperationSource(OperationSource us);

        void DeleteOperationSource(int id);
    }
}
