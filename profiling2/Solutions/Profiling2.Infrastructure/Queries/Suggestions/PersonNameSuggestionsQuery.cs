using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using Profiling2.Domain.Contracts.Queries.Suggestions;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;
using Profiling2.Domain.Prf.Careers;

namespace Profiling2.Infrastructure.Queries.Suggestions
{
    public class PersonNameSuggestionsQuery : NHibernateQuery, IPersonNameSuggestionsQuery
    {
        protected IList<string> Split(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return (from s in name.Split(new char[] { ' ' })
                        where !string.IsNullOrEmpty(s) && s.Length > 3  // filter short names
                        select s.Trim()).ToList<string>();
            }
            return null;
        }

        public IList<Event> GetEventsByLastName(Person p)
        {
            return this.GetEvents(this.Split(p.LastName));
        }

        public IList<Event> GetEventsByFirstName(Person p)
        {
            return this.GetEvents(this.Split(p.FirstName));
        }

        public IList<Event> GetEventsByAlias(Person p)
        {
            IList<string> names = new List<string>();
            foreach (PersonAlias pa in p.PersonAliases)
                if (!string.IsNullOrEmpty(pa.Name))
                    names = names.Concat(this.Split(pa.Name)).ToList();
            return this.GetEvents(names.Distinct());
        }

        protected IList<Event> GetEvents(IEnumerable<string> names)
        {
            if (names != null && names.Any())
            {
                IList<Rank> ranks = Session.QueryOver<Rank>().Where(x => !x.Archive).List();

                var q = Session.QueryOver<Event>().Where(x => !x.Archive);
                foreach (string name in names)
                {
                    if (!ranks.Where(x => x.RankName == name).Any())
                    {
                        q = q.And(Restrictions.Disjunction()
                                    .Add(Restrictions.On<Event>(x => x.NarrativeEn).IsLike("%" + name + "%"))
                                    .Add(Restrictions.On<Event>(x => x.NarrativeFr).IsLike("%" + name + "%"))
                                );
                    }
                }
                return q.List();
            }
            return new List<Event>();
        }
    }
}
