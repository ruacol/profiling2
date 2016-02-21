using SharpArch.Domain.DomainModel;
using NHibernate.Envers.Configuration.Attributes;

namespace Profiling2.Domain.Prf.Units
{
    public class UnitLocation : Entity, IAsOfDate, IIncompleteDate
    {
        [Audited]
        public virtual Unit Unit { get; set; }
        [Audited]
        public virtual Location Location { get; set; }
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
        public virtual int DayAsOf { get; set; }
        [Audited]
        public virtual int MonthAsOf { get; set; }
        [Audited]
        public virtual int YearAsOf { get; set; }
        [Audited]
        public virtual string Commentary { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
    }
}
