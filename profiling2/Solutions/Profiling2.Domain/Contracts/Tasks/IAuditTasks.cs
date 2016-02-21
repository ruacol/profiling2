using System;
using System.Collections.Generic;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IAuditTasks
    {
        IList<PersonChangeActivityDTO> GetPersonOldAuditTrail(int personId);

        IList<AuditTrailDTO> GetPersonAuditTrail(int personId);

        IList<ChangeActivityDTO> GetEventOldAuditTrail(int eventId);

        IList<AuditTrailDTO> GetEventAuditTrail(int eventId);

        DateTime GetEventLastModified(int eventId);

        IList<AuditTrailDTO> GetUnitAuditTrail(int unitId);

        IList<ChangeActivityDTO> GetUnitOldAuditTrail(int unitId);

        IList<AuditTrailDTO> GetOperationAuditTrail(int operationId);

        IList<DeletedProfilesAuditDTO> GetOldDeletedProfiles();

        IList<object[]> GetDeletedProfiles();

        IList<object[]> GetCreatedProfiles(DateTime startDate, DateTime endDate);

        IList<ModifiedProfilesAuditDTO> GetModifiedProfiles(DateTime startDate, DateTime endDate);

        IList<Career> GetHistoricalCurrentCareers(Person person, DateTime date);

        IList<Person> GetPersons(DateTime date, ProfileStatus profileStatus);
    }
}
