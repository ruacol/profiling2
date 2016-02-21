using SharpArch.Domain.DomainModel;

namespace Profiling2.Domain.Prf.Responsibility
{
    public class PersonResponsibilityType : Entity
    {
        public virtual string PersonResponsibilityTypeName { get; set; }
        public virtual bool Archive { get; set; }
        public virtual string Notes { get; set; }

        public override string ToString()
        {
            return this.PersonResponsibilityTypeName;
        }

        public virtual object ToJSON()
        {
            return new
                {
                    Id = this.Id,
                    Name = this.PersonResponsibilityTypeName
                };
        }
    }
}
