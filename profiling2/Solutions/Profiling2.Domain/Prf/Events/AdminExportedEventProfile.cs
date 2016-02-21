using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Events
{
    public class AdminExportedEventProfile : Entity
    {
        public virtual DateTime ExportDateTime { get; set; }
        public virtual AdminUser ExportedByAdminUser { get; set; }
        public virtual string ClientDnsName { get; set; }
        public virtual string ClientIpAddress { get; set; }
        public virtual string ClientUserAgent { get; set; }
        public virtual Event Event { get; set; }
        public virtual bool Archive { get; set; }
    }
}
