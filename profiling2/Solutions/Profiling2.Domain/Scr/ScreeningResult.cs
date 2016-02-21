using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Scr
{
    public class ScreeningResult : Entity
    {
        public const string GREEN = "Green";
        public const string YELLOW = "Yellow";
        public const string RED = "Red";
        public const string PENDING = "Pending";
        public const int ID_GREEN = 1;
        public const int ID_YELLOW = 2;
        public const int ID_RED = 3;
        public const int ID_PENDING = 4;

        public virtual string ScreeningResultName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.ScreeningResultName;
        }
    }
}
