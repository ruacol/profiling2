using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Units
{
    public class AdminUnitImport : Entity
    {
        public virtual Unit Unit { get; set; }
        public virtual AdminUnitImportType AdminUnitImportType { get; set; }
        public virtual DateTime ImportDate { get; set; }
        public virtual int PreviousID { get; set; }
        public virtual string Notes { get; set; }
        public virtual bool Archive { get; set; }
    }
}
