using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate.Contracts.Repositories;

namespace Profiling2.Tasks
{
    public class CareerTasks : ICareerTasks
    {
        protected readonly INHibernateRepository<Career> careerRepository;
        protected readonly ICommandersQuery commandersQuery;
        protected readonly ILuceneTasks luceneTasks;
        protected readonly IPersonTasks personTasks;

        public CareerTasks(INHibernateRepository<Career> careerRepository, ICommandersQuery commandersQuery, ILuceneTasks luceneTasks, IPersonTasks personTasks)
        {
            this.careerRepository = careerRepository;
            this.commandersQuery = commandersQuery;
            this.luceneTasks = luceneTasks;
            this.personTasks = personTasks;
        }

        public Career GetCareer(int id)
        {
            return this.careerRepository.Get(id);
        }

        public Career SaveCareer(Career career)
        {
            career.Person.AddCareer(career);

            if (!career.Person.HasValidProfileStatus())
                career.Person.ProfileStatus = this.personTasks.GetProfileStatus(ProfileStatus.ROUGH_OUTLINE);
            this.personTasks.SavePerson(career.Person);  // updates lucene

            return this.careerRepository.SaveOrUpdate(career);
        }

        public void DeleteCareer(Career career)
        {
            career.Person.RemoveCareer(career);

            // queue update to Person index
            BackgroundJob.Enqueue<IPersonTasks>(x => x.LuceneUpdatePersonQueueable(career.Person.Id));

            this.careerRepository.Delete(career);
        }

        public IList<Career> GetCareersWithJob()
        {
            return (from career in this.careerRepository.GetAll()
             //where career.Role != null || !string.IsNullOrEmpty(career.Job) || career.Unit != null
                    where !string.IsNullOrEmpty(career.Job)
                    select career).ToList<Career>();
        }

        public IList<Career> GetCommanders()
        {
            return this.commandersQuery.GetCommanders();
        }
    }
}
