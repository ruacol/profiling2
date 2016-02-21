using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using NHibernate.Envers;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Contracts.Queries.Procs;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using Profiling2.Infrastructure.Queries.Audit;
using StackExchange.Profiling;

namespace Profiling2.Tasks
{
    public class AuditTasks : IAuditTasks
    {
        protected readonly ILog log = LogManager.GetLogger(typeof(AuditTasks));

        protected readonly IPersonTasks personTasks;
        protected readonly IEventTasks eventTasks;
        protected readonly IOrganizationTasks orgTasks;
        protected readonly IPersonChangeActivityQuery personChangeActivityQuery;
        protected readonly IPersonAuditable<Person> auditPersonQuery;
        protected readonly IPersonAuditable<Career> auditCareerQuery;
        protected readonly IPersonAuditable<PersonAlias> auditPersonAliasQuery;
        protected readonly IPersonAuditable<PersonSource> auditPersonSourceQuery;
        protected readonly IPersonAuditable<PersonPhoto> auditPersonPhotoQuery;
        protected readonly IPersonAuditable<PersonRelationship> auditPersonRelationshipQuery;
        protected readonly IPersonAuditable<ActionTaken> auditActionTakenQuery;
        protected readonly IPersonAuditable<PersonResponsibility> auditPersonResponsibilityQuery;
        protected readonly IPersonAuditable<PersonRestrictedNote> auditPersonRestrictedNoteQuery;
        protected readonly IDeletedProfilesStoredProcQuery oldDeletedProfilesQuery;
        protected readonly IAuditedEntityQuery auditedEntityQuery;
        protected readonly IModifiedProfilesStoredProcQuery oldModifiedProfilesQuery;
        protected readonly IAdminAuditQuery adminAuditQuery;
        protected readonly IEventRevisionsQuery eventRevisionsQuery;
        protected readonly IUnitRevisionsQuery unitRevisionsQuery;
        protected readonly IOperationRevisionsQuery operationRevisionsQuery;

        public AuditTasks(IPersonTasks personTasks,
            IEventTasks eventTasks,
            IOrganizationTasks orgTasks,
            IPersonChangeActivityQuery personChangeActivityQuery,
            IPersonAuditable<Person> auditPersonQuery,
            IPersonAuditable<Career> auditCareerQuery,
            IPersonAuditable<PersonAlias> auditPersonAliasQuery,
            IPersonAuditable<PersonSource> auditPersonSourceQuery,
            IPersonAuditable<PersonPhoto> auditPersonPhotoQuery,
            IPersonAuditable<PersonRelationship> auditPersonRelationshipQuery,
            IPersonAuditable<ActionTaken> auditActionTakenQuery,
            IPersonAuditable<PersonResponsibility> auditPersonResponsibilityQuery,
            IPersonAuditable<PersonRestrictedNote> auditPersonRestrictedNoteQuery,
            IDeletedProfilesStoredProcQuery oldDeletedProfilesQuery,
            IAuditedEntityQuery auditedEntityQuery,
            IModifiedProfilesStoredProcQuery oldModifiedProfilesQuery,
            IAdminAuditQuery adminAuditQuery,
            IEventRevisionsQuery eventRevisionsQuery,
            IUnitRevisionsQuery unitRevisionsQuery,
            IOperationRevisionsQuery operationRevisionsQuery)
        {
            this.personTasks = personTasks;
            this.eventTasks = eventTasks;
            this.orgTasks = orgTasks;
            this.personChangeActivityQuery = personChangeActivityQuery;
            this.auditPersonQuery = auditPersonQuery;
            this.auditCareerQuery = auditCareerQuery;
            this.auditPersonAliasQuery = auditPersonAliasQuery;
            this.auditPersonSourceQuery = auditPersonSourceQuery;
            this.auditPersonPhotoQuery = auditPersonPhotoQuery;
            this.auditPersonRelationshipQuery = auditPersonRelationshipQuery;
            this.auditActionTakenQuery = auditActionTakenQuery;
            this.auditPersonResponsibilityQuery = auditPersonResponsibilityQuery;
            this.auditPersonRestrictedNoteQuery = auditPersonRestrictedNoteQuery;
            this.oldDeletedProfilesQuery = oldDeletedProfilesQuery;
            this.auditedEntityQuery = auditedEntityQuery;
            this.oldModifiedProfilesQuery = oldModifiedProfilesQuery;
            this.adminAuditQuery = adminAuditQuery;
            this.eventRevisionsQuery = eventRevisionsQuery;
            this.unitRevisionsQuery = unitRevisionsQuery;
            this.operationRevisionsQuery = operationRevisionsQuery;
        }

        public IList<PersonChangeActivityDTO> GetPersonOldAuditTrail(int personId)
        {
            return this.personChangeActivityQuery.GetRevisions(personId);
        }

