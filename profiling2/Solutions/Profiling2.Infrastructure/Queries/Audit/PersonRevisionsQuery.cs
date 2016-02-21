using System;
using System.Collections.Generic;
using System.Linq;
using KellermanSoftware.CompareNetObjects;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using Profiling2.Domain;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Persons;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class PersonRevisionsQuery : NHibernateAuditQuery, IPersonAuditable<Person>, IPersonRevisionsQuery
    {
        public new CompareLogic CompareLogic
        {
            get
            {
                base.CompareLogic.Config.MembersToIgnore.AddRange(new List<string>()
                    {
                        "Id", "ProfileLastModified", "AdminPersonImports", "PersonAliases", "PersonRelationshipAsSubject", "PersonRelationshipAsObject",  // Person 
                        "Name", "DateOfBirth", "PlaceOfBirth", "CurrentCareers", "CurrentCareer", "CurrentRank", "CurrentRankSortOrder",
                        "PersonPhotos", "PersonSources", "AdminSuggestionPersonResponsibilities", "ActionsTakenAsSubject", "ActionsTakenAsObject",
                        "PersonResponsibilities", "Careers", "AdminExportedPersonProfiles", "AdminReviewedSources", "RequestPersons",
                        "Careers", "OrganizationResponsibilities", "OrganizationRelationshipsAsSubject", "OrganizationRelationshipsAsObject"
                    });
                return base.CompareLogic;
            }
        }

        public IList<object[]> GetRawRevisions(Person person)
        {
            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(Person), false, true)
                .Add(AuditEntity.Id().Eq(person.Id))
                .GetResultList<object[]>();
        }

        public IList<AuditTrailDTO> GetRevisions(Person person)
        {
            IList<AuditTrailDTO> trail = this.GetAuditTrailDTOs<Person>(person);

            // Compares each dto with the one before it - makes sense when each dto refers to the same entity
            Entity last = null;
            for (int i = 0; i < trail.Count; i++)
            {
                trail[i].Differences = new List<Difference>();
                Entity current = trail[i].Entity as Entity;

                if (trail[i].REVINFO.Id != AuditableExtensions.BASE_REVISION_ID)  // don't show diffs for base revision
                {
                    ComparisonResult result = this.CompareLogic.Compare(current, last);
                    if (!result.AreEqual)
                        trail[i].Differences = trail[i].Differences.Concat(result.Differences).ToList();
                }

                last = current;
            }

            return trail;
        }

        public IList<Person> GetPersons(DateTime date, ProfileStatus profileStatus)
        {
            int revision = this.GetRevisionNumberForDate(date);

            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForEntitiesAtRevision(typeof(Person), Convert.ToInt64(revision))
                .Add(AuditEntity.Property("ProfileStatus").Eq(profileStatus))
                .Add(AuditEntity.Property("Archive").Eq(false))
                .GetResultList<Person>();
        }
    }
}
