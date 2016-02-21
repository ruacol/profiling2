using Profiling2.Domain.Scr;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using Profiling2.Domain.Prf;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class RequestEntityMappingOverride : IAutoMappingOverride<RequestEntity>
    {
        public void Override(AutoMapping<RequestEntity> mapping)
        {
            mapping.HasManyToMany<AdminUser>(x => x.Users)
                .Table("SCR_RequestEntityAdminUser")
                .ParentKeyColumn("RequestEntityID")
                .ChildKeyColumn("AdminUserID")
                .Inverse()
                .Cascade.None()
                .AsBag();
        }
    }
}