        public IList<AuditTrailDTO> GetPersonAuditTrail(int personId)
        {
            Person person = this.personTasks.GetPerson(personId);
            IList<AuditTrailDTO> personTrail = this.auditPersonQuery.GetRevisions(person);
            IList<AuditTrailDTO> careerTrail = this.auditCareerQuery.GetRevisions(person);
            IList<AuditTrailDTO> personAliasTrail = this.auditPersonAliasQuery.GetRevisions(person);
            IList<AuditTrailDTO> personSourceTrail = this.auditPersonSourceQuery.GetRevisions(person);
            IList<AuditTrailDTO> personPhotoTrail = this.auditPersonPhotoQuery.GetRevisions(person);
            IList<AuditTrailDTO> personRelationshipTrail = this.auditPersonRelationshipQuery.GetRevisions(person);
            IList<AuditTrailDTO> actionTakenTrail = this.auditActionTakenQuery.GetRevisions(person);
            IList<AuditTrailDTO> personResponsibilityTrail = this.auditPersonResponsibilityQuery.GetRevisions(person);
            IList<AuditTrailDTO> personRestrictedNoteTrail = this.auditPersonRestrictedNoteQuery.GetRevisions(person);

            return personTrail
                .Concat(careerTrail)
                .Concat(personAliasTrail)
                .Concat(personSourceTrail)
                .Concat(personPhotoTrail)
                .Concat(personRelationshipTrail)
                .Concat(actionTakenTrail)
                .Concat(personResponsibilityTrail)
                .Concat(personRestrictedNoteTrail)
                .Where(x => x.REVINFO.Id != AuditableExtensions.BASE_REVISION_ID)
                .OrderByDescending(x => x.REVINFO.REVTSTMP)
                .ToList<AuditTrailDTO>();
        }

        public IList<ChangeActivityDTO> GetEventOldAuditTrail(int eventId)
        {
            return this.eventRevisionsQuery.GetOldEventAuditTrail(eventId);
        }

        public IList<AuditTrailDTO> GetEventAuditTrail(int eventId)
        {
            Event e = this.eventTasks.GetEvent(eventId);

            return this.eventRevisionsQuery.GetEventRevisions(e)
                .Concat(this.eventRevisionsQuery.GetEventCollectionRevisions<PersonResponsibility>(e))
                .Concat(this.eventRevisionsQuery.GetEventCollectionRevisions<OrganizationResponsibility>(e))
                .Concat(this.eventRevisionsQuery.GetEventCollectionRevisions<EventSource>(e))
                .Concat(this.eventRevisionsQuery.GetEventCollectionRevisions<ActionTaken>(e))
                .Concat(this.eventRevisionsQuery.GetEventRelationshipRevisions(e))
                .OrderByDescending(x => x.REVINFO.REVTSTMP)
                .ToList<AuditTrailDTO>();
        }

        public DateTime GetEventLastModified(int eventId)
        {
            IList<AuditTrailDTO> trail = this.GetEventAuditTrail(eventId);
            if (trail != null && trail.Any())
                if (trail.First(x => x.REVINFO != null && x.REVINFO.REVTSTMP.HasValue) != null)
                    return trail.First(x => x.REVINFO != null && x.REVINFO.REVTSTMP.HasValue).REVINFO.REVTSTMP.Value;

            IList<ChangeActivityDTO> oldTrail = this.GetEventOldAuditTrail(eventId);
            if (oldTrail != null && oldTrail.Any())
                return oldTrail.OrderByDescending(x => x.When).First().When;

            Event e = this.eventTasks.GetEvent(eventId);
            if (e != null && e.Created.HasValue)
                return e.Created.Value;

            return new DateTime();
        }

        public IList<AuditTrailDTO> GetUnitAuditTrail(int unitId)
        {
            Unit u = this.orgTasks.GetUnit(unitId);

            return this.unitRevisionsQuery.GetUnitRevisions(u)
                .Concat(this.unitRevisionsQuery.GetUnitCollectionRevisions<UnitAlias>(u))
                .Concat(this.unitRevisionsQuery.GetUnitCollectionRevisions<UnitOperation>(u))
                .Concat(this.unitRevisionsQuery.GetUnitCollectionRevisions<UnitSource>(u))
                .OrderByDescending(x => x.REVINFO.REVTSTMP)
                .ToList<AuditTrailDTO>();
        }

        public IList<ChangeActivityDTO> GetUnitOldAuditTrail(int unitId)
        {
            return this.unitRevisionsQuery.GetOldUnitAuditTrail(unitId);
        }

