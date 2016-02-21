using System.Collections.Generic;
using SharpArch.Domain.DomainModel;
using NHibernate.Envers.Configuration.Attributes;

namespace Profiling2.Domain.Prf.Units
{
    public class UnitHierarchy : Entity, IIncompleteDate, IAsOfDate
    {
        [Audited]
        public virtual Unit Unit { get; set; }
        [Audited]
        public virtual Unit ParentUnit { get; set; }
        [Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
        public virtual UnitHierarchyType UnitHierarchyType { get; set; }
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
        public virtual string Commentary { get; set; }
        [Audited]
        public virtual bool Archive { get; set; }
        [Audited]
        public virtual string Notes { get; set; }
        [Audited]
        public virtual int DayAsOf { get; set; }
        [Audited]
        public virtual int MonthAsOf { get; set; }
        [Audited]
        public virtual int YearAsOf { get; set; }

        public virtual IList<AdminUnitHierarchyImport> AdminUnitHierarchyImports { get; set; }
    }
}
