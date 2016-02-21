using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr
{
    public class RequestType : Entity
    {
        public virtual string RequestTypeName { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Notes))
                return this.RequestTypeName + " (" + this.Notes + ")";
            else
                return this.RequestTypeName;
        }
    }
}
