using System;
using System.Collections.Generic;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Audit
{
    public interface IHistoricalCareerQuery
    {
        IList<Career> GetCareers(Person person, DateTime date);

        int GetCareerCount(DateTime date);
    }
}
