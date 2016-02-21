using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr.PersonFinalDecision
{
    public class ScreeningSupportStatus : Entity
    {
        public const int ID_SUPPORT_RECOMMENDED = 1;
        public const int ID_SUPPORT_NOT_RECOMMENDED = 2;
        public const int ID_MONITORED_SUPPORT = 3;
        public const int ID_PENDING = 4;

        public virtual string ScreeningSupportStatusName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.ScreeningSupportStatusName;
        }
    }
}
