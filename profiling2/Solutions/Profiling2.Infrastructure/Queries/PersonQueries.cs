using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Type;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class PersonQueries : NHibernateQuery, IPersonQueries
    {
        public Person GetPerson(ISession session, int personId)
        {
            IList<Person> persons = session.QueryOver<Person>()
                .Where(x => x.Id == personId)
                .List<Person>();

            if (persons != null && persons.Count > 0)
                return persons.First();
            return null;
        }

        public IList<Person> GetPersonsByEthnicity(Ethnicity ethnicity, bool canAccessRestrictedProfiles)
        {
            var qo = Session.QueryOver<Person>()
                .Where(Restrictions.Disjunction()
                    .Add<Person>(x => x.Ethnicity == ethnicity)
                    .Add(Restrictions.On<Person>(x => x.Ethnicity).IsIn(ethnicity.SameEthnicitiesFrom1.ToArray()))
                    .Add(Restrictions.On<Person>(x => x.Ethnicity).IsIn(ethnicity.SameEthnicitiesFrom2.ToArray()))
                    )
                .And(x => !x.Archive);

            if (!canAccessRestrictedProfiles)
                qo = qo.Where(x => !x.IsRestrictedProfile);

            return qo.List<Person>();
        }

        public IList<Person> GetIncompletePersons()
        {
            Person personAlias = null;
            ProfileStatus statusAlias = null;
            Region regionAlias = null;
            Ethnicity ethnicityAlias = null;

            return Session.QueryOver<Person>(() => personAlias)
                .Fetch(x => x.Ethnicity).Eager
                .Fetch(x => x.BirthRegion).Eager
                .JoinAlias(() => personAlias.ProfileStatus, () => statusAlias)
                .JoinAlias(() => personAlias.Ethnicity, () => ethnicityAlias, JoinType.LeftOuterJoin)
                .JoinAlias(() => personAlias.BirthRegion, () => regionAlias, JoinType.LeftOuterJoin)
                .Where(Restrictions.Disjunction()
                    .Add(() => personAlias.Ethnicity == null || ethnicityAlias.EthnicityName == "Unknown")
                    .Add(Restrictions.On<Person>(x => x.PersonPhotos).IsEmpty)
                    .Add(Restrictions.On<Person>(x => x.PersonResponsibilities).IsEmpty)
                    .Add(Restrictions.On<Person>(x => x.Careers).IsEmpty)
                    .Add(() => ((personAlias.BirthVillage == null || personAlias.BirthVillage == "0") && (personAlias.BirthRegion == null || regionAlias.RegionName == "Unknown")))
                    .Add(() => personAlias.YearOfBirth == 0 && personAlias.MonthOfBirth == 0 && personAlias.DayOfBirth == 0 && personAlias.ApproximateBirthDate == null))
                .AndRestrictionOn(() => statusAlias.ProfileStatusName).Not.IsIn(new string[] { ProfileStatus.FARDC_2007_LIST })
                .List<Person>();
        }

        public IList<Person> GetPagedPersons(ISession session, int pageSize, int page)
        {
            return session.QueryOver<Person>()
                .Where(x => !x.Archive)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .List<Person>();
        }

        public int GetPersonsCount(ISession session)
        {
            ISession thisSession = session == null ? Session : session;
            return thisSession.QueryOver<Person>().Where(x => !x.Archive).RowCount();
        }

        public IList<Person> GetPersonsWanted()
        {
            return Session.QueryOver<Person>()
                .Where(Restrictions.Disjunction()
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%arrest%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%arret%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%warrant%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%mandat%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%fugitive%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%fugitif%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%escape%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%evade%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%wanted%"))
                    .Add(Restrictions.On<Person>(x => x.BackgroundInformation).IsLike("%recherche%"))
                    )
                .And(x => !x.Archive)
                .List<Person>();
        }

        public IList<Person> GetPersonsByName(string term)
        {
            //var qo = Session.QueryOver<Person>();
            //if (!string.IsNullOrEmpty(term))
            //    return qo.Where(Restrictions.On<Person>(x => x.FirstName).IsLike("%" + term + "%") 
            //        || Restrictions.On<Person>(x => x.LastName).IsLike("%" + term + "%")).Take(50).List<Person>();
            //else
            //    return new List<Person>();
            if (!string.IsNullOrEmpty(term))
                return Session.CreateCriteria<Person>()
                    .Add(Expression.Sql(@"
                        FirstName LIKE ? COLLATE Latin1_general_CI_AI
                        OR LastName LIKE ? COLLATE Latin1_general_CI_AI
                    ", new string[] { "%" + term + "%", "%" + term + "%" }, new IType[] { NHibernateUtil.String, NHibernateUtil.String }))
                     .AddOrder(Order.Asc("LastName"))
                     .AddOrder(Order.Asc("FirstName"))
                     .SetMaxResults(50)
                    .List<Person>();
            else
                return new List<Person>();
        }

        public IList<ProfileStatus> GetAllProfileStatuses(ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<ProfileStatus>()
                .Where(x => !x.Archive)
                .AndRestrictionOn(x => x.ProfileStatusName).Not.IsInsensitiveLike(ProfileStatus.FARDC_2007_LIST)
                .List();
        }

        public ProfileStatus GetProfileStatus(string code, ISession session)
        {
            ISession thisSession = session == null ? this.Session : session;
            return thisSession.QueryOver<ProfileStatus>()
                .Where(x => !x.Archive)
                .AndRestrictionOn(x => x.ProfileStatusName).IsInsensitiveLike(code)
                .SingleOrDefault();
        }
    }
}
