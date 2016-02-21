using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Events;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class ViolationDataTablesQuery : NHibernateQuery, IViolationDataTablesQuery
    {
        protected Violation violationAlias = null;
        protected Violation parentViolationAlias = null;
        protected Event eventsAlias = null;
        protected ViolationDataTableView output = null;

        protected IQueryOver<Violation, Violation> BaseSearch(string searchText)
        {
            IQueryOver<Violation, Violation> qo = Session.QueryOver<Violation>(() => violationAlias);
            if (!string.IsNullOrEmpty(searchText))
                qo.Where(Restrictions.On<Violation>(x => x.Name).IsInsensitiveLike("%" + searchText + "%")
                    || Restrictions.On<Violation>(x => x.Description).IsInsensitiveLike("%" + searchText + "%"));
            return qo;
        }

        public int GetSearchTotal(string searchText)
        {
            return this.BaseSearch(searchText).RowCount();
        }

        public IList<ViolationDataTableView> GetPaginatedResults(int iDisplayStart, int iDisplayLength, string searchText,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir)
        {
            var qo = this.BaseSearch(searchText);
            qo = qo.JoinAlias(() => violationAlias.ParentViolation, () => parentViolationAlias)
                .JoinAlias(() => violationAlias.Events, () => eventsAlias, JoinType.LeftOuterJoin)
                .SelectList(list => list
                    .Select(Projections.Group(() => violationAlias.Id).WithAlias(() => output.Id))
                    .Select(Projections.Group(() => violationAlias.Name).WithAlias(() => output.Name))
                    .Select(Projections.Group(() => violationAlias.Description).WithAlias(() => output.Description))
                    .Select(Projections.Group(() => violationAlias.ConditionalityInterest).WithAlias(() => output.ConditionalityInterest))
                    .Select(Projections.Group(() => parentViolationAlias.Name).WithAlias(() => output.ParentViolation))
                    .Select(Projections.Count(() => eventsAlias.Id)).WithAlias(() => output.NumberOfEvents)
                );

            if (iDisplayLength > -1)
                qo.Take(iDisplayLength);
            qo.Skip(iDisplayStart);

            for (int k = 0; k < iSortingCols; k++)
            {
                IProjection sortColumn = Projections.Property(() => violationAlias.Id);  // case 0
                switch (iSortCol[k])
                {
                    case 1:
                        sortColumn = Projections.Property(() => violationAlias.Name);
                        break;
                    case 2:
                        sortColumn = Projections.Property(() => violationAlias.Description);
                        break;
                    case 3:
                        sortColumn = Projections.Property(() => violationAlias.ConditionalityInterest);
                        break;
                    case 4:
                        sortColumn = Projections.Property(() => parentViolationAlias.Name);
                        break;
                    case 5:
                        sortColumn = Projections.Count(() => eventsAlias.Id);
                        break;
                }

                if (string.Equals(sSortDir[k], "asc"))
                    qo = qo.ThenBy(sortColumn).Asc;
                else if (string.Equals(sSortDir[k], "desc"))
                    qo = qo.ThenBy(sortColumn).Desc;
            }

            return qo.TransformUsing(Transformers.AliasToBean<ViolationDataTableView>())
                .List<ViolationDataTableView>();
        }
    }
}
