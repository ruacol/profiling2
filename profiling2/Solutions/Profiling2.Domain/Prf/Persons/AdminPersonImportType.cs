using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Persons
{
    public class AdminPersonImportType : Entity
    {
        public virtual string AdminPersonImportTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.AdminPersonImportTypeName;
        }
    }
}
