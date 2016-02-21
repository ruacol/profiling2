using System.Collections.Generic;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class AdminUserQueries : NHibernateQuery, IAdminUserQueries
    {
        public AdminUser GetAdminUser(string userId)
        {
            IList<AdminUser> list = Session.QueryOver<AdminUser>()
                .Where(x => !x.Archive)
                .AndRestrictionOn(x => x.UserID).IsInsensitiveLike(userId)
                .List();
            if (list != null && list.Count > 0)
                return list[0];
            return null;
        }
    }
}
