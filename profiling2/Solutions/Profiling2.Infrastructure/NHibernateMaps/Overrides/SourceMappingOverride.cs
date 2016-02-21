using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class SourceMappingOverride : IAutoMappingOverride<Source>
    {
        public void Override(AutoMapping<Source> mapping)
        {
            // 2013-03-16 currently breaks audit of PersonSource.SourceID field with 'no persister for SourceProxy' exception.
            //mapping.Map(x => x.FileData).LazyLoad();

            mapping.HasMany<SourceRelationship>(x => x.SourceRelationshipsAsParent)
                .KeyColumn("ParentSourceID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasMany<SourceRelationship>(x => x.SourceRelationshipsAsChild)
                .KeyColumn("ChildSourceID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
            mapping.HasManyToMany<SourceAuthor>(x => x.SourceAuthors)
                .Table("PRF_SourceAuthorSource")
                .ParentKeyColumn("SourceID")
                .ChildKeyColumn("SourceAuthorID")
                .AsBag();
            mapping.HasManyToMany<SourceOwningEntity>(x => x.SourceOwningEntities)
                .Table("PRF_SourceOwner")
                .ParentKeyColumn("SourceID")
                .ChildKeyColumn("SourceOwningEntityID")
                .AsBag();
        }
    }
}
