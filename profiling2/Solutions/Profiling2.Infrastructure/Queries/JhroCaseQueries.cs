using System.Collections.Generic;
using NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class JhroCaseQueries : NHibernateQuery, IJhroCaseQueries
    {
        public JhroCase GetJhroCase(ISession session, string caseNumber)
        {
            ISession thisSession = session == null ? this.Session : session;

            IList<JhroCase> results = thisSession.QueryOver<JhroCase>()
                .Where(x => x.CaseNumber == caseNumber)
                .List<JhroCase>();

            if (results != null && results.Count > 0)
                return results[0];

            return null;
        }

        public void SaveJhroCase(ISession session, JhroCase jhroCase)
        {
            ISession thisSession = session == null ? this.Session : session;

            thisSession.SaveOrUpdate(jhroCase);
        }

        public IList<JhroCase> SearchJhroCases(string term)
        {
            return Session.QueryOver<JhroCase>()
                .WhereRestrictionOn(x => x.CaseNumber).IsInsensitiveLike("%" + term + "%")
                .List<JhroCase>();
        }
    }
}
