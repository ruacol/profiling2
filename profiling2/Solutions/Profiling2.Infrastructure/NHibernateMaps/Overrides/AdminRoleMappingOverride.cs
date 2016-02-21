using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class AdminRoleMappingOverride : IAutoMappingOverride<AdminRole>
    {
        public void Override(AutoMapping<AdminRole> mapping)
        {
            mapping.HasManyToMany<AdminPermission>(x => x.AdminPermissions)
                .Table("PRF_AdminRolePermission")
                .ParentKeyColumn("AdminRoleID")
                .ChildKeyColumn("AdminPermissionID")
                .AsBag();
        }
    }
}
