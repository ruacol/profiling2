using System.Collections.Generic;
using Profiling2.Domain.Prf.Careers;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface ICareerTasks
    {
        Career GetCareer(int id);

        Career SaveCareer(Career career);

        void DeleteCareer(Career career);

        IList<Career> GetCareersWithJob();

        IList<Career> GetCommanders();
    }
}
