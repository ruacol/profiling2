using System;
using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Events;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IEventTasks
    {
        Event GetEvent(int id);

        IList<Event> GetAllEvents();

        IList<Event> GetAllEvents(ISession session);

        IList<Event> GetEvents(string term);

        Event SaveEvent(Event e);

        bool DeleteEvent(Event e);

        IList<Event> SearchEventNotes(string term);

        Violation GetViolation(int id);

        Violation GetViolation(string name);

        int GetViolationDataTablesCount(string term);

        IList<ViolationDataTableView> GetViolationDataTablesPaginated(int iDisplayStart, int iDisplayLength, string searchText,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir);

        Violation SaveViolation(Violation v);

        void DeleteViolation(Violation v);

        IList<Violation> GetViolations();

        IList<Violation> GetRootParentViolations();

        IList<object> GetViolationsJson(int eventId);

        IList<object> GetViolationsJson(string term);

        IList<object> GetViolationsJson(string term, int eventId);

        IDictionary<Violation, int> ScoreViolations(string eventName, string[] eventNameSeparators);

        IList<EventRelationshipType> GetEventRelationshipTypes();

        EventRelationshipType GetEventRelationshipType(int id);

        EventRelationship GetEventRelationship(int id);

        EventRelationship SaveEventRelationship(EventRelationship eventRelationship);

        void DeleteEventRelationship(EventRelationship relationship);

        /// <summary>
        /// Stored procedure from Profiling1.
        /// </summary>
        /// <param name="toKeepEventId"></param>
        /// <param name="toDeleteEventId"></param>
        /// <param name="userId"></param>
        /// <param name="isProfilingChange"></param>
        /// <returns>Returns 1 on success.</returns>
        int MergeEvents(int toKeepEventId, int toDeleteEventId, string userId, bool isProfilingChange);

        byte[] ExportDocument(Event e, DateTime lastModifiedDate, AdminUser user, string clientDnsName, string clientIpAddress, string clientUserAgent);

        void ApproveEvent(Event e, AdminUser user);

        IList<Event> GetUnapprovedEvents();

        IList<Tag> GetAllTags();

        Tag GetTag(int id);

        Tag GetTag(string tagName);

        Tag SaveTag(Tag tag);

        void DeleteTag(Tag tag);

        IList<Tag> SearchTags(string term);

        IList<EventVerifiedStatus> GetAllEventVerifiedStatuses();

        EventVerifiedStatus GetEventVerifiedStatus(int id);

        EventVerifiedStatus GetEventVerifiedStatus(string name);
    }
}
