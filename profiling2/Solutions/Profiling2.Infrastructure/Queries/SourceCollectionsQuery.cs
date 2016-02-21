using System.Collections.Generic;
using NHibernate.Criterion;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Prf.Units;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class SourceCollectionsQuery : NHibernateQuery, ISourceCollectionsQuery
    {
        public IList<AdminReviewedSource> GetReviewsForSource(int sourceId)
        {
            return Session.QueryOver<AdminReviewedSource>()
                .Where(x => x.Source.Id == sourceId)
                .And(x => !x.Archive)
                .OrderBy(x => x.ReviewedDateTime).Desc
                .List();
        }

        public IList<AdminSourceImport> GetAdminImportsForSource(int sourceId)
        {
            return Session.QueryOver<AdminSourceImport>()
                .Where(x => x.Source.Id == sourceId)
                .OrderBy(x => x.ImportDate).Desc
                .List();
        }

        public IList<PersonSource> GetPersonSources(int sourceId)
        {
            return Session.QueryOver<PersonSource>()
                .Where(x => x.Source.Id == sourceId)
                .And(x => !x.Archive)
                .List();
        }

        public IList<EventSource> GetEventSources(int sourceId)
        {
            return Session.QueryOver<EventSource>()
                .Where(x => x.Source.Id == sourceId)
                .And(x => !x.Archive)
                .List();
        }

        public IList<EventSource> SearchEventSources(string term)
        {
            return Session.QueryOver<EventSource>()
                .Where(Restrictions.Disjunction()
                    .Add(Restrictions.On<EventSource>(x => x.Commentary).IsInsensitiveLike("%" + term + "%"))
                    .Add(Restrictions.On<EventSource>(x => x.Notes).IsInsensitiveLike("%" + term + "%"))
                    )
                .And(x => !x.Archive)
                .List();
        }

        public IList<UnitSource> GetUnitSources(int sourceId)
        {
            return Session.QueryOver<UnitSource>()
                .Where(x => x.Source.Id == sourceId)
                .And(x => !x.Archive)
                .List();
        }

        public IList<OperationSource> GetOperationSources(int sourceId)
        {
            return Session.QueryOver<OperationSource>()
                .Where(x => x.Source.Id == sourceId)
                .And(x => !x.Archive)
                .List();
        }

        public SourceRelationship GetParentSourceOf(int sourceId)
        {
            return Session.QueryOver<SourceRelationship>()
                .Where(x => x.ChildSource.Id == sourceId)
                .Take(1)
                .SingleOrDefault();
        }

        public IList<SourceRelationship> GetChildSourcesOf(int sourceId)
        {
            return Session.QueryOver<SourceRelationship>()
                .Where(x => x.ParentSource.Id == sourceId)
                .List();
        }
    }
}