        public IList<AuditTrailDTO> GetOperationAuditTrail(int operationId)
        {
            Operation op = this.orgTasks.GetOperation(operationId);

            return this.operationRevisionsQuery.GetOperationRevisions(op)
                .Concat(this.operationRevisionsQuery.GetOperationCollectionRevisions<OperationAlias>(op))
                .Concat(this.operationRevisionsQuery.GetOperationCollectionRevisions<UnitOperation>(op))
                .Concat(this.operationRevisionsQuery.GetOperationCollectionRevisions<OperationSource>(op))
                .OrderByDescending(x => x.REVINFO.REVTSTMP)
                .ToList<AuditTrailDTO>();
        }

        public IList<DeletedProfilesAuditDTO> GetOldDeletedProfiles()
        {
            return this.oldDeletedProfilesQuery.GetRows();
        }

        public IList<object[]> GetDeletedProfiles()
        {
            return this.auditedEntityQuery.GetRawRevisions(typeof(Person), RevisionType.Deleted, null, null);
        }

        public IList<object[]> GetCreatedProfiles(DateTime startDate, DateTime endDate)
        {
            var profiler = MiniProfiler.Current;

            // Get audit records from old PRF_AdminAudit table
            IList<object[]> list = new List<object[]>();
            using (profiler.Step("PRF_AdminAudit created query"))
            {
                foreach (AdminAudit aa in this.adminAuditQuery.GetAdminAudits("Person", AdminAuditType.ID_INSERT, startDate, endDate))
                {
                    Person p = this.personTasks.GetPerson(aa.ChangedRecordID);  // person could have been deleted
                    list.Add(new object[]
                {
                    null,  // the Person object here doesn't represent the Person object at the time of the audit revision
                    new REVINFO()
                    {
                        UserName = aa.AdminUser.UserName,
                        REVTSTMP = aa.ChangedDateTime
                    },
                    aa.ChangedRecordID,
                    p != null ? p.Name : string.Empty
                });
                }
            }

            // Combine with current audit records from PRF_Person_AUD
            IList<object[]> currentAuditRecords;
            using (profiler.Step("PRF_Person_AUD created via envers"))
                currentAuditRecords = this.auditedEntityQuery.GetRawRevisions(typeof(Person), RevisionType.Added, startDate, endDate);

            return list.Concat(currentAuditRecords).ToList();
        }

        public IList<ModifiedProfilesAuditDTO> GetModifiedProfiles(DateTime startDate, DateTime endDate)
        {
            var profiler = MiniProfiler.Current;

            IList<ModifiedProfilesAuditDTO> newAudits;
            using (profiler.Step("PRF_Person_AUD modified via envers"))
            {
                IList<object[]> rows = this.auditedEntityQuery.GetRawRevisions(typeof(Person), RevisionType.Modified, startDate, endDate);
                newAudits = (from row in rows
                             select new ModifiedProfilesAuditDTO()
                             {
                                 Who = ((REVINFO)row[1]).UserName,
                                 When = ((REVINFO)row[1]).REVTSTMP.Value,
                                 PersonID = ((Person)row[0]).Id.ToString(),
                                 Person = ((Person)row[0]).Name,
                                 Status = ((Person)row[0]).ProfileStatus.ToString()
                             }).ToList();
            }

            IList<ModifiedProfilesAuditDTO> oldAudits;
            using (profiler.Step("PRF_AdminAudit modified via proc"))
                oldAudits = this.oldModifiedProfilesQuery.GetRows(startDate, endDate);

            IDictionary<string, ModifiedProfilesAuditDTO> filtered = new Dictionary<string, ModifiedProfilesAuditDTO>();
            foreach (ModifiedProfilesAuditDTO dto in newAudits.Concat(oldAudits))
            {
                // use most recent modified entity in filtered list
                if (!filtered.Keys.Contains(dto.PersonID))
                    filtered[dto.PersonID] = dto;
                else if (filtered[dto.PersonID].When < dto.When)
                    filtered[dto.PersonID] = dto;
            }

            return filtered.Values.OrderByDescending(x => x.When).ToList();
        }

        /// <summary>
        /// Return a person's current career/s as of given date.  Note this represents our knowledge at the time of the given date,
        /// not our *current* knowledge of the person's careers at the given date.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public IList<Career> GetHistoricalCurrentCareers(Person person, DateTime date)
        {
            // all person's known careers at given date
            IList<Career> careers = ((IHistoricalCareerQuery)this.auditCareerQuery).GetCareers(person, date);

            // return careers current as of given date
            return careers.Where(x => !x.Archive && x.IsCurrentCareer).ToList<Career>();
        }

        public IList<Person> GetPersons(DateTime date, ProfileStatus profileStatus)
        {
            return ((IPersonRevisionsQuery)this.auditPersonQuery).GetPersons(date, profileStatus);
        }
    }
}
