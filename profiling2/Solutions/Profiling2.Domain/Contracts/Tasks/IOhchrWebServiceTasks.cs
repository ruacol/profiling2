using System;
using System.Collections.Generic;
using HrdbWebServiceClient.Domain;
using NHibernate;

namespace Profiling2.Domain.Contracts.Tasks
{
    public interface IOhchrWebServiceTasks
    {
        void LogColumns();

        IDictionary<string, HrdbCase> GetAndPersistHrdbCasesQueueable();

        IDictionary<string, HrdbCase> GetAndPersistHrdbCases(DateTime from, DateTime to, ISession session);
    }
}
