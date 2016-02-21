using System.Collections.Generic;
using NHibernate.Criterion;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Careers;
using SharpArch.NHibernate;
using NHibernate.SqlCommand;

namespace Profiling2.Infrastructure.Queries
{
    public class CommandersQuery : NHibernateQuery, ICommandersQuery
    {
        protected Career careerAlias = null;
        protected Role roleAlias = null;
        protected Rank rankAlias = null;

        public IList<Career> GetCommanders()
        {
            return Session.QueryOver<Career>(() => careerAlias)
                .JoinAlias(() => careerAlias.Role, () => roleAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => careerAlias.Rank, () => rankAlias, JoinType.LeftOuterJoin)
                .Where(Restrictions.On(() => roleAlias.RoleName).IsLike("%ommander%")
                    || Restrictions.On(() => roleAlias.RoleName).IsLike("%eader%")
                    || Restrictions.On(() => rankAlias.RankName).IsLike("%ommander%")
                    || Restrictions.On(() => rankAlias.RankName).IsLike("%eader%"))
                .Where(() => !careerAlias.Archive)
                .List<Career>();
        }
    }
}
