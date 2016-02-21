using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class PhotoMappingOverride : IAutoMappingOverride<Photo>
    {
        public void Override(AutoMapping<Photo> mapping)
        {
            //mapping.Map(x => x.FileData).LazyLoad();
        }
    }
}
