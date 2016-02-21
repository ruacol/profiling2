using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Actions;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class ActionTakenTasks : IActionTakenTasks
    {
        protected readonly INHibernateRepository<ActionTakenType> actionTakenTypeRepo;
        protected readonly INHibernateRepository<ActionTaken> actionTakenRepo;
        protected readonly IActionTakenWantedQuery wantedQuery;
        protected readonly IPersonTasks personTasks;

        public ActionTakenTasks(INHibernateRepository<ActionTakenType> actionTakenTypeRepo,
            INHibernateRepository<ActionTaken> actionTakenRepo,
            IActionTakenWantedQuery wantedQuery,
            IPersonTasks personTasks)
        {
            this.actionTakenTypeRepo = actionTakenTypeRepo;
            this.actionTakenRepo = actionTakenRepo;
            this.wantedQuery = wantedQuery;
            this.personTasks = personTasks;
        }

        public IList<ActionTakenType> GetActionTakenTypes()
        {
            return this.actionTakenTypeRepo.GetAll().OrderBy(x => x.ActionTakenTypeName).ToList<ActionTakenType>();
        }

        public IList<ActionTakenType> GetActionTakenTypesByName(string term)
        {
            return this.wantedQuery.GetActionTakenTypes(term);
        }

        public ActionTakenType GetActionTakenType(int id)
        {
            return this.actionTakenTypeRepo.Get(id);
        }

        public ActionTakenType SaveActionTakenType(ActionTakenType type)
        {
            return this.actionTakenTypeRepo.SaveOrUpdate(type);
        }

        public bool DeleteActionTakenType(ActionTakenType type)
        {
            if (type != null && type.ActionsTaken.Count == 0)
            {
                this.actionTakenTypeRepo.Delete(type);
                return true;
            }
            return false;
        }

        public ActionTaken GetActionTaken(int id)
        {
            return this.actionTakenRepo.Get(id);
        }

        public ActionTaken SaveActionTaken(ActionTaken at)
        {
            at.Event.AddActionTaken(at);

            foreach (Person person in new Person[] { at.ObjectPerson, at.SubjectPerson })
            {
                if (person != null)
                {
                    if (!person.HasValidProfileStatus())
                        person.ProfileStatus = this.personTasks.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
                    this.personTasks.SavePerson(person);  // also updates lucene
                }
            }

            return this.actionTakenRepo.SaveOrUpdate(at);
        }

        public void DeleteActionTaken(ActionTaken at)
        {
            at.Event.RemoveActionTaken(at);
            this.actionTakenRepo.Delete(at);
        }

        public IList<ActionTaken> GetWantedActionsTaken()
        {
            return this.wantedQuery.GetWantedActionsTaken();
        }
    }
}
