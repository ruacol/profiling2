using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Domain.Contracts.Queries
{
    public interface IJhroCaseQueries
    {
        JhroCase GetJhroCase(ISession session, string caseNumber);

        void SaveJhroCase(ISession session, JhroCase jhroCase);

        IList<JhroCase> SearchJhroCases(string term);
    }
}
