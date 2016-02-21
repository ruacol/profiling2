using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class AdminUnitHierarchyImport : Entity
    {
        public virtual UnitHierarchy UnitHierarchy { get; set; }
        public virtual AdminUnitHierarchyImportType AdminUnitHierarchyImportType { get; set; }
        public virtual DateTime ImportDate { get; set; }
        public virtual int PreviousID { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
    }
}
