using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NHibernate.Envers;
using NHibernate.Envers.Query;
using Profiling2.Domain.Contracts.Queries.Audit;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Infrastructure.Queries.Audit
{
    public class CareerRevisionsQuery : NHibernateAuditQuery, IPersonAuditable<Career>, IHistoricalCareerQuery
    {
        public new CompareLogic CompareLogic
        {
            get
            {
                // For some reason using ElementsToInclude won't compare child attributes past the first level.
                //this._compareObjects.ElementsToInclude = new List<string>() { 
                //    "Career", "Organization", "Location", "Rank", "Role", "Function", "Unit", "Job",
                //    "DayOfStart", "MonthOfStart", "YearOfStart", "DayOfEnd", "MonthOfEnd", "YearOfEnd",
                //    "Commentary", "Archive", "Notes", "DayAsOf", "MonthAsOf", "YearAsOf", 
                //    "IsCurrentCareer", "Defected", "Acting"
                //};
                //this._compareObjects.ShowBreadcrumb = true;

                base.CompareLogic.Config.MembersToIgnore.AddRange(new List<string>()
                    {
                        "Id", "Person", "AdminCareerImports",  // Career 
                        "Careers", "OrganizationResponsibilities", "OrganizationRelationshipsAsSubject", "OrganizationRelationshipsAsObject", 
                        "OrganizationAliases", "UnitHierarchies", "OrganizationPhotos",  // Organization
                        "Events",  // Location
                        "AdminUnitImports", "UnitHierarchyChildren"  // Unit
                    });
                return base.CompareLogic;
            }
        }

        public IList<object[]> GetRawRevisions(Person person)
        {
            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForRevisionsOfEntity(typeof(Career), false, true)
                .Add(AuditEntity.Property("Person").Eq(person))
                // don't process audit changes before baseline entities were initialised; some career.organization and career.unit
                // fields return as proxies and throw 'object not found' exceptions.
                .Add(AuditEntity.RevisionNumber().Ge(100))
                .GetResultList<object[]>();
        }

        public IList<Career> GetCareers(Person person, DateTime date)
        {
            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForEntitiesAtRevision(typeof(Career), GetRevisionNumberForDate(date))
                .Add(AuditEntity.Property("Person").Eq(person))
                .GetResultList<Career>();
        }

        public int GetCareerCount(DateTime date)
        {
            int revision = this.GetRevisionNumberForDate(date);

            return AuditReaderFactory.Get(Session).CreateQuery()
                .ForEntitiesAtRevision(typeof(Career), Convert.ToInt64(revision))
                .Add(AuditEntity.Property("Archive").Eq(false))
                .GetResultList().Count;
        }
    }
}
