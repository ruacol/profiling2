using FluentNHibernate.Automapping.Alterations;
using Profiling2.Domain.Scr.Attach;
using FluentNHibernate.Automapping;

namespace Profiling2.Infrastructure.NHibernateMaps.Overrides
{
    public class RequestAttachmentOverride : IAutoMappingOverride<RequestAttachment>
    {
        public void Override(AutoMapping<RequestAttachment> mapping)
        {
            mapping.HasMany<RequestAttachmentHistory>(x => x.Histories)
                .KeyColumn("RequestAttachmentID")
                .Cascade.AllDeleteOrphan()
                .Inverse();
        }
    }
}
