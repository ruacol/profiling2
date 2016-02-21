using NHibernate.Envers.Configuration.Attributes;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class UnitOperation : Entity, IIncompleteDate
    {
        [Audited]
        public virtual Unit Unit { get; set; }
        [Audited]
        public virtual Operation Operation { get; set; }
        [Audited]
        public virtual int DayOfStart { get; set; }
        [Audited]
        public virtual int MonthOfStart { get; set; }
        [Audited]
        public virtual int YearOfStart { get; set; }
        [Audited]
        public virtual int DayOfEnd { get; set; }
        [Audited]
        public virtual int MonthOfEnd { get; set; }
        [Audited]
        public virtual int YearOfEnd { get; set; }
        [Audited]
        public virtual bool IsCommandUnit { get; set; }

        public override string ToString()
        {
            return (this.Unit != null ? "Unit(ID=" + this.Unit.Id.ToString() + ")" : string.Empty)
                + (this.Operation != null ? " is linked with Operation(ID=" + this.Operation.Id.ToString() + ")" : string.Empty);
        }
    }
}
