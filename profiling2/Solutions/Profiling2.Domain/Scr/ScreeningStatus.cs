using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr
{
    public class ScreeningStatus : Entity
    {
        public const int ADDED = 1;
        public const int UPDATED = 2;

        public virtual string ScreeningStatusName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.ScreeningStatusName;
        }
    }
}
