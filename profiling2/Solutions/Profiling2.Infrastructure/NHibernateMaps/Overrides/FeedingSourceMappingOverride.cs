using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Prf.Sources;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class FeedingSourceMappingOverride : IAutoMappingOverride<FeedingSource>
    {
        public void Override(AutoMapping<FeedingSource> mapping)
        {
            mapping.HasManyToMany<SourceAuthor>(x => x.SourceAuthors)
                .Table("PRF_SourceAuthorFeedingSource")
                .ParentKeyColumn("FeedingSourceID")
                .ChildKeyColumn("SourceAuthorID")
                .AsBag();
            mapping.HasManyToMany<SourceOwningEntity>(x => x.SourceOwningEntities)
                .Table("PRF_FeedingSourceOwner")
                .ParentKeyColumn("FeedingSourceID")
                .ChildKeyColumn("SourceOwningEntityID")
                .AsBag();
        }
    }
}
