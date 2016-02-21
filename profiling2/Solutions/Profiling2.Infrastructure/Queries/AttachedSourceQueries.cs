using System.Collections.Generic;
using NHibernate.Transform;
using Profiling2.Domain.Contracts.Queries;
using Profiling2.Domain.DTO;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using SharpArch.NHibernate;

namespace Profiling2.Infrastructure.Queries
{
    public class AttachedSourceQueries : NHibernateQuery, IAttachedSourceQueries
    {
        public IList<PersonSourceDTO> GetPersonSourceDTOs(int personId)
        {
            PersonSourceDTO dto = null;
            PersonSource psAlias = null;
            Source sAlias = null;
            Person pAlias = null;
            Reliability rAlias = null;

            return Session.QueryOver<PersonSource>(() => psAlias)
                .JoinAlias(() => psAlias.Source, () => sAlias)
                .JoinAlias(() => psAlias.Person, () => pAlias)
                .JoinAlias(() => psAlias.Reliability, () => rAlias)
                .Where(() => pAlias.Id == personId)
                .And(() => !psAlias.Archive)
                .SelectList(list => list
                    .Select(() => psAlias.Id).WithAlias(() => dto.Id)
                    .Select(() => psAlias.Commentary).WithAlias(() => dto.Commentary)
                    .Select(() => psAlias.Notes).WithAlias(() => dto.Notes)
                    .Select(() => sAlias.Id).WithAlias(() => dto.SourceId)
                    .Select(() => sAlias.SourceName).WithAlias(() => dto.SourceName)
                    .Select(() => sAlias.IsRestricted).WithAlias(() => dto.SourceIsRestricted)
                    .Select(() => pAlias.Id).WithAlias(() => dto.PersonId)
                    .Select(() => rAlias.Id).WithAlias(() => dto.ReliabilityId)
                    .Select(() => rAlias.ReliabilityName).WithAlias(() => dto.ReliabilityName)
                )
                .TransformUsing(Transformers.AliasToBean<PersonSourceDTO>())
                .List<PersonSourceDTO>();
        }
    }
}
