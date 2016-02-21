using System.Collections.Generic;
using System.Linq;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Tasks
{
    public class WantedTasks : IWantedTasks
    {
        protected readonly IPersonTasks personTasks;
        protected readonly ICareerTasks careerTasks;
        protected readonly IActionTakenTasks actionTasks;

        public WantedTasks(IPersonTasks personTasks,
            ICareerTasks careerTasks,
            IActionTakenTasks actionTasks)
        {
            this.personTasks = personTasks;
            this.careerTasks = careerTasks;
            this.actionTasks = actionTasks;
        }

        public IList<Person> GetWantedCommanders()
        {
            IList<Person> commanders = this.careerTasks.GetCommanders().Select(x => x.Person).Distinct().ToList<Person>();
            IList<Person> wantedPersonsByBackground = this.personTasks.GetPersonsWanted();
            IList<Person> wantedSubjectsByAction = this.actionTasks.GetWantedActionsTaken().Select(x => x.SubjectPerson).ToList<Person>();
            IList<Person> wantedObjectsByAction = this.actionTasks.GetWantedActionsTaken().Select(x => x.ObjectPerson).ToList<Person>();

            return commanders.Intersect(
                    wantedPersonsByBackground
                    .Concat(wantedSubjectsByAction)
                    .Concat(wantedObjectsByAction)
                )
                .Distinct()
                .ToList<Person>();
        }
    }
}
