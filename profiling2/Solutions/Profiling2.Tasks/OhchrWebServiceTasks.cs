using System;
using System.Collections.Generic;
using HrdbWebServiceClient.Contracts;
using HrdbWebServiceClient.Domain;
using log4net;
using NHibernate;
using Profiling2.Domain.Contracts.Tasks;
using Profiling2.Domain.Contracts.Tasks.Sources;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Infrastructure.Util;
using SharpArch.NHibernate;
using StackExchange.Profiling;

namespace Profiling2.Tasks
{
    public class OhchrWebServiceTasks : IOhchrWebServiceTasks
    {
        protected static ILog log = LogManager.GetLogger(typeof(OhchrWebServiceTasks));

        protected readonly IHrdbEntitiesRepository hrdbRepo;
        protected readonly ISourceTasks sourceTasks;

        public OhchrWebServiceTasks(ISourceTasks sourceTasks, 
            IHrdbEntitiesRepository hrdbRepo)
        {
            this.hrdbRepo = hrdbRepo;
            this.sourceTasks = sourceTasks;
        }

        public void LogColumns()
        {
            this.hrdbRepo.LogOhchrCaseColumns(DateTime.Now.Subtract(TimeSpan.FromDays(1)), DateTime.Now);
            this.hrdbRepo.LogViolationColumns();
            this.hrdbRepo.LogAffiliationColumns();
            this.hrdbRepo.LogAuthorityResponseColumns(DateTime.Now.Subtract(TimeSpan.FromDays(1)), DateTime.Now);
            this.hrdbRepo.LogIndividualPerpetratorColumns();
        }

        public IDictionary<string, HrdbCase> GetAndPersistHrdbCasesQueueable()
        {
            IDictionary<string, HrdbCase> dict; 

            using (ISession session = NHibernateSession.GetDefaultSessionFactory().OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    dict = this.GetAndPersistHrdbCases(DateTime.Now.Subtract(TimeSpan.FromDays(1)), DateTime.Now, session);

                    transaction.Commit();
                }
            }

            return dict;
        }

        public IDictionary<string, HrdbCase> GetAndPersistHrdbCases(DateTime from, DateTime to, ISession session)
        {
            var profiler = MiniProfiler.Current;
            IDictionary<string, HrdbCase> dict = null;

            using (profiler.Step("Calling webservice..."))
                dict = this.hrdbRepo.GetHrdbCases(from, to);

            using (profiler.Step("Persisting cases..."))
            {
                foreach (KeyValuePair<string, HrdbCase> kvp in dict)
                {
                    JhroCase jc = this.sourceTasks.GetJhroCase(session, kvp.Value.CaseCode);
                    if (jc == null)
                        jc = new JhroCase() { CaseNumber = kvp.Key };

                    // serialize data received over the wire
                    jc.HrdbContentsSerialized = StreamUtil.Serialize(kvp.Value);

                    this.sourceTasks.SaveJhroCase(session, jc);
                }
            }

            return dict;
        }
    }
}
