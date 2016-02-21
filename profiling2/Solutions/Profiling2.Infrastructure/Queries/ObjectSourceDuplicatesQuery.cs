using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Events;
using Profiling2.Domain.Prf.Organizations;
using Profiling2.Domain.Prf.Persons;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class ObjectSourceDuplicatesQuery : NHibernateQuery, IObjectSourceDuplicatesQuery
    {
        public IList<ObjectSourceDuplicateDTO> GetPersonSourceDuplicates()
        {
            ObjectSourceDuplicateDTO output = null;

            return Session.QueryOver<PersonSource>()
                .SelectList(list => list
                    .SelectGroup(x => x.Person.Id).WithAlias(() => output.ObjectID)
                    .SelectGroup(x => x.Source.Id).WithAlias(() => output.SourceID)
                    .SelectCount(x => x.Source.Id).WithAlias(() => output.Count)
                )
                .Where(Restrictions.Gt(
                    Projections.Count<PersonSource>(x => x.Person.Id), 1))
                .TransformUsing(Transformers.AliasToBean<ObjectSourceDuplicateDTO>())
                .List<ObjectSourceDuplicateDTO>();
        }

        public IList<ObjectSourceDuplicateDTO> GetEventSourceDuplicates()
        {
            ObjectSourceDuplicateDTO output = null;

            return Session.QueryOver<EventSource>()
                .SelectList(list => list
                    .SelectGroup(x => x.Event.Id).WithAlias(() => output.ObjectID)
                    .SelectGroup(x => x.Source.Id).WithAlias(() => output.SourceID)
                    .SelectCount(x => x.Source.Id).WithAlias(() => output.Count)
                )
                .Where(Restrictions.Gt(
                    Projections.Count<EventSource>(x => x.Event.Id), 1))
                .TransformUsing(Transformers.AliasToBean<ObjectSourceDuplicateDTO>())
                .List<ObjectSourceDuplicateDTO>();
        }

        public IList<ObjectSourceDuplicateDTO> GetOrganizationSourceDuplicates()
        {
            ObjectSourceDuplicateDTO output = null;

            return Session.QueryOver<OrganizationSource>()
                .SelectList(list => list
                    .SelectGroup(x => x.Organization.Id).WithAlias(() => output.ObjectID)
                    .SelectGroup(x => x.Source.Id).WithAlias(() => output.SourceID)
                    .SelectCount(x => x.Source.Id).WithAlias(() => output.Count)
                )
                .Where(Restrictions.Gt(
                    Projections.Count<OrganizationSource>(x => x.Organization.Id), 1))
                .TransformUsing(Transformers.AliasToBean<ObjectSourceDuplicateDTO>())
                .List<ObjectSourceDuplicateDTO>();
        }
    }
}
