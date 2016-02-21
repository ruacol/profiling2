using System.Collections.Generic;
using SharpArch.NHibernate;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Actions;
using NHibernate.Criterion;

namespace Profiling2.Infrastructure.Queries
{
    public class ActionTakenWantedQuery : NHibernateQuery, IActionTakenWantedQuery
    {
        protected ActionTaken atAlias = null;
        protected ActionTakenType attAlias = null;

        public IList<ActionTaken> GetWantedActionsTaken()
        {
            return Session.QueryOver<ActionTaken>(() => atAlias)
                .JoinAlias(() => atAlias.ActionTakenType, () => attAlias)
                .Where(Restrictions.On(() => attAlias.ActionTakenTypeName).IsIn(new string[]
                {
                    "Investigation opened",
                    "Alleged perpetrator(s) transferred to military prosecutor's office",
                    "Complaint lodged",
                    "was sentenced"
                }))
                .And(Restrictions.Or(
                    Restrictions.On(() => atAlias.SubjectPerson).IsNotNull,
                    Restrictions.On(() => atAlias.ObjectPerson).IsNotNull
                    ))
                .And(() => !atAlias.Archive)
                .List<ActionTaken>();
        }

        public IList<ActionTakenType> GetActionTakenTypes(string term)
        {
            var qo = Session.QueryOver<ActionTakenType>();
            if (!string.IsNullOrEmpty(term))
                return qo.Where(Restrictions.On<ActionTakenType>(x => x.ActionTakenTypeName).IsInsensitiveLike("%" + term + "%"))
                    .List<ActionTakenType>();
            else
                return new List<ActionTakenType>();
        }
    }
}
