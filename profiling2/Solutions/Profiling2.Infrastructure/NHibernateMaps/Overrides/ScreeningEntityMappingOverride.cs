using Profiling2.Domain.Scr;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using Profiling2.Domain.Prf;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class ScreeningEntityMappingOverride : IAutoMappingOverride<ScreeningEntity>
    {
        public void Override(AutoMapping<ScreeningEntity> mapping)
        {
            mapping.HasManyToMany<AdminUser>(x => x.Users)
                .Table("SCR_ScreeningEntityAdminUser")
                .ParentKeyColumn("ScreeningEntityID")
                .ChildKeyColumn("AdminUserID")
                .Inverse()
                .Cascade.None()
                .AsBag();
        }
    }
}
