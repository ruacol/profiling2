using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Scr;
using Profiling2.Domain.Scr.PersonEntity;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class ScreeningEntityQueries : NHibernateQuery, IScreeningEntityQueries
    {
        public IList<ScreeningRequestPersonEntity> SearchScreenings(string term, int screeningEntityId)
        {
            ScreeningRequestPersonEntity srpeAlias2 = null;

            return Session.QueryOver<ScreeningRequestPersonEntity>(() => srpeAlias2)
                .Where(() => !srpeAlias2.Archive)
                .And(() => srpeAlias2.ScreeningEntity.Id == screeningEntityId)
                .And(Restrictions.Disjunction()
                    .Add(Restrictions.On(() => srpeAlias2.Reason).IsInsensitiveLike("%" + term + "%"))
                    .Add(Restrictions.On(() => srpeAlias2.Commentary).IsInsensitiveLike("%" + term + "%"))
                )
                .List<ScreeningRequestPersonEntity>();
        }

        public IList<ScreeningEntity> GetAllScreeningEntities(ISession session)
        {
            ISession thisSession = session == null ? Session : session;
            return thisSession.QueryOver<ScreeningEntity>().Where(x => !x.Archive).List();
        }

        public IList<ScreeningRequestPersonEntity> GetAllScreeningRequestPersonEntities(ISession session)
        {
            ISession thisSession = session == null ? Session : session;
            return thisSession.QueryOver<ScreeningRequestPersonEntity>().Where(x => !x.Archive).List();
        }
    }
}
