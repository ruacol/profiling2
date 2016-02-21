using Profiling2.Domain.Prf.Units;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr
{
    public class RequestUnit : Entity
    {
        public virtual Request Request { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }

        public RequestUnit() { }
    }
}
