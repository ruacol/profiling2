using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf
{
    public class FileType : Entity
    {
        public virtual string FileTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.FileTypeName;
        }
    }
}
