using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class AdminUnknown : Entity
    {
        public virtual string UnknownValue { get; set; }
        public virtual bool Archive { get; set; }
    }
}
