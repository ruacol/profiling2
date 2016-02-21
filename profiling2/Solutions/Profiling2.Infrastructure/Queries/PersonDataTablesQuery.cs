using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    /// <summary>
    /// Attempt to rewrite PRF_Search_SearchForPerson stored procedure using NHibernate QueryOver; not used.
    /// </summary>
    public class PersonDataTablesQuery : NHibernateQuery//, IPersonDataTablesQuery
    {
        protected Person personAlias = null;
        protected PersonAlias aliasAlias = null;

        // TODO search person aliases, current rank, role if commander or deputy commander, date of birth in yyyy-mm-dd or yyyy/mm/dd
        protected IQueryOver<Person, Person> BaseSearch(string searchText)
        {
            IQueryOver<Person, Person> qo = Session.QueryOver<Person>(() => personAlias)
                .JoinAlias(() => personAlias.PersonAliases, () => aliasAlias, JoinType.LeftOuterJoin)
                // Using this method of distinct places the load on the DB; however it doesn't give us access to the PersonAlias collection.
                // Using TransformUsing(Transformers.DistinctRootEntity<Person>()) would give us the fully hydrated entity, but at the expense
                // of having proper result counts (since it only returns unique results post-query).
                .Select(Projections.Distinct(
                    Projections.ProjectionList()
                        .Add(Projections.Property(() => personAlias.Id).WithAlias(() => personAlias.Id))
                        .Add(Projections.Property(() => personAlias.FirstName).WithAlias(() => personAlias.FirstName))
                        .Add(Projections.Property(() => personAlias.LastName).WithAlias(() => personAlias.LastName))
                        .Add(Projections.Property(() => personAlias.MilitaryIDNumber).WithAlias(() => personAlias.MilitaryIDNumber))
                        .Add(Projections.Property(() => personAlias.YearOfBirth).WithAlias(() => personAlias.YearOfBirth))
                        .Add(Projections.Property(() => personAlias.MonthOfBirth).WithAlias(() => personAlias.MonthOfBirth))
                        .Add(Projections.Property(() => personAlias.DayOfBirth).WithAlias(() => personAlias.DayOfBirth))
                        .Add(Projections.Property(() => personAlias.ApproximateBirthDate).WithAlias(() => personAlias.ApproximateBirthDate))
                        .Add(Projections.Property(() => personAlias.BirthVillage).WithAlias(() => personAlias.BirthVillage))
                        .Add(Projections.Property(() => personAlias.BirthRegion).WithAlias(() => personAlias.BirthRegion))
                    ));
            if (!string.IsNullOrEmpty(searchText))
                qo.Where(Restrictions.On<Person>(x => x.FirstName).IsLike("%" + searchText + "%")
                    || Restrictions.On<Person>(x => x.LastName).IsLike("%" + searchText + "%")
                    || Restrictions.On<Person>(x => x.MilitaryIDNumber).IsLike("%" + searchText + "%")
                    || Restrictions.On(() => aliasAlias.FirstName).IsLike("%" + searchText + "%")
                    || Restrictions.On(() => aliasAlias.LastName).IsLike("%" + searchText + "%"));
            return qo.TransformUsing(Transformers.AliasToBean<Person>());
        }

        public int GetSearchTotal(string searchText)
        {
            return this.BaseSearch(searchText).RowCount();
        }

        public IList<Person> GetPaginatedResults(int iDisplayStart, int iDisplayLength, string searchText,
            int iSortingCols, IList<int> iSortCol, IList<string> sSortDir)
        {
            var qo = this.BaseSearch(searchText);

            if (iDisplayLength > -1)
                qo.Take(iDisplayLength);
            qo.Skip(iDisplayStart);

            for (int k = 0; k < iSortingCols; k++)
            {
                IList<Expression<Func<object>>> sortColumns = new List<Expression<Func<object>>>();
                switch (iSortCol[k])
                {
                    case 0:
                        sortColumns.Add(() => personAlias.Id);
                        break;
                    case 1:
                        sortColumns.Add(() => personAlias.FirstName);
                        break;
                    case 2:
                        sortColumns.Add(() => personAlias.LastName);
                        break;
                    case 3:
                        break;
                    case 4:
                        sortColumns.Add(() => personAlias.YearOfBirth);
                        sortColumns.Add(() => personAlias.MonthOfBirth);
                        sortColumns.Add(() => personAlias.DayOfBirth);
                        break;
                    case 5:
                        sortColumns.Add(() => personAlias.BirthVillage);
                        break;
                    case 6:
                        sortColumns.Add(() => personAlias.MilitaryIDNumber);
                        break;
                }

                foreach (Expression<Func<object>> column in sortColumns)
                {
                    if (string.Equals(sSortDir[k], "asc"))
                        qo = qo.ThenBy(column).Asc;
                    else if (string.Equals(sSortDir[k], "desc"))
                        qo = qo.ThenBy(column).Desc;
                }
            }

            return qo.List<Person>();
        }
    }
}
