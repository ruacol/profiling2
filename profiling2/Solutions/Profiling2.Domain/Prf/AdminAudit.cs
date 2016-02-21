using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class AdminAudit : Entity
    {
        public virtual AdminAuditType AdminAuditType { get; set; }
        public virtual AdminUser AdminUser { get; set; }
        public virtual string ChangedTableName { get; set; }
        public virtual int ChangedRecordID { get; set; }
        public virtual DateTime ChangedDateTime { get; set; }
        //public virtual XML ChangedColumns { get; set; }
        public virtual bool Archive { get; set; }
        public virtual bool IsProfilingChange { get; set; }
    }
}
