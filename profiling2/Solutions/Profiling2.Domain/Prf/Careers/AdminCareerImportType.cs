using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Careers
{
    public class AdminCareerImportType : Entity
    {
        public virtual string AdminCareerImportTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.AdminCareerImportTypeName;
        }
    }
}
