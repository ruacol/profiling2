using System;
using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Audit
{
    public interface IPersonRevisionsQuery
    {
        IList<Person> GetPersons(DateTime date, ProfileStatus profileStatus);
    }
}
