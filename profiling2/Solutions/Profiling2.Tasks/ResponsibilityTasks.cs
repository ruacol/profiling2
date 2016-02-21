using System.Collections.Generic;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class ResponsibilityTasks : IResponsibilityTasks
    {
        protected readonly INHibernateRepository<PersonResponsibility> personResponsibilityRepo;
        protected readonly INHibernateRepository<PersonResponsibilityType> personResponsibilityTypeRepo;
        protected readonly INHibernateRepository<OrganizationResponsibility> orgResponsibilityRepo;
        protected readonly INHibernateRepository<OrganizationResponsibilityType> orgResponsibilityTypeRepo;
        protected readonly IPersonTasks personTasks;

        public ResponsibilityTasks(INHibernateRepository<PersonResponsibility> personResponsibilityRepo,
            INHibernateRepository<PersonResponsibilityType> personResponsibilityTypeRepo,
            INHibernateRepository<OrganizationResponsibility> orgResponsibilityRepo,
            INHibernateRepository<OrganizationResponsibilityType> orgResponsibilityTypeRepo,
            IPersonTasks personTasks)
        {
            this.personResponsibilityRepo = personResponsibilityRepo;
            this.personResponsibilityTypeRepo = personResponsibilityTypeRepo;
            this.orgResponsibilityRepo = orgResponsibilityRepo;
            this.orgResponsibilityTypeRepo = orgResponsibilityTypeRepo;
            this.personTasks = personTasks;
        }

        public IList<PersonResponsibility> GetAllPersonResponsibilities()
        {
            return this.personResponsibilityRepo.GetAll();
        }

        public PersonResponsibility GetPersonResponsibility(int id)
        {
            return this.personResponsibilityRepo.Get(id);
        }

        public PersonResponsibility GetPersonResponsibility(Event e, Person p)
        {
            IDictionary<string, object> criteria = new Dictionary<string, object>();
            criteria.Add("Event", e);
            criteria.Add("Person", p);
            IList<PersonResponsibility> results = this.personResponsibilityRepo.FindAll(criteria);
            if (results != null && results.Count > 0)
                return results[0];
            return null;
        }

        public void DeletePersonResponsibility(PersonResponsibility pr)
        {
            pr.Event.RemovePersonResponsibility(pr);
            foreach (Violation v in pr.Violations)
                v.RemovePersonResponsibility(pr);
            pr.Person.RemovePersonResponsibility(pr);
            this.personResponsibilityRepo.Delete(pr);
        }

        public PersonResponsibility SavePersonResponsibility(PersonResponsibility pr)
        {
            pr.Event.AddPersonResponsibility(pr);
            foreach (Violation v in pr.Violations)
                v.AddPersonResponsibility(pr);
            pr.Person.AddPersonResponsibility(pr);

            if (!pr.Person.HasValidProfileStatus())
            {
                pr.Person.ProfileStatus = this.personTasks.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                this.personTasks.SavePerson(pr.Person);
            }

            return this.personResponsibilityRepo.SaveOrUpdate(pr);
        }

        public PersonResponsibilityType GetPersonResponsibilityType(int id)
        {
            return this.personResponsibilityTypeRepo.Get(id);
        }

        public IList<PersonResponsibilityType> GetPersonResponsibilityTypes()
        {
            return this.personResponsibilityTypeRepo.GetAll();
        }

        public PersonResponsibilityType SavePersonResponsibilityType(PersonResponsibilityType type)
        {
            return this.personResponsibilityTypeRepo.SaveOrUpdate(type);
        }

        public OrganizationResponsibility GetOrganizationResponsibility(int id)
        {
            return this.orgResponsibilityRepo.Get(id);
        }

        public OrganizationResponsibility SaveOrganizationResponsibility(OrganizationResponsibility or)
        {
            if (or != null)
            {
                if (or.Event != null)
                    or.Event.AddOrganizationResponsibility(or);
                if (or.Organization != null)
                    or.Organization.AddOrganizationResponsibility(or);
                if (or.Unit != null)
                    or.Unit.AddOrganizationResponsibility(or);
                return this.orgResponsibilityRepo.SaveOrUpdate(or);
            }
            return or;
        }

        public void DeleteOrganizationResponsibility(OrganizationResponsibility or)
        {
            if (or.Event != null)
                or.Event.RemoveOrganizationResponsibility(or);
            if (or.Organization != null)
                or.Organization.RemoveOrganizationResponsibility(or);
            if (or.Unit != null)
                or.Unit.RemoveOrganizationResponsibility(or);
            this.orgResponsibilityRepo.Delete(or);
        }

        public OrganizationResponsibilityType GetOrgResponsibilityType(int id)
        {
            return this.orgResponsibilityTypeRepo.Get(id);
        }

        public IList<OrganizationResponsibilityType> GetOrgResponsibilityTypes()
        {
            return this.orgResponsibilityTypeRepo.GetAll();
        }
    }
}
