using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class AdminAuditType : Entity
    {
        public const int ID_INSERT = 1;
        public const int ID_UPDATE = 2;
        public const int ID_DELETE = 3;

        public virtual string AdminAuditTypeName { get; set; }
        public virtual bool Archive { get; set; }

        public override string ToString()
        {
            return this.AdminAuditTypeName;
        }
    }
}
