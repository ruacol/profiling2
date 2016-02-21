using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Units;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class UnitQueries : NHibernateQuery, IUnitQueries
    {
        public IList<UnitDTO> GetEmptyUnits()
        {
            Unit unitAlias = null;
            //Career careerAlias = null;
            //UnitHierarchy hierarchyAlias = null;
            //UnitHierarchy hierarchyChildrenAlias = null;
            //OrganizationResponsibility orgResponsibilityAlias = null;
            UnitDTO output = null;

            return Session.QueryOver<Unit>(() => unitAlias)
                //.JoinAlias(() => unitAlias.Careers, () => careerAlias, JoinType.LeftOuterJoin)
                //.JoinAlias(() => unitAlias.UnitHierarchies, () => hierarchyAlias, JoinType.LeftOuterJoin)
                //.JoinAlias(() => unitAlias.UnitHierarchyChildren, () => hierarchyChildrenAlias, JoinType.LeftOuterJoin)
                //.JoinAlias(() => unitAlias.OrganizationResponsibilities, () => orgResponsibilityAlias, JoinType.LeftOuterJoin)
                //.Where(Restrictions.On(() => careerAlias).IsNull
                //    || Restrictions.On(() => hierarchyAlias).IsNull
                //    || Restrictions.On(() => hierarchyChildrenAlias).IsNull
                //    || Restrictions.On(() => orgResponsibilityAlias).IsNull)
                .WhereRestrictionOn(() => unitAlias.Careers).IsEmpty
                .AndRestrictionOn(() => unitAlias.UnitHierarchies).IsEmpty
                .AndRestrictionOn(() => unitAlias.UnitHierarchyChildren).IsEmpty
                .AndRestrictionOn(() => unitAlias.OrganizationResponsibilities).IsEmpty
                .Where(() => !unitAlias.Archive)
                .SelectList(list => list
                    .Select(Projections.Property(() => unitAlias.Id)).WithAlias(() => output.Id)
                    .Select(Projections.Property(() => unitAlias.UnitName)).WithAlias(() => output.UnitName)
                    )
                .TransformUsing(Transformers.AliasToBean<UnitDTO>())
                .List<UnitDTO>();
        }

        public IList<Unit> GetAllUnits(ISession session)
        {
            ISession thisSession = session == null ? Session : session;
            return thisSession.QueryOver<Unit>().List();
        }

        public Unit GetUnit(ISession session, int unitId)
        {
            ISession thisSession = session == null ? Session : session;
            IList<Unit> units = thisSession.QueryOver<Unit>()
                .Where(x => x.Id == unitId)
                .List<Unit>();

            if (units != null && units.Count > 0)
                return units[0];
            return null;
        }
    }
}
