using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Contracts.Queries.Search;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class PersonRelationshipTypeNameQuery : NHibernateQuery, IPersonRelationshipTypeNameQuery
    {
        public IList<PersonRelationshipType> GetResults(string term)
        {
            //var qo = Session.QueryOver<PersonRelationshipType>();
            //if (!string.IsNullOrEmpty(term))
            //    return qo.Where(Restrictions.On<PersonRelationshipType>(x => x.PersonRelationshipTypeName).IsLike("%" + term + "%"))
            //        .Take(50)
            //        .List<PersonRelationshipType>();
            //else
            //    return new List<PersonRelationshipType>();
            if (!string.IsNullOrEmpty(term))
                return Session.CreateCriteria<PersonRelationshipType>()
                    .Add(Expression.Sql("PersonRelationshipTypeName LIKE ? COLLATE Latin1_general_CI_AI", "%" + term + "%", NHibernateUtil.String))
                    .AddOrder(Order.Asc("PersonRelationshipTypeName"))
                    .SetMaxResults(50)
                    .List<PersonRelationshipType>();
            else
                return new List<PersonRelationshipType>();
        }
    }
}
