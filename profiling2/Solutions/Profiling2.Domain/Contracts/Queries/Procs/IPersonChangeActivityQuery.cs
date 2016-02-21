﻿using System.Collections.Generic;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Domain.Contracts.Queries.Procs
{
    public interface IPersonChangeActivityQuery
    {
        IList<PersonChangeActivityDTO> GetRevisions(int personId);
    }
}
