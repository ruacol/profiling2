using System;
using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Careers
{
    public class AdminCareerImport : Entity
    {
        public virtual Career Career { get; set; }
        public virtual AdminCareerImportType AdminCareerImportType { get; set; }
        public virtual DateTime ImportDate { get; set; }
        public virtual int PreviousID { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }
    }
}
