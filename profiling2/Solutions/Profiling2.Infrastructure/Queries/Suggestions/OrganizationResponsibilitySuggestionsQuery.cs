using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries.Suggestions;
using Profiling2.Domain.Prf.Careers;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Responsibility;
using Profiling2.Domain.Prf.Units;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries.Suggestions
{
    public class OrganizationResponsibilitySuggestionsQuery : NHibernateQuery, IOrganizationResponsibilitySuggestionsQuery
    {
        public IList<OrganizationResponsibility> GetOrganizationResponsibilitiesLinkedByOrganization(Person p)
        {
            OrganizationResponsibility or = null;
            Organization o = null;
            Career c = null;

            return Session.QueryOver<OrganizationResponsibility>(() => or)
                .JoinAlias(() => or.Organization, () => o, JoinType.InnerJoin)
                .JoinAlias(() => o.Careers, () => c, JoinType.InnerJoin)
                .Where(() => c.Person == p)
                .And(() => !c.Archive)
                .And(() => !o.Archive)
                .And(() => !or.Archive)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();
        }

        public IList<OrganizationResponsibility> GetOrganizationResponsibilitiesLinkedByUnit(Person p)
        {
            OrganizationResponsibility or = null;
            Unit u = null;
            Career c = null;

            return Session.QueryOver<OrganizationResponsibility>(() => or)
                .JoinAlias(() => or.Unit, () => u, JoinType.InnerJoin)
                .JoinAlias(() => u.Careers, () => c, JoinType.InnerJoin)
                .Where(() => c.Person == p)
                .And(() => !c.Archive)
                .And(() => !u.Archive)
                .And(() => !or.Archive)
                .TransformUsing(Transformers.DistinctRootEntity)
                .List();
        }
    }
}
