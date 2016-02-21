using NHibernate;
using Profiling2.Domain.Contracts.Queries.Stats;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Stats
{
    public class CountsQuery : NHibernateQuery, ICountsQuery
    {
        public int GetCareerCount(ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<Career>()
                .Where(x => !x.Archive)
                .RowCount();
        }

        public int GetOrganizationCount(ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<Organization>()
                .Where(x => !x.Archive)
                .RowCount();
        }

        public int GetEventCount(ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<Event>()
                .Where(x => !x.Archive)
                .RowCount();
        }

        public int GetPersonResponsibilityCount(ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<PersonResponsibility>()
                .Where(x => !x.Archive)
                .RowCount();
        }

        public int GetOrganizationResponsibilityCount(ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<OrganizationResponsibility>()
                .Where(x => !x.Archive)
                .RowCount();
        }

        public int GetSourceCount(ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<Source>()
                .Where(x => !x.Archive)
                .RowCount();
        }
    }
}
