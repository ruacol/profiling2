using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf;
using Profiling2.Domain.Prf.Persons;
using Profiling2.Domain.Prf.Sources;
using Profiling2.Domain.Scr;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class AdminUserMappingOverride : IAutoMappingOverride<AdminUser>
    {
        public void Override(AutoMapping<AdminUser> mapping)
        {
            mapping.HasManyToMany<AdminRole>(x => x.AdminRoles)
                .Table("PRF_AdminUserRole")
                .ParentKeyColumn("AdminUserID")
                .ChildKeyColumn("AdminRoleID")
                .AsBag();
            mapping.HasManyToMany<RequestEntity>(x => x.RequestEntities)
                .Table("SCR_RequestEntityAdminUser")
                .ParentKeyColumn("AdminUserID")
                .ChildKeyColumn("RequestEntityID")
                .AsBag();
            mapping.HasManyToMany<ScreeningEntity>(x => x.ScreeningEntities)
                .Table("SCR_ScreeningEntityAdminUser")
                .ParentKeyColumn("AdminUserID")
                .ChildKeyColumn("ScreeningEntityID")
                .AsBag();
            mapping.HasMany<AdminExportedPersonProfile>(x => x.AdminExportedPersonProfiles)
                .KeyColumn("ExportedByAdminUserID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasManyToMany<SourceOwningEntity>(x => x.Affiliations)
                .Table("PRF_AdminUserAffiliation")
                .ParentKeyColumn("AdminUserID")
                .ChildKeyColumn("SourceOwningEntityID")
                .AsBag();
        }
    }
}
